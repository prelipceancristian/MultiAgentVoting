using ActressMas;
using MultiAgentVoting.Agents;
using MultiAgentVoting.Exports;
using MultiAgentVoting.Models;
using MultiAgentVoting.VotingProtocols;
using System.Collections.Concurrent;
using System.Text.Json;

namespace MultiAgentVoting;

internal static class Program
{
    const int candidateCount = 10;
    const int voterCount = 10000;

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

    private static string StartEnv(int seed)
    {
        Utils.Rng = new Random(seed);

        SharedKnowledgeService.Voters = new ConcurrentDictionary<VoterAgent, bool>();
        SharedKnowledgeService.Candidates = new ConcurrentDictionary<CandidateAgent, bool>();


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

        return moderatorAgent.Winner;
    }

    private static ProtocolRunResult ExportProtocolResult(int seed, IVotingProtocol protocol)
    {
        SharedKnowledgeService.VotingProtocol = protocol;
        Console.WriteLine($"Run {seed} for {protocol.Name}");
        var winnerName = StartEnv(seed);
        var protocolRunResult = new ProtocolRunResult
        {
            ProtocolName = protocol.Name,
            Winner = winnerName
        };
        return protocolRunResult;
    }

    private static void Main(string[] args)
    {
        const int noOfRuns = 3000;
        var exportRunList = new List<ExportRun>();
        
        for (int i = 1; i <= noOfRuns; i++)
        {
            var exportRun = new ExportRun
            {
                Seed = i,
                Results = [
                    ExportProtocolResult(i, new ApprovalVotingProtocol()),
                    ExportProtocolResult(i, new PluralityVotingProtocol()),
                    ExportProtocolResult(i, new StvProtocol()),
                ]
            };

            exportRunList.Add(exportRun);
        }

        var exportExperiment = new ExportExperiment
        {
            VoterCount = voterCount,
            CandidateCount = candidateCount,
            ExportRuns = exportRunList
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        string jsonResult = JsonSerializer.Serialize(exportExperiment, options);
        File.WriteAllText("data.json", jsonResult);
    }
}