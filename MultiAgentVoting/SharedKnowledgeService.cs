using System.Collections.Concurrent;
using MultiAgentVoting.Agents;
using MultiAgentVoting.VotingProtocols;

namespace MultiAgentVoting
{
    internal static class SharedKnowledgeService
    {
        public static ConcurrentDictionary<CandidateAgent, bool> Registrations { get; } = [];

        public static ConcurrentDictionary<VoterAgent, bool> Voters { get; } = [];

        public static IVotingProtocol VotingProtocol { get; } = new StvProtocol();

        public static void RegisterCandidate(CandidateAgent candidate)
        {
            if (Registrations.ContainsKey(candidate))
            {
                throw new Exception($"Could not register candidate agent {candidate.Name} - agent is already registered!");
            }

            Registrations.TryAdd(candidate, true);
        }

        public static void RemoveCandidate(CandidateAgent candidate)
        {
            if (!Registrations.ContainsKey(candidate))
            {
                throw new Exception($"Could not remove candidate agent {candidate.Name} - agent is not registered!");
            }
        
            Registrations.TryRemove(candidate, out _);
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
