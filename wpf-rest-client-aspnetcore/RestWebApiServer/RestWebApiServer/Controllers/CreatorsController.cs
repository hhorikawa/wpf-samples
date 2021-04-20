using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestWebApiServer.Data;
using RestWebApiServer.Models;

namespace RestWebApiServer.Controllers
{

[Route("api/[controller]")]
[ApiController]
public class CreatorsController : ControllerBase
{
    readonly RestWebApiServerContext _context;

    public CreatorsController(RestWebApiServerContext context)
    {
        _context = context;
    }

    // GET: api/Creators
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Creator>>> GetCreator()
    {
        return await _context.Creator.ToListAsync();
    }

        // GET: api/Creators/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Creator>> GetCreator(int id)
        {
            var creator = await _context.Creator.FindAsync(id);

            if (creator == null)
            {
                return NotFound();
            }

            return creator;
        }

        // PUT: api/Creators/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCreator(int id, Creator creator)
        {
            if (id != creator.Id)
            {
                return BadRequest();
            }

            _context.Entry(creator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreatorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Creators
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Creator>> PostCreator(Creator creator)
        {
            _context.Creator.Add(creator);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCreator", new { id = creator.Id }, creator);
        }

        // DELETE: api/Creators/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCreator(int id)
        {
            var creator = await _context.Creator.FindAsync(id);
            if (creator == null)
            {
                return NotFound();
            }

            _context.Creator.Remove(creator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CreatorExists(int id)
        {
            return _context.Creator.Any(e => e.Id == id);
        }
    }
}
