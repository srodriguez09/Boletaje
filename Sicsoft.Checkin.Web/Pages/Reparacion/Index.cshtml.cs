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

namespace Boletaje.Pages.Reparacion
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<EncReparacionViewModel, int> service;
        private readonly ICrudApi<TecnicosViewModel, int> serviceT;

        [BindProperty]
        public EncReparacionViewModel[] Objeto { get; set; }

        [BindProperty]
        public bool bandera { get; set; }

        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<EncReparacionViewModel, int> service, ICrudApi<TecnicosViewModel, int> serviceT)
        {
            this.service = service;
            this.serviceT = serviceT;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "21").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                if (!string.IsNullOrEmpty(Roles1.Where(a => a == "22").FirstOrDefault()))
                {
                    filtro.Codigo1 = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                    bandera = false;
                }
                else
                {
                    bandera = true;
                }
                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now;

                    filtro.FechaInicial = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime primerDia = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;



                }

                Objeto = await service.ObtenerLista(filtro);
                Tecnicos = await serviceT.ObtenerLista("");

                return Page();
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
