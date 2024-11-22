using MultiAgentVoting.Agents;

namespace MultiAgentVoting.Models
{
    internal record Vote(VoterAgent VoterAgent, List<CandidateAgent> ViableCandidates);
}
