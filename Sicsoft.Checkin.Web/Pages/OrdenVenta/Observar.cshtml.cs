using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;


namespace Boletaje.Pages.OrdenVenta
{
    public class ObservarModel : PageModel
    {
        private readonly ICrudApi<OrdenVentaViewModel, int> service;
        private readonly ICrudApi<ImpuestosViewModel, int> impuestos;
        private readonly ICrudApi<ClientesPOrdenesViewModel, int> clientes;
        private readonly ICrudApi<ProductosCOrdenesViewModel, int> prod;

        [BindProperty]
        public string Bodega { get; set; }
        [BindProperty]
        public OrdenVentaViewModel Orden { get; set; }

        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        [BindProperty]
        public ClientesPOrdenesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosCOrdenesViewModel Productos { get; set; }
        public ObservarModel(ICrudApi<OrdenVentaViewModel, int> service, ICrudApi<ImpuestosViewModel, int> impuestos, ICrudApi<ClientesPOrdenesViewModel, int> clientes, ICrudApi<ProductosCOrdenesViewModel, int> prod)
        {
            this.service = service;
            this.impuestos = impuestos;
            this.clientes = clientes;
            this.prod = prod;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "44").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Bodega = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Bodega").Select(s1 => s1.Value).FirstOrDefault();

                Clientes = await clientes.ObtenerListaEspecial("");
                Impuestos = await impuestos.ObtenerLista("");


                Orden = await service.ObtenerPorId(id);
                Orden.Cliente = Orden.CardCode + " - " + (Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault().CardName);


                      ParametrosFiltros filtro = new ParametrosFiltros();
                filtro.Texto = Orden.Detalle.FirstOrDefault().Bodega;
                filtro.ListPrice = Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault() == null ? "" : Clientes.Clientes.Where(a => a.CardCode == Orden.CardCode).FirstOrDefault().ListNum.ToString();
                Productos = await prod.ObtenerListaEspecial(filtro);
                    return Page();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
