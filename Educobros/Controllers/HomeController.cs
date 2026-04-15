using Educobros.Data;
using Educobros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace Educobros.Controllers
{
    public class HomeController : Controller
    {
        private readonly EduCobrosContext _context;
        private readonly IMemoryCache _cache;

        public HomeController(EduCobrosContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IActionResult Index()
        {
            if (!_cache.TryGetValue("dashboard_stats", out DashboardVM? stats))
            {
                stats = new DashboardVM
                {
                    TotalEstudiantes = _context.Estudiantes.Count(),
                    Pendientes = _context.Estudiantes.Count(e => e.MesesDebidos > 0),
                    Recaudado = _context.Pagos.Sum(p => (decimal?)p.Monto) ?? 0,
                    Mora = _context.Estudiantes
                        .Where(e => e.MesesDebidos > 0)
                        .Sum(e => (decimal?)(e.Mensualidad * e.MesesDebidos)) ?? 0,
                    UltimosPagos = _context.Pagos
                        .Include(p => p.Estudiante)
                        .OrderByDescending(p => p.Fecha)
                        .Take(5)
                        .ToList()
                };

                _cache.Set("dashboard_stats", stats, TimeSpan.FromMinutes(5));
            }

            return View(stats);
        }
    }
}