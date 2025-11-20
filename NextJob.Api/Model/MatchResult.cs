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
        public double RequiredSkillsScore { get; set; } 
        public double DesiredSkillsScore { get; set; }   
        public double SoftSkillsScore { get; set; }     

        public double TotalCompatibility { get; set; }   

        public string ResumeSuggestions { get; set; } = string.Empty;

        public string MissingSkills { get; set; } = string.Empty;

        public string RecommendedCourses { get; set; } = string.Empty;

        public string CareerPlan { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
