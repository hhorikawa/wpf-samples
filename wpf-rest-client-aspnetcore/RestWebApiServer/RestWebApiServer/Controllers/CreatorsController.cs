using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestWebApiServer.Data;
using RestWebApiServer.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace RestWebApiServer.Controllers
{

//[Route("api/[controller]")]
[Route("api/creators")]
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
    [SwaggerOperation(OperationId = "FindAllCreators")] 
    public async Task<ActionResult<IEnumerable<Creator>>> GetCreator()
    {
        return await _context.Creator.ToListAsync();
    }

        // GET: api/Creators/5
    [HttpGet("{id}")]
    [SwaggerOperation(OperationId = "FindCreator")] 
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
    [SwaggerOperation(OperationId = "UpdateCreator")] 
    public async Task<IActionResult> PutCreator(int id, [FromBody] Creator creator)
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
    [SwaggerOperation(OperationId = "CreateCreator")] 
    public async Task<ActionResult<Creator>> PostCreator([FromBody] Creator creator)
    {
            _context.Creator.Add(creator);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCreator", new { id = creator.Id }, creator);
        }

        // DELETE: api/Creators/5
    [HttpDelete("{id}")]
    [SwaggerOperation(OperationId = "DestroyCreator")]
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
