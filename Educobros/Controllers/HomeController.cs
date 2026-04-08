using Educobros.Models;
using Educobros.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Educobros.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EduCobrosContext _context;

        public HomeController(ILogger<HomeController> logger, EduCobrosContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Total de estudiantes
            ViewBag.TotalEstudiantes = await _context.Estudiantes.CountAsync();

            // Estudiantes con deuda
            ViewBag.Pendientes = await _context.Estudiantes
                .CountAsync(e => e.MesesDebidos > 0);

            // Total recaudado (suma de todos los pagos)
            ViewBag.Recaudado = await _context.Pagos
                .SumAsync(p => p.Monto);

            // Total en mora
            ViewBag.Mora = await _context.Estudiantes
                .Where(e => e.MesesDebidos > 0)
                .SumAsync(e => e.Mensualidad * e.MesesDebidos);

            // Últimos 5 pagos registrados
            ViewBag.UltimosPagos = await _context.Pagos
                .Include(p => p.Estudiante)
                .OrderByDescending(p => p.Fecha)
                .Take(5)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
