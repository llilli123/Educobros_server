using Educobros.Data;
using Educobros.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Educobros.Controllers
{
    [Authorize]
    public class EstudiantesController : Controller
    {
        private readonly EduCobrosContext _context;

        // DI: ASP.NET inyecta el contexto automáticamente
        public EstudiantesController(EduCobrosContext context)
        {
            _context = context;
        }

        // GET: /Estudiantes
        public async Task<IActionResult> Index(string filtro)
        {
            var query = _context.Estudiantes.AsQueryable();

            if (filtro == "mora")
                query = query.Where(e => e.MesesDebidos > 0);
            else if (filtro == "aldia")
                query = query.Where(e => e.MesesDebidos == 0);

            var estudiantes = await query.ToListAsync();

            ViewBag.Filtro = filtro;

            return View(estudiantes);
        }

        // GET: /Estudiantes/Create
        [Authorize(Roles = "Admin,Secretaria")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Estudiantes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> Create(Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estudiante);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(estudiante);
        }

        // GET: /Estudiantes/Edit/5
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> Edit(int id)
        {
            var est = await _context.Estudiantes.FindAsync(id);
            if (est == null) return NotFound();
            return View(est);
        }

        // POST: /Estudiantes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> Edit(int id, Estudiante estudiante)
        {
            if (id != estudiante.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(estudiante);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(estudiante);
        }

        // POST: /Estudiantes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var est = await _context.Estudiantes.FindAsync(id);
            if (est != null)
            {
                _context.Estudiantes.Remove(est);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}