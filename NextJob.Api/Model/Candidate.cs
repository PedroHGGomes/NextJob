namespace NextJob.Api.Model
{
    public class Candidate
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ResumeText { get; set; } = string.Empty; // Texto completo do currículo

        public string TechnicalSkills { get; set; } = string.Empty; // Ex.: "C#, SQL, Azure"

        public string SoftSkills { get; set; } = string.Empty; // Ex.: "Comunicação, Liderança"

        public string Certifications { get; set; } = string.Empty; // Ex.: "AZ-900, AWS CCP"

        public int YearsOfExperience { get; set; }

        public string CurrentRole { get; set; } = string.Empty;
    }
}
