﻿using MultiAgentVoting.Agents;
using MultiAgentVoting.Models;
using MultiAgentVoting.VotingProtocols;

namespace MultiAgentVoting
{
    internal static class SharedKnowledgeService
    {
        public static List<CandidateAgent> Registrations { get; } = [];

        public static IVotingProtocol VotingProtocol { get; } = new ApprovalVotingProtocol();

        public static void RegisterCandidate(CandidateAgent candidate)
        {
            if (Registrations.Contains(candidate))
            {
                throw new Exception($"Could not register candidate agent {candidate.Name} - agent is already registered!");
            }
            Registrations.Add(candidate);
        }

        public static void RemoveCandidateByName(string name)
        {
            var candidate = Registrations.SingleOrDefault(x => x.Name == name);
            if (candidate is null)
            {
                throw new Exception($"Could not remove candidate agent {name} - agent is not registered!");
            }
            Registrations.Remove(candidate);
        }
    }
}
