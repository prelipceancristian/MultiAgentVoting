using ActressMas;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.Agents
{
    internal class ModeratorAgent : Agent
    {
        // stabileste winner
        // comunica winner

        // trimite ok pt inregistrare
        // trimite ok pt considerare

        // accepta voturi
        public Dictionary<CandidateAgent, List<VoterAgent>> Votes = [];

        public override async void Setup()
        {
            var voterAgentNames = Environment.FilteredAgents("Voter");
            var messageContent = new MessageContent(MessageAction.Vote, VotingProtocol.Approval);
            SendToMany(voterAgentNames, messageContent);
        }

        
        public (CandidateAgent Winner, int VoteCount) EstablishPluralityVotingWinner()
        {
            return GetCandidateWithMostVotes();
        }

        public (CandidateAgent Winner, int VoteCount) EstablishApprovalVoteWinner()
        {
            return GetCandidateWithMostVotes();
        }

        public (CandidateAgent Winner, int VoteCount)? EstablishSingleTransferableVote()
        {
            var (winner, voteCount) = GetCandidateWithMostVotes();
            var totalVoteCount = Votes.SelectMany(kvp => kvp.Value).Count();
            if (voteCount > (totalVoteCount / 2.0))
            {
                return (winner, voteCount);
            }

            // no winner was established. The moderator must start proceedings for another vote session
            Console.WriteLine("A majority voter could not be established");
            return null;
        }

        private (CandidateAgent Winner, int VoteCount) GetCandidateWithMostVotes()
        {
            var mostNumberOfVotes = Votes.Select(kvp => kvp.Value.Count).Max();
            var candidatesWithMostVotes = Votes
                .Where(kvp => kvp.Value.Count == mostNumberOfVotes)
                .Select(kvp => kvp.Key)
                .ToList();
            var winner = Utils.PickRandom(candidatesWithMostVotes);
            return (winner, mostNumberOfVotes);
        }

        public void AcceptPluralityVote(SingularVote vote)
        {
            // assume Votes already contains all available candidates
            if (!Votes.TryGetValue(vote.CandidateAgent, out var candidateVoters))
            {
                throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} failed to vote for {vote.CandidateAgent.Name}. The candidate is not registered");
            }
            if (candidateVoters.Contains(vote.VoterAgent))
            {
                throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} already voted for {vote.CandidateAgent.Name}");
            }
            candidateVoters.Add(vote.VoterAgent);
        }

        public void AcceptSingleTransferableVote(MultipleVote vote)
        {
            foreach (var candidate in vote.ViableCandidates)
            {
                if (!Votes.TryGetValue(candidate, out var candidateVoters))
                {
                    throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} failed to vote for {candidate.Name}. The candidate is not registered");
                }
                if (candidateVoters.Contains(vote.VoterAgent))
                {
                    throw new Exception($"[Moderator agent] Voter {vote.VoterAgent.Name} already voted for {candidate.Name}");
                }
                candidateVoters.Add(vote.VoterAgent);
            }
        }
    }
}
