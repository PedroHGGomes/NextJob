namespace NextJob.Api.Model
{
    public class JobOpening
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string RequiredSkills { get; set; } = string.Empty;   

        public string DesiredSkills { get; set; } = string.Empty;    

        public string SoftSkills { get; set; } = string.Empty;

        public string Seniority { get; set; } = string.Empty;        

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
