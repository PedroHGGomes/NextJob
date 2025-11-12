using Microsoft.EntityFrameworkCore;
using NextJob.Api.Model;

namespace NextJob.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Candidate> Candidates => Set<Candidate>();
        public DbSet<JobOpening> JobOpenings => Set<JobOpening>();
        public DbSet<MatchResult> MatchResults => Set<MatchResult>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabelas com nomes Oracle-friendly (tudo maiúsculo)
            modelBuilder.Entity<Candidate>().ToTable("CANDIDATE");
            modelBuilder.Entity<JobOpening>().ToTable("JOB_OPENING");
            modelBuilder.Entity<MatchResult>().ToTable("MATCH_RESULT");

            // Exemplo: chaves estrangeiras para o MatchResult
            modelBuilder.Entity<MatchResult>()
                .HasOne(m => m.Candidate)
                .WithMany()
                .HasForeignKey(m => m.CandidateId);

            modelBuilder.Entity<MatchResult>()
                .HasOne(m => m.JobOpening)
                .WithMany()
                .HasForeignKey(m => m.JobOpeningId);
        }
    }
}

