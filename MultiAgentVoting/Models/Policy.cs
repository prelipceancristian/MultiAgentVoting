namespace MultiAgentVoting.Models
{
    internal class Policy
    {
        public IDictionary<string, double> CriteriaEvaluations { get; private init; } = new Dictionary<string, double>();

        public static Policy GeneratePolicy()
        {
            var criteriaIds = new[] 
            { 
                "Economy",
                "Healthcare",
                "Education", 
                "Environment & Energy",
                "Foreign Affairs"
            };
            var criteriaEvaluations = criteriaIds
                .ToDictionary(criteriaId => criteriaId, _ => Utils.GenerateCriteriaEvaluation());

            return new Policy { CriteriaEvaluations = criteriaEvaluations };
        }
    }
}
