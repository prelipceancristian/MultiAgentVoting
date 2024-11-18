using MultiAgentVoting.Agents;

namespace MultiAgentVoting.Models
{
    internal abstract class Vote
    {
        public VoterAgent VoterAgent { get; set; }

        protected Vote(VoterAgent voterAgent)
        {
            VoterAgent = voterAgent;
        }
    }
}
