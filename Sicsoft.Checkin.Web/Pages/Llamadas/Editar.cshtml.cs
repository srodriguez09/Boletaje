using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using Castle.Core.Configuration;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.Llamadas
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<LlamadasViewModel, int> service;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<GarantiasViewModel, int> garantias;
        private readonly ICrudApi<SucursalesViewModel, int> sucursales;
        private readonly ICrudApi<TecnicosViewModel, int> tecnicos;
        private readonly ICrudApi<StatusViewModel, int> status;
        private readonly ICrudApi<TiposCasosViewModel, int> tp;
        private readonly ICrudApi<AsuntosViewModel, int> asuntos;



        [BindProperty]
        public string Cliente { get; set; }

        [BindProperty]
        public string Producto { get; set; }


        [BindProperty]
        public LlamadasViewModel Input { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        [BindProperty]

        public GarantiasViewModel[] Garantias { get; set; }

        [BindProperty]

        public TiposCasosViewModel[] TP { get; set; }


        [BindProperty]

        public SucursalesViewModel[] Sucursales { get; set; }

        [BindProperty]

        public TecnicosViewModel[] Tecnicos { get; set; }
        [BindProperty]

        public StatusViewModel[] Status { get; set; }
        [BindProperty]
        public AsuntosViewModel[] Asuntos { get; set; }

        public EditarModel(ICrudApi<LlamadasViewModel, int> service, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods, ICrudApi<GarantiasViewModel, int> garantias,
            ICrudApi<SucursalesViewModel, int> sucursales, ICrudApi<TecnicosViewModel, int> tecnicos, ICrudApi<StatusViewModel, int> status, ICrudApi<TiposCasosViewModel, int> tp, ICrudApi<AsuntosViewModel, int> asuntos)
        {
            this.service = service;
            this.clientes = clientes;
            this.prods = prods;
            this.garantias = garantias;
            this.sucursales = sucursales;
            this.tecnicos = tecnicos;
            this.status = status;
            this.tp = tp;
            this.asuntos = asuntos;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "16").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Asuntos = await asuntos.ObtenerLista("");

                Input = await service.ObtenerPorId(id);
                Clientes = await clientes.ObtenerListaEspecial("");
                Cliente = Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault().CardCode + " - " + Clientes.Clientes.Where(a => a.CardCode == Input.CardCode).FirstOrDefault().CardName;
                 Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Input.ItemCode).FirstOrDefault().itemName;

                TP = await tp.ObtenerLista("");
                Garantias = await garantias.ObtenerLista("");
                Sucursales = await sucursales.ObtenerLista("");
                Tecnicos = await tecnicos.ObtenerLista("");
                Status = await status.ObtenerLista("");


                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Input.TratadoPor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
         
                await service.Editar(Input);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
