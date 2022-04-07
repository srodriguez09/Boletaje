using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;


namespace Boletaje.Pages.TiposErrores
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<ErroresViewModel, int> service;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;

        [BindProperty]
        public DiagnosticosViewModel[] Diagnosticos { get; set; }

        [BindProperty]
        public ErroresViewModel Input { get; set; }

        public EditarModel(ICrudApi<ErroresViewModel, int> service, ICrudApi<DiagnosticosViewModel, int> serviceD)
        {
            this.service = service;
            this.serviceD = serviceD;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "30").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Diagnosticos = await serviceD.ObtenerLista("");
                Input = await service.ObtenerPorId(id);
                return Page();
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
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
