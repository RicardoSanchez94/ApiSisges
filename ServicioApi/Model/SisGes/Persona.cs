using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class Persona
    {
        public Guid Id { get; set; }
        public string? Run { get; set; }
        public int RunCuerpo { get; set; }
        public string RunDigito { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Nombres { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public int? SexoCodigo { get; set; }
        public string? Correo { get; set; }

        public virtual Usuario? Usuario { get; set; }
    }
}
