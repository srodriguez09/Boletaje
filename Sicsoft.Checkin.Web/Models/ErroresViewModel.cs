using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class ErroresViewModel
    {
        public int id { get; set; }
        public int idDiagnostico { get; set; }
        public string Descripcion { get; set; }
        public string Diagnostico { get; set; }
    }
}
