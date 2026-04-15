using Educobros.Data;
using Educobros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Educobros.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudiantesApiController : ControllerBase
    {
        private readonly EduCobrosContext _context;

        public EstudiantesApiController(EduCobrosContext context)
        {
            _context = context;
        }

        // GET: api/estudiantesapi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estudiante>>> GetEstudiantes()
        {
            return await _context.Estudiantes.ToListAsync();
        }

        // GET: api/estudiantesapi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Estudiante>> GetEstudiante(int id)
        {
            var est = await _context.Estudiantes
                .Include(e => e.Pagos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (est == null) return NotFound();
            return est;
        }

        // POST: api/estudiantesapi
        [HttpPost]
        public async Task<ActionResult<Estudiante>> PostEstudiante(Estudiante est)
        {
            _context.Estudiantes.Add(est);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEstudiante),
                new { id = est.Id }, est);
        }

        // DELETE: api/estudiantesapi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstudiante(int id)
        {
            var est = await _context.Estudiantes.FindAsync(id);
            if (est == null) return NotFound();
            _context.Estudiantes.Remove(est);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/estudiantesapi/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult> GetDashboard()
        {
            return Ok(new
            {
                totalEstudiantes = await _context.Estudiantes.CountAsync(),
                enMora = await _context.Estudiantes.CountAsync(e => e.MesesDebidos > 0),
                totalRecaudado = await _context.Pagos.SumAsync(p => (decimal?)p.Monto) ?? 0,
                totalMora = await _context.Estudiantes
                    .Where(e => e.MesesDebidos > 0)
                    .SumAsync(e => (decimal?)(e.Mensualidad * e.MesesDebidos)) ?? 0
            });
        }
    }
}