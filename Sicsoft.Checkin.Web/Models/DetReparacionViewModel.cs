using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class DetReparacionViewModel
    {
        public int id { get; set; }
        public int idEncabezado { get; set; }
        public int idProducto { get; set; }
        public string ItemCode { get; set; }
        public int Cantidad { get; set; }
        public int idError { get; set; }
        public decimal Stock { get; set; }
    }
}
