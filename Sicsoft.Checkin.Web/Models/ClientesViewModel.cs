﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class ClientesViewModel
    {
        
        public cliente[] Clientes { get; set; }
    }
    public class cliente
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Phone1 { get; set; }
    }
}
