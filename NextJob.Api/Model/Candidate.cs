namespace NextJob.Api.Model
{
    public class Candidate
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ResumeText { get; set; } = string.Empty; 

        public string TechnicalSkills { get; set; } = string.Empty; 

        public string SoftSkills { get; set; } = string.Empty; 

        public string Certifications { get; set; } = string.Empty;

        public int YearsOfExperience { get; set; }

        public string CurrentRole { get; set; } = string.Empty;
    }
}
