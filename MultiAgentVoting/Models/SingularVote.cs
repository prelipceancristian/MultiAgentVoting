using MultiAgentVoting.Agents;

namespace MultiAgentVoting.Models
{
    internal class SingularVote : Vote
    {
        public CandidateAgent CandidateAgent { get; set; }

        public SingularVote(VoterAgent voterAgent, CandidateAgent candidateAgent) : base(voterAgent)
        { 
            CandidateAgent = candidateAgent;
        }
    }
}
