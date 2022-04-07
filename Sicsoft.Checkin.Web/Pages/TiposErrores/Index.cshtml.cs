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

namespace Boletaje.Pages.TiposErrores
{
    public class IndexModel : PageModel
    {
        private readonly ICrudApi<ErroresViewModel, int> service;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;

        [BindProperty]
        public ErroresViewModel[] Objeto { get; set; }

        [BindProperty]
        public DiagnosticosViewModel[] Diagnosticos { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<ErroresViewModel, int> service, ICrudApi<DiagnosticosViewModel, int> serviceD)
        {
            this.service = service;
            this.serviceD = serviceD;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "29").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }



                Objeto = await service.ObtenerLista(filtro);
                Diagnosticos = await serviceD.ObtenerLista("");

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
