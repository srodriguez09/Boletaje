﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class ControlProductosViewModel
    {
        public int idProducto { get; set; }
        public string Item { get; set; }
        public string ItemName { get; set; }
        public int CantidadUsado { get; set; }
    }
}
