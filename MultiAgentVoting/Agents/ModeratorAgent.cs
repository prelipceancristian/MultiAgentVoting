using ActressMas;
using MultiAgentVoting.Models;
using MultiAgentVoting.VotingProtocols;

namespace MultiAgentVoting.Agents;

internal class ModeratorAgent : Agent
{
    private Dictionary<CandidateAgent, List<VoterAgent>> _votes = new();
    private Dictionary<VoterAgent, bool> _votersStatus = new();

    public ModeratorAgent(string name)
    {
        Name = name;
    }

    public override void Setup()
    {
        var messageContent = new MessageContent(MessageAction.Start, null!);
        Send(Name, messageContent);
    }

    public override void Act(Message message)
    {
        var messageContent = (MessageContent)message.ContentObj;
            
        switch (messageContent.Action)
        {
            case MessageAction.Start:
                HandleStart();
                break;

            case MessageAction.VoteResponse:
                HandleVoteResponse(messageContent);
                break;

            case MessageAction.Winner:
                Stop();
                break;

            case MessageAction.Vote:

            default:
                throw new Exception("Could not parse message content");
        }
    }

    private void HandleStart()
    {
        _votes = SharedKnowledgeService.Candidates.ToDictionary(kvp => kvp.Key, _ => new List<VoterAgent>());
        _votersStatus = SharedKnowledgeService.Voters.ToDictionary(kvp => kvp.Key, _ => false);
        var voterNames = _votersStatus.Select(kvp => kvp.Key.Name).ToList();
        SendVoteToVoters(voterNames);
    }
    
    private void HandleVoteResponse(MessageContent messageContent)
    {
        var vote = (Vote)messageContent.Payload!;
        var votingProtocol = SharedKnowledgeService.VotingProtocol;
        votingProtocol.UpdateVotes(_votes, vote);
        _votersStatus[vote.VoterAgent] = true;
        if (_votersStatus.All(v => v.Value))
        {
            AnalyzeElectionResults();
        }
    }

    private void SendVoteToVoters(List<string> voterNames)
    {
        var messageContent = new MessageContent(MessageAction.Vote, null);
        SendToMany(voterNames, messageContent);
    }

    private void AnalyzeElectionResults()
    {
        DisplayElectionResults();
        var electionResult = SharedKnowledgeService.VotingProtocol.EstablishWinner(_votes);
        switch (electionResult)
        {
            case SuccessfulElectionResult successfulElectionResult:
                SendWinnerToAll(successfulElectionResult.Winner);
                break;
            case FailedElectionResult failedElectionResult:
                RestartElectionProcess(failedElectionResult);
                break;
            default:
                throw new Exception("Unsupported election result");
        }
    }

    private void DisplayElectionResults()
    {
        Console.WriteLine("[Moderator] Results:");
        foreach (var kvp in _votes)
        {
            Console.WriteLine($"{kvp.Key.Name}: {kvp.Value.Count}");
        }
    }

    private void RestartElectionProcess(FailedElectionResult failedElectionResult)
    {
        Console.WriteLine($"[Moderator] No candidate reached 50% of the votes. " +
                          $"Removing candidate {failedElectionResult.RemovedCandidate.Name} and trying again.");
        SharedKnowledgeService.RemoveCandidate(failedElectionResult.RemovedCandidate);
        var votersToRetry = _votes[failedElectionResult.RemovedCandidate];
        _votersStatus = votersToRetry.ToDictionary(voter => voter, _ => false);
        _votes.Remove(failedElectionResult.RemovedCandidate);
        var voterNames = votersToRetry.Select(x => x.Name).ToList();
        SendVoteToVoters(voterNames);
    }

    private void SendWinnerToAll(CandidateAgent winner)
    {
        var messageContent = new MessageContent(MessageAction.Winner, winner);
        Console.WriteLine($"[Moderator] And the winner is {winner.Name}!");
        Broadcast(messageContent, includeSender: true);
    }
}