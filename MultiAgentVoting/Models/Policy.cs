namespace MultiAgentVoting.Models
{
    // TODO: this could be a record
    internal class Policy
    {
        public IDictionary<string, double> CriteriaEvaluations { get; set; } = new Dictionary<string, double>();

        public static Policy GeneratePolicy()
        {
            var criterias = new string[] 
            { 
                "Economy",
                "Healthcare",
                "Education", 
                "Environment & Energy",
                "Foreign affairs"
            };
            var criteriaEvaluations = criterias.ToDictionary(crit => crit, _ => Utils.GenerateCriteriaEvaluation());
            return new Policy { CriteriaEvaluations = criteriaEvaluations };
        }
    }
}
