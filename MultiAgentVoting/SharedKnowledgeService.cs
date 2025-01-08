using System.Collections.Concurrent;
using MultiAgentVoting.Agents;
using MultiAgentVoting.VotingProtocols;

namespace MultiAgentVoting
{
    internal static class SharedKnowledgeService
    {
        public static IDictionary<CandidateAgent, bool> Candidates { get; } = new ConcurrentDictionary<CandidateAgent, bool>();

        public static IDictionary<VoterAgent, bool> Voters { get; } = new ConcurrentDictionary<VoterAgent, bool>();

        public static IVotingProtocol VotingProtocol { get; } = new ApprovalVotingProtocol();

        public static void RegisterCandidate(CandidateAgent candidate)
        {
            if (Candidates.ContainsKey(candidate))
            {
                throw new Exception($"Could not register candidate agent {candidate.Name} - agent is already registered!");
            }

            Candidates.TryAdd(candidate, true);
        }

        public static void RemoveCandidate(CandidateAgent candidate)
        {
            if (!Candidates.ContainsKey(candidate))
            {
                throw new Exception($"Could not remove candidate agent {candidate.Name} - agent is not registered!");
            }

            Candidates.Remove(candidate);
        }

        public static void RegisterVoter(VoterAgent voter)
        {
            if (Voters.ContainsKey(voter))
            {
                throw new Exception($"Could not register voter {voter.Name} - agent is already registered!");
            }

            Voters.TryAdd(voter, true);
        }
    }
}