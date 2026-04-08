using Educobros.Data;
using Educobros.Models;
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
        public IActionResult Create(int? estudianteId)
        {
            CargarEstudiantes(estudianteId);

            var pago = new Pago();

            if (estudianteId.HasValue)
                pago.EstudianteId = estudianteId.Value;

            return View(pago);
        }  
        

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            CargarEstudiantes(pago.EstudianteId);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago != null)
            {
                _context.Pagos.Remove(pago);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private void CargarEstudiantes(object? estudianteSeleccionado = null)
        {
            ViewBag.EstudianteId = new SelectList(
                _context.Estudiantes.ToList(),
                "Id",
                "Nombre",
                estudianteSeleccionado
            );
        }
    
    }
}
