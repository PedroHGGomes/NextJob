using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextJob.Api.Data;
using NextJob.Api.Model;
using NextJob.Api.Model.Requests;
using System;
using System.Linq;

namespace NextJob.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatchController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/v1/Match
        [HttpPost]
        public async Task<ActionResult<object>> CalculateMatch(MatchRequest request)
        {
            var candidate = await _context.Candidates.FindAsync(request.CandidateId);
            var job = await _context.JobOpenings.FindAsync(request.JobOpeningId);

            if (candidate == null || job == null)
                return NotFound("Candidato ou Vaga não encontrados.");

            // Função simples para calcular "match" entre listas de skills
            double CalcScore(string candidateText, string jobText)
            {
                if (string.IsNullOrWhiteSpace(candidateText) || string.IsNullOrWhiteSpace(jobText))
                    return 0;

                var candidateTokens = candidateText.ToLower()
                    .Split(',', ';', '/', ' ')
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct()
                    .ToHashSet();

                var jobTokens = jobText.ToLower()
                    .Split(',', ';', '/', ' ')
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct()
                    .ToList();

                if (!jobTokens.Any())
                    return 0;

                var matchCount = jobTokens.Count(t => candidateTokens.Contains(t));
                return (double)matchCount / jobTokens.Count * 100.0;
            }

            var requiredScore = CalcScore(candidate.TechnicalSkills, job.RequiredSkills);
            var desiredScore = CalcScore(candidate.TechnicalSkills, job.DesiredSkills);
            var softScore = CalcScore(candidate.SoftSkills, job.SoftSkills);

            var total = requiredScore * 0.6 + desiredScore * 0.3 + softScore * 0.1;

            // Recomendações "fake IA" (depois dá pra ligar com ML.NET/GPT se quiser)
            var resumeSuggestions = "Reescreva suas experiências destacando resultados mensuráveis e usando palavras-chave da vaga.";
            var missingSkills = "Revise a seção de habilidades técnicas e adicione competências importantes que aparecem na vaga.";
            var recommendedCourses = "Sugestão: cursos de C# Avançado, Cloud Computing e Soft Skills de comunicação.";
            var careerPlan = "Objetivo recomendado: crescer para Desenvolvedor .NET Pleno em 2-3 anos e Engenheiro de Software Sênior até 2030.";

            var matchResult = new MatchResult
            {
                CandidateId = candidate.Id,
                JobOpeningId = job.Id,
                RequiredSkillsScore = requiredScore,
                DesiredSkillsScore = desiredScore,
                SoftSkillsScore = softScore,
                TotalCompatibility = total,
                ResumeSuggestions = resumeSuggestions,
                MissingSkills = missingSkills,
                RecommendedCourses = recommendedCourses,
                CareerPlan = careerPlan
            };

            _context.MatchResults.Add(matchResult);
            await _context.SaveChangesAsync();

            var links = new
            {
                self = Url.Action(nameof(GetMatchById), new { id = matchResult.Id }),
                candidate = Url.Action("GetById", "Candidates", new { id = candidate.Id }),
                job = Url.Action("GetById", "JobOpenings", new { id = job.Id })
            };

            return CreatedAtAction(nameof(GetMatchById), new { id = matchResult.Id }, new { data = matchResult, links });
        }

        // GET api/v1/Match/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetMatchById(int id)
        {
            var match = await _context.MatchResults
                .Include(m => m.Candidate)
                .Include(m => m.JobOpening)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound();

            return Ok(new { data = match });
        }
    }
}
