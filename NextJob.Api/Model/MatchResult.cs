namespace NextJob.Api.Model
{
    public class MatchResult
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; } = default!;

        public int JobOpeningId { get; set; }
        public JobOpening JobOpening { get; set; } = default!;

        // Pontuações calculadas
        public double RequiredSkillsScore { get; set; } // 0–100
        public double DesiredSkillsScore { get; set; }   // 0–100
        public double SoftSkillsScore { get; set; }      // 0–100

        public double TotalCompatibility { get; set; }   // Peso 60/30/10

        public string ResumeSuggestions { get; set; } = string.Empty;

        public string MissingSkills { get; set; } = string.Empty;

        public string RecommendedCourses { get; set; } = string.Empty;

        public string CareerPlan { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
