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

namespace Boletaje.Pages.Bodega
{
    public class IndexModel : PageModel
    {
    //    private readonly ICrudApi<LlamadasViewModel, int> service;
      //  private readonly ICrudApi<EncReparacionViewModel, int> serviceEnc;
        private readonly ICrudApi<BitacoraMovimientosViewModel, int> serviceMov;
        private readonly ICrudApi<TecnicosViewModel, int> serviceT;


        [BindProperty]
        public BitacoraMovimientosViewModel[] Objeto { get; set; }
        [BindProperty]
        public TecnicosViewModel[] Tecnicos { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<BitacoraMovimientosViewModel, int> serviceMov,  ICrudApi<TecnicosViewModel, int> serviceT)
        {
            this.serviceMov = serviceMov;
            this.serviceT = serviceT;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "25").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now;

                    filtro.FechaInicial = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime primerDia = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;

                    filtro.Codigo3 = 0;

                }
             

                Objeto = await serviceMov.ObtenerLista(filtro);
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


        public async Task<IActionResult> OnGetEnviarSAP(string idB)
        {
            try
            {

                var Llamada = new BitacoraMovimientosViewModel();
                Llamada.id = Convert.ToInt32(idB);
                Llamada.Status = "1";

                var objetos = await serviceMov.Agregar(Llamada);




                return new JsonResult(objetos);
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());


                return new JsonResult(error);
            }
        }
    }
}
