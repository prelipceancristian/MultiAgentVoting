using ActressMas;
using MultiAgentVoting.Models;
using MultiAgentVoting.VotingProtocols;

namespace MultiAgentVoting.Agents
{
    internal class ModeratorAgent : Agent
    {
        private Dictionary<CandidateAgent, List<VoterAgent>> _votes = new();

        public ModeratorAgent(string name)
        {
            Name = name;
        }

        // This method is defined as async void to obtain the "fire and forget" behavior
        public override async void Setup()
        {
            SendRegisterToCandidates();
            Console.WriteLine("[Moderator agent] Waiting for candidates to register...");
            await Task.Delay(2000);
            
            var registeredCandidates = SharedKnowledgeService.Registrations;
            _votes = registeredCandidates.ToDictionary(candidate => candidate, _ => new List<VoterAgent>());
            
            SendVoteToVoters();
            Console.WriteLine("[Moderator agent] Waiting for votes...");
            await Task.Delay(2000);

            EstablishAndSendWinner();
        }
        
        public override void Act(Message message)
        {
            var messageContent = (MessageContent)message.ContentObj;
            
            switch (messageContent.Action)
            {
                case MessageAction.VoteResponse:
                    HandleVoteResponse(messageContent);
                    break;
                case MessageAction.Winner:
                    Stop();
                    break;
                default:
                    throw new Exception("Could not parse message content");
            }
        }

        private void SendRegisterToCandidates()
        {
            var candidateAgents = Environment.FilteredAgents("Candidate");
            var registerMessageContent = new MessageContent(MessageAction.Register, null!);
            SendToMany(candidateAgents, registerMessageContent);
        }

        private void SendVoteToVoters()
        {
            var voterAgentNames = Environment.FilteredAgents("Voter");
            var messageContent = new MessageContent(MessageAction.Vote, null!);
            SendToMany(voterAgentNames, messageContent);
        }

        private void EstablishAndSendWinner()
        {
            Console.WriteLine("Results:");
            foreach (var kvp in _votes)
            {
                Console.WriteLine($"{kvp.Key.Name}: {kvp.Value.Count}");
            }

            var electionResult = SharedKnowledgeService.VotingProtocol.EstablishWinner(_votes);
            switch (electionResult)
            {
                case SuccessfulElectionResult successfulElectionResult:
                    SendWinnerToAll(successfulElectionResult.Winner);
                    break;
                default:
                    throw new Exception("Unsupported election result");
            }
        }

        private void SendWinnerToAll(CandidateAgent winner)
        {
            var messageContent = new MessageContent(MessageAction.Winner, winner);
            Console.WriteLine($"[Moderator agent] And the winner is {winner.Name}!");
            Broadcast(messageContent, includeSender: true);
        }

        private void HandleVoteResponse(MessageContent messageContent)
        {
            var vote = (Vote)messageContent.Payload;
            var votingProtocol = SharedKnowledgeService.VotingProtocol;
            votingProtocol.UpdateVotes(_votes, vote);
        }

        // private (CandidateAgent Winner, int VoteCount) EstablishPluralityVotingWinner()
        // {
        //     return GetCandidateWithMostVotes();
        // }
        
        // public (CandidateAgent Winner, int VoteCount) EstablishApprovalVoteWinner()
        // {
        //     return GetCandidateWithMostVotes();
        // }

        // public (CandidateAgent Winner, int VoteCount)? EstablishSingleTransferableVote()
        // {
        //     var (winner, voteCount) = GetCandidateWithMostVotes();
        //     var totalVoteCount = _votes.SelectMany(kvp => kvp.Value).Count();
        //     if (voteCount > (totalVoteCount / 2.0))
        //     {
        //         return (winner, voteCount);
        //     }
        //
        //     // no winner was established. The moderator must start proceedings for another vote session
        //     Console.WriteLine("A majority voter could not be established");
        //     return null;
        // }

        // private (CandidateAgent Winner, int VoteCount) GetCandidateWithMostVotes()
        // {
        //     var mostNumberOfVotes = _votes.Select(kvp => kvp.Value.Count).Max();
        //     var candidatesWithMostVotes = _votes
        //         .Where(kvp => kvp.Value.Count == mostNumberOfVotes)
        //         .Select(kvp => kvp.Key)
        //         .ToList();
        //     var winner = Utils.PickRandom(candidatesWithMostVotes);
        //     return (winner, mostNumberOfVotes);
        // }

        // public void AcceptPluralityVote(SingularVote vote)
        // {
        //     // assume Votes already contains all available candidates
        //     if (!Votes.TryGetValue(vote.CandidateAgent, out var candidateVoters))
        //     {
        //         throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} failed to vote for {vote.CandidateAgent.Name}. The candidate is not registered");
        //     }
        //     if (candidateVoters.Contains(vote.VoterAgent))
        //     {
        //         throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} already voted for {vote.CandidateAgent.Name}");
        //     }
        //     candidateVoters.Add(vote.VoterAgent);
        // }

        // public void AcceptSingleTransferableVote(MultipleVote vote)
        // {
        //     foreach (var candidate in vote.ViableCandidates)
        //     {
        //         if (!Votes.TryGetValue(candidate, out var candidateVoters))
        //         {
        //             throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} failed to vote for {candidate.Name}. The candidate is not registered");
        //         }
        //         if (candidateVoters.Contains(vote.VoterAgent))
        //         {
        //             throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} already voted for {candidate.Name}");
        //         }
        //         candidateVoters.Add(vote.VoterAgent);
        //     }
        // }
    }
}
