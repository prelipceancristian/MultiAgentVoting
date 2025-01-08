using ActressMas;
using MultiAgentVoting.Agents;
using MultiAgentVoting.Models;

namespace MultiAgentVoting;

internal static class Program
{
    private static List<CandidateAgent> GenerateCandidates(int candidateCount)
    {
        var candidates = new List<CandidateAgent>();
        for (var i = 0; i < candidateCount; i++)
        {
            var candidatePolicy = Policy.GeneratePolicy();
            var candidateName = $"Candidate_{i}";
            var candidate = new CandidateAgent(candidateName, candidatePolicy);
            candidates.Add(candidate);
        }

        return candidates;
    }

    private static List<VoterAgent> GenerateVoters(int voterCount)
    {
        var candidates = new List<VoterAgent>();
        for (var i = 0; i < voterCount; i++)
        {
            var voterPolicy = Policy.GeneratePolicy();
            var voterName = $"Voter_{i}";
            var voter = new VoterAgent(voterName, voterPolicy);
            candidates.Add(voter);
        }

        return candidates;
    }

    private static void Main(string[] args)
    {
        const int candidateCount = 10;
        const int voterCount = 100;

        var environment = new EnvironmentMas();
        var candidateAgents = GenerateCandidates(candidateCount);
        var voterAgents = GenerateVoters(voterCount);
        var moderatorAgent = new ModeratorAgent("Moderator");

        foreach (var agent in candidateAgents)
        {
            environment.Add(agent);
        }

        foreach (var agent in voterAgents)
        {
            environment.Add(agent);
        }

        environment.Add(moderatorAgent);
        environment.Start();
    }
}