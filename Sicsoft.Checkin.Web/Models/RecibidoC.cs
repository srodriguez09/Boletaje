using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class RecibidoC
    {
        public int idPadre { get; set; }
        public ProductosHijosViewModel[] ProductosHijos { get; set; }

    }
}
