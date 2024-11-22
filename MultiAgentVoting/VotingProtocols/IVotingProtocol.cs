using MultiAgentVoting.Agents;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.VotingProtocols;

//NOTE: could pass just voter, as the voter agent in the current implementation has the ratings as a prop
//NOTE: actually, could just pass the agents directly
internal interface IVotingProtocol
{
    public Vote Vote(VoterAgent voter, List<Rating> ratings);

    public void UpdateVotes(Dictionary<CandidateAgent, List<VoterAgent>> votes, Vote vote);
    
    public ElectionResult EstablishWinner(Dictionary<CandidateAgent, List<VoterAgent>> votes);
}

internal abstract record ElectionResult;

internal record SuccessfulElectionResult(CandidateAgent Winner) : ElectionResult;

internal record FailedElectionResult(CandidateAgent RemovedCandidate) : ElectionResult;

