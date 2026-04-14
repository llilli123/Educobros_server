namespace Educobros.Models
{
    public class DashboardVM
    {
        public int TotalEstudiantes { get; set; }
        public int Pendientes { get; set; }
        public decimal Recaudado { get; set; }
        public decimal Mora { get; set; }
        public List<Pago> UltimosPagos { get; set; } = new();
    }
}
