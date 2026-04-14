using Educobros.Data;
using Educobros.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Educobros.Controllers
{
    [Authorize]
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
            if (!ModelState.IsValid)
            {
                CargarEstudiantes(pago.EstudianteId);
                return View(pago);
            }

            var estudiante = await _context.Estudiantes.FindAsync(pago.EstudianteId);

            if (estudiante == null)
            {
                ModelState.AddModelError("EstudianteId", "El estudiante seleccionado no existe.");
                CargarEstudiantes(pago.EstudianteId);
                return View(pago);
            }

            // Guardar el pago
            _context.Pagos.Add(pago);

            // Aplicar el pago a la deuda del estudiante
            if (estudiante.MesesDebidos > 0 && estudiante.Mensualidad > 0)
            {
                int mesesCubiertos = (int)(pago.Monto / estudiante.Mensualidad);

                if (mesesCubiertos > 0)
                {
                    estudiante.MesesDebidos -= mesesCubiertos;

                    if (estudiante.MesesDebidos < 0)
                    {
                        estudiante.MesesDebidos = 0;
                    }

                    _context.Estudiantes.Update(estudiante);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
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
