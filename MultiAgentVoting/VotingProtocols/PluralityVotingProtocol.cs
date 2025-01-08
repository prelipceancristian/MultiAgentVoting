using MultiAgentVoting.Agents;
using MultiAgentVoting.Models;

namespace MultiAgentVoting.VotingProtocols;

internal class PluralityVotingProtocol : IVotingProtocol
{
    public void UpdateVotes(Dictionary<CandidateAgent, List<VoterAgent>> votes, Vote vote)
    {
        // assume votes already contains all available candidates
        if (vote.ViableCandidates.Count != 1)
        {
            throw new Exception("Invalid vote count");
        }
        var candidate = vote.ViableCandidates[0];
        if (!votes.TryGetValue(candidate, out var candidateVoters))
        {
            throw new Exception($"Voter {vote.VoterAgent.Name} failed to vote for {candidate.Name}. The candidate is not registered");
        }
        if (candidateVoters.Contains(vote.VoterAgent))
        {
            throw new Exception($"Voter {vote.VoterAgent.Name} already voted for {candidate.Name}");
        }
        candidateVoters.Add(vote.VoterAgent);
    }

    public Vote Vote(VoterAgent voter, List<Rating> ratings)
    {
        var bestRating = ratings.First().Value;
        var bestCandidates = ratings.Where(r => Math.Abs(r.Value - bestRating) < 1e-6).ToList();
        var candidate = Utils.PickRandom(bestCandidates).Candidate;
        var vote = new Vote(voter, [candidate]);
        return vote;
    }
    
    public ElectionResult EstablishWinner(Dictionary<CandidateAgent, List<VoterAgent>> votes)
    {
        var mostNumberOfVotes = votes.Select(kvp => kvp.Value.Count).Max();
        var candidatesWithMostVotes = votes
            .Where(kvp => kvp.Value.Count == mostNumberOfVotes)
            .Select(kvp => kvp.Key)
            .ToList();
        var winner = Utils.PickRandom(candidatesWithMostVotes);
        return new SuccessfulElectionResult(winner);
    }
}