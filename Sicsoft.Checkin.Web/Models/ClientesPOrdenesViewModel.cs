namespace Boletaje.Models
{
    public class ClientesPOrdenesViewModel
    {

        public Clientes[] Clientes { get; set; }
    }
    public class Clientes
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int ListNum { get; set; }
    }
}
