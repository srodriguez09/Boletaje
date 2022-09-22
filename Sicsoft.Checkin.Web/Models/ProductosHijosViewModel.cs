using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class ProductosHijosViewModel
    {
        public int id { get; set; }
        public int idPadre { get; set; }
        public string codSAP { get; set; }
        public string Nombre { get; set; }
        public decimal Stock { get; set; }
        public int Cantidad { get; set; }
        public string Localizacion { get; set; }
        public decimal Costo { get; set; }
        public int PorMinimo { get; set; }
        public decimal Precio { get; set; }

    }
}
