using MultiAgentVoting.Agents;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.VotingProtocols;

internal interface IVotingProtocol
{
    public Vote Vote(VoterAgent voter, List<Rating> ratings);

    public void UpdateVotes(Dictionary<CandidateAgent, List<VoterAgent>> votes, Vote vote);
    
    public ElectionResult EstablishWinner(Dictionary<CandidateAgent, List<VoterAgent>> votes);
}

internal abstract record ElectionResult;

internal record SuccessfulElectionResult(CandidateAgent Winner) : ElectionResult;

internal record FailedElectionResult(CandidateAgent RemovedCandidate) : ElectionResult;

