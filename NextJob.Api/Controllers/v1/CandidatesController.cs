using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextJob.Api.Data;
using NextJob.Api.Model;

namespace NextJob.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CandidatesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CandidatesController> _logger;

        public CandidatesController(AppDbContext context, ILogger<CandidatesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/v1/Candidates?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<object>> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page e pageSize devem ser maiores que zero.");

            var query = _context.Candidates.AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

            string? NextLink()
            {
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                if (page >= totalPages) return null;
                return $"{baseUrl}?page={page + 1}&pageSize={pageSize}";
            }

            string? PrevLink()
            {
                if (page <= 1) return null;
                return $"{baseUrl}?page={page - 1}&pageSize={pageSize}";
            }

            var result = new
            {
                page,
                pageSize,
                totalItems,
                data = items,
                links = new
                {
                    self = $"{baseUrl}?page={page}&pageSize={pageSize}",
                    next = NextLink(),
                    prev = PrevLink(),
                    create = Url.Action(nameof(Create))
                }
            };

            return Ok(result);
        }

        // GET api/v1/Candidates/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return NotFound();

            var links = new
            {
                self = Url.Action(nameof(GetById), new { id }),
                update = Url.Action(nameof(Update), new { id }),
                delete = Url.Action(nameof(Delete), new { id })
            };

            return Ok(new { data = candidate, links });
        }

        // POST api/v1/Candidates
        [HttpPost]
        public async Task<ActionResult<Candidate>> Create(Candidate candidate)
        {
            _logger.LogInformation("Criando candidato {Name}", candidate.FullName);

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, candidate);
        }

        // PUT api/v1/Candidates/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Candidate update)
        {
            if (id != update.Id)
                return BadRequest("Id da rota e do corpo não conferem.");

            var exists = await _context.Candidates.AnyAsync(c => c.Id == id);
            if (!exists)
                return NotFound();

            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/v1/Candidates/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return NotFound();

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
