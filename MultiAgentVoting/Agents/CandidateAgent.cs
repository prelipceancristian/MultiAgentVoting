using ActressMas;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.Agents;

internal class CandidateAgent : Agent
{
    public Policy Policy { get; }

    public CandidateAgent(string name, Policy policy)
    {
        Name = name;
        Policy = policy;
    }

    public override void Setup()
    {
        SharedKnowledgeService.RegisterCandidate(this);
    }

    public override void Act(Message message)
    {
        var messageContent = (MessageContent)message.ContentObj;
        switch (messageContent.Action)
        {
            case MessageAction.Winner:
                var winner = (CandidateAgent)messageContent.Payload!;
                HandleResults(winner);
                break;

            case MessageAction.Start:

            case MessageAction.Vote:

            case MessageAction.VoteResponse:

            default:
                throw new Exception("Unknown action");
        }
    }

    private void HandleResults(CandidateAgent winner)
    {
        if (winner == this)
        {
            Console.WriteLine($"[{Name}] I won!");
        }
        else
        {
            // There's a small chance that the candidate is a sore loser. 
            if (Utils.Rng.NextDouble() < 0.01)
            {
                Console.WriteLine($"[{Name}] Rigged!");
            }
            else
            {
                Console.WriteLine($"[{Name}] I lost, congrats.");
            }
        }

        Stop();
    }
}