using EduCobros.Data;
using EduCobros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Educobros.Controllers
{
    public class PagosController : Controller
    {
        private readonly EduCobrosContext _context;

        public PagosController(EduCobrosContext context)
        {
            _context = context;
        }

        // GET: Pagos
        public async Task<IActionResult> Index()
        {
            var pagos = await _context.Pagos
                .Include(p => p.Estudiante)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pagos);
        }

        // GET: Pagos/Create
        public async Task<IActionResult> Create()
        {
            await CargarEstudiantesAsync();
            return View();
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (!ModelState.IsValid)
            {
                await CargarEstudiantesAsync(pago.EstudianteId);
                return View(pago);
            }

            // Validación: el estudiante debe existir
            var estudianteExiste = await _context.Estudiantes
                .AnyAsync(e => e.Id == pago.EstudianteId);

            if (!estudianteExiste)
            {
                ModelState.AddModelError("EstudianteId", "El estudiante seleccionado no existe.");
                await CargarEstudiantesAsync(pago.EstudianteId);
                return View(pago);
            }

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.Estudiante)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
                return NotFound();

            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago != null)
            {
                _context.Pagos.Remove(pago);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarEstudiantesAsync(object? estudianteSeleccionado = null)
        {
            var estudiantes = await _context.Estudiantes
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.EstudianteId = new SelectList(
                estudiantes,
                "Id",
                "NombreCompleto",
                estudianteSeleccionado
            );
        }
    }
}
