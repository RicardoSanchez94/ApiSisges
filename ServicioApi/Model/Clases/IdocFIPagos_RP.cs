using Swashbuckle.AspNetCore.Filters;

namespace ServicioApi.Model.Clases
{
    public class IdocFIPagos_RP
    {
        public DateTime Fecha { get; set; }
        public int local { get; set; }
        public string ambiente { get; set; }
    }

    public class IdocFIPagos_RPExample : IExamplesProvider<IdocFIPagos_RP>
    {
        public IdocFIPagos_RP GetExamples()
        {
            return new IdocFIPagos_RP
            {
                Fecha = new DateTime(2022, 5, 1),
                local = 2,
                ambiente = "SAPFSQ"
            };
        }
    }
}