using ActressMas;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.Agents
{
    internal class CandidateAgent : Agent
    {
        public Policy Policy;

        public CandidateAgent(string name, Policy policy)
        {
            Name = name;
            Policy = policy;
        }

        public void PublishPolicy()
        {
            SharedKnowledgeService.RegisterCandidate(this);
        }

        public void HandleResults(string winnerName)
        {
            if (Name == winnerName)
            {
                Console.WriteLine($"[Candidate {Name}] I won!");
            }
            else
            {
                // There's a small chance that the candidate is a sore loser. 
                if (Utils.Rng.NextDouble() < 0.01)
                {
                    Console.WriteLine($"[Candidate {Name}] Rigged!");
                }
                else
                {
                    Console.WriteLine($"[Candidate {Name}] I lost, congrats.");
                }
            }
        }
    }
}
