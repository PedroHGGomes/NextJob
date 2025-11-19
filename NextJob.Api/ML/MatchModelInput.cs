using Microsoft.ML.Data;

namespace NextJob.Api.ML
{
    // Dados de entrada
    public class MatchModelInput
    {
        public float RequiredSkillsScore { get; set; }
        public float DesiredSkillsScore { get; set; }
        public float SoftSkillsScore { get; set; }

        // Anos
        public float YearsOfExperience { get; set; }
    }

    // Saída do modelo
    public class MatchModelOutput
    {
        // Score de compatibilidade
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
