using MultiAgentVoting.Agents;

namespace MultiAgentVoting.Models
{
    internal class MultipleVote : Vote
    {
        public List<CandidateAgent> ViableCandidates { get; set; }

        public MultipleVote(VoterAgent voterAgent, List<CandidateAgent> candidateAgents) : base(voterAgent)
        {
            ViableCandidates = candidateAgents;
        }
    }
}
