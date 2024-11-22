using ActressMas;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.Agents
{
    internal class CandidateAgent : Agent
    {
        public Policy Policy { get; }

        public CandidateAgent(string name, Policy policy)
        {
            Name = name;
            Policy = policy;
        }
        
        public override void Act(Message message)
        {
            var messageContent = (MessageContent)message.ContentObj;
            switch (messageContent.Action)
            {
                case MessageAction.Register:
                    Register();
                    break;
                case MessageAction.Winner:
                    var winner = (CandidateAgent)messageContent.Payload;
                    HandleResults(winner);
                    break;
                default:
                    throw new Exception("Unknown action");
            }
        }

        private void Register()
        {
            SharedKnowledgeService.RegisterCandidate(this);
        }

        private void HandleResults(CandidateAgent winner)
        {
            if (Name == winner.Name)
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
            Stop();
        }
    }
}
