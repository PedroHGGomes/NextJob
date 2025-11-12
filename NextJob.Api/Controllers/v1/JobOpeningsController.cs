using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextJob.Api.Data;
using NextJob.Api.Model;

namespace NextJob.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class JobOpeningsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobOpeningsController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/v1/JobOpenings?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<object>> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page e pageSize devem ser maiores que zero.");

            var query = _context.JobOpenings.AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(j => j.Id)
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

        // GET api/v1/JobOpenings/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var job = await _context.JobOpenings.FindAsync(id);
            if (job == null)
                return NotFound();

            var links = new
            {
                self = Url.Action(nameof(GetById), new { id }),
                update = Url.Action(nameof(Update), new { id }),
                delete = Url.Action(nameof(Delete), new { id })
            };

            return Ok(new { data = job, links });
        }

        // POST api/v1/JobOpenings
        [HttpPost]
        public async Task<ActionResult<JobOpening>> Create(JobOpening job)
        {
            _context.JobOpenings.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }

        // PUT api/v1/JobOpenings/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, JobOpening update)
        {
            if (id != update.Id)
                return BadRequest("Id da rota e do corpo não conferem.");

            var exists = await _context.JobOpenings.AnyAsync(j => j.Id == id);
            if (!exists)
                return NotFound();

            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/v1/JobOpenings/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _context.JobOpenings.FindAsync(id);
            if (job == null)
                return NotFound();

            _context.JobOpenings.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
