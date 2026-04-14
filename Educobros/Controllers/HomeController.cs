using Educobros.Data;
using Educobros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
        // Intenta obtener del caché primero
        if (!_cache.TryGetValue("dashboard_stats", out DashboardVM stats))
        {
            // No está en caché → consultar BD
            stats = new DashboardVM
            {
                TotalEstudiantes = _context.Estudiantes.Count(),
                Recaudado = _context.Pagos.Sum(p => p.Monto),
                Pendientes = _context.Estudiantes.Count(e => e.MesesDebidos > 0)
            };

            // Guardar en caché por 5 minutos
            _cache.Set("dashboard_stats", stats, TimeSpan.FromMinutes(5));
        }

        return View(stats);
    }
}
