using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Boletaje.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;
using Sicsoft.CostaRica.Checkin.Web.Models;

namespace Boletaje.Pages.ProductosPadres
{
    public class EditarModel : PageModel
    {
        private readonly ICrudApi<ProductosPadresViewModel, int> service;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<ColeccionProductosViewModel, int> service3;

        [BindProperty]
        public ProductosPadresViewModel Input { get; set; }

        [BindProperty]
        public ProductosHijosViewModel[] InputHijos { get; set; }
        [BindProperty]
        public ProductosHijosViewModel[] InputHijosAsignados { get; set; }


        public EditarModel(ICrudApi<ProductosPadresViewModel, int> service, ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<ColeccionProductosViewModel, int> service3)
        {
            this.service = service;
            this.service2 = service2;
            this.service3 = service3;

        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "20").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = id;

                Input = await service.ObtenerPorId(id);
                InputHijosAsignados = await service2.ObtenerLista(filt);
                InputHijos = await service2.ObtenerLista("");


                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            try
            {
                RecibidoC recibido = JsonConvert.DeserializeObject<RecibidoC>(recibidos);

                ColeccionProductosViewModel coleccion = new ColeccionProductosViewModel();
                coleccion.ProductosPadres = new ProductosPadresViewModel();
                coleccion.ProductosHijos = new ProductosHijosViewModel[recibido.ProductosHijos.Length];

                coleccion.ProductosPadres.id = recibido.idPadre;

                short cantidad = 1;
                foreach(var item in recibido.ProductosHijos)
                {
                    coleccion.ProductosHijos[cantidad - 1] = new ProductosHijosViewModel();
                    coleccion.ProductosHijos[cantidad - 1].id = item.id;
                    coleccion.ProductosHijos[cantidad - 1].Cantidad = item.Cantidad;
                    cantidad++;
                }

                await service3.Agregar(coleccion);

                var obj = new
                {
                    success = true,
                    mensaje = ""
                };

                return new JsonResult(obj);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                var obj = new
                {
                    success = false,
                    mensaje = "Error en el exception: -> " + ex.Message + " -> " + ex.StackTrace
                };
                return new JsonResult(obj);
            }
        }
    }
}
