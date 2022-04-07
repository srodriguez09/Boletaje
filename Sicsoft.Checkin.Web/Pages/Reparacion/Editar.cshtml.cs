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

namespace Boletaje.Pages.Reparacion
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<DetReparacionViewModel, int> service;
        private readonly ICrudApi<LlamadasViewModel, int> serviceL;
        private readonly ICrudApi<ClientesViewModel, int> clientes;
        private readonly ICrudApi<ProductosViewModel, int> prods;
        private readonly ICrudApi<ProductosHijosViewModel, int> service2;
        private readonly ICrudApi<EncReparacionViewModel, int> serviceE;
        private readonly ICrudApi<ColeccionRepuestosViewModel, int> serviceColeccion;
        private readonly ICrudApi<BodegasViewModel, int> serviceBodegas;
        private readonly ICrudApi<BitacoraMovimientosViewModel, int> bt;
        private readonly ICrudApi<DiagnosticosViewModel, int> serviceD;
        private readonly ICrudApi<ErroresViewModel, int> serviceError;



        [BindProperty]
        public ErroresViewModel[] Errores { get; set; }
        [BindProperty]
        public DiagnosticosViewModel[] Diagnosticos { get; set; }
        [BindProperty]
        public string Cliente { get; set; }

        [BindProperty]
        public string Producto { get; set; }
        [BindProperty]
        public BodegasViewModel[] Bodegas { get; set; }

        [BindProperty]
        public BitacoraMovimientosViewModel[] BTS { get; set; }
        [BindProperty]
        public ProductosHijosViewModel[] InputHijos { get; set; }

        [BindProperty]
        public DetReparacionViewModel[] Input { get; set; }

        [BindProperty]
        public ClientesViewModel Clientes { get; set; }

        [BindProperty]
        public ProductosViewModel Productos { get; set; }

        [BindProperty]
        public EncReparacionViewModel Encabezado { get; set; }

        public EditarModel(ICrudApi<DetReparacionViewModel, int> service, ICrudApi<LlamadasViewModel, int> serviceL, ICrudApi<ClientesViewModel, int> clientes, ICrudApi<ProductosViewModel, int> prods,
           ICrudApi<ProductosHijosViewModel, int> service2, ICrudApi<EncReparacionViewModel, int> serviceE, ICrudApi<ColeccionRepuestosViewModel, int> serviceColeccion, ICrudApi<BodegasViewModel, int> serviceBodegas, ICrudApi<BitacoraMovimientosViewModel, int> bt, ICrudApi<DiagnosticosViewModel, int> serviceD
            ,ICrudApi<ErroresViewModel, int> serviceError)
        {
            this.service = service;
            this.serviceL = serviceL;
            this.clientes = clientes;
            this.prods = prods;
            this.service2 = service2;
            this.serviceE = serviceE;
            this.serviceColeccion = serviceColeccion;
            this.serviceBodegas = serviceBodegas;
            this.bt = bt;
            this.serviceD = serviceD;
            this.serviceError = serviceError;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "23").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                ParametrosFiltros filt = new ParametrosFiltros();
                filt.Codigo1 = id;

                BTS = await bt.ObtenerLista(filt);
                Bodegas = await serviceBodegas.ObtenerLista("");
                Input = await service.ObtenerLista(filt); //Se trae todos los hijos seleccionados
                Encabezado = await serviceE.ObtenerPorId(id);

                filt.Codigo1 = 0;
                filt.Texto = Encabezado.idProductoArreglar;
                InputHijos = await service2.ObtenerLista(filt); // Se trae todos los productos hijos dispobibles
               
                foreach(var item in Input)
                {
                    InputHijos = InputHijos.Where(a => a.id != item.idProducto).ToArray();
                }
                
                Productos = await prods.ObtenerListaEspecial("");
                Producto = Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemCode + " - " + Productos.Productos.Where(a => a.itemCode == Encabezado.idProductoArreglar).FirstOrDefault().itemName;
                Diagnosticos = await serviceD.ObtenerLista("");
                Errores = await serviceError.ObtenerLista("");


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
                RecibidoReparacion recibido = JsonConvert.DeserializeObject<RecibidoReparacion>(recibidos);

                ColeccionRepuestosViewModel coleccion = new ColeccionRepuestosViewModel();
                coleccion.EncReparacion = new EncReparacionViewModel();
                coleccion.DetReparacion = new DetReparacionViewModel[recibido.DetReparacion.Length];

                coleccion.EncReparacion.id = recibido.id;
                coleccion.EncReparacion.TipoReparacion = recibido.Tipo;
                coleccion.EncReparacion.Status = recibido.Status;
                coleccion.EncReparacion.Comentarios = recibido.comentarios;
                coleccion.EncReparacion.BodegaOrigen = recibido.BodegaInicial;
                coleccion.EncReparacion.BodegaFinal = recibido.BodegaFinal;
                coleccion.EncReparacion.idDiagnostico = recibido.idDiagnostico;

                short cantidad = 1;
                foreach (var item in recibido.DetReparacion)
                {
                    coleccion.DetReparacion[cantidad - 1] = new DetReparacionViewModel();
                    coleccion.DetReparacion[cantidad - 1].idEncabezado = recibido.id;
                    coleccion.DetReparacion[cantidad - 1].idProducto = item.idProducto;
                    coleccion.DetReparacion[cantidad - 1].ItemCode = item.ItemCode;
                    coleccion.DetReparacion[cantidad - 1].Cantidad = item.Cantidad;
                    coleccion.DetReparacion[cantidad - 1].idError = item.idError;

                    cantidad++;
                }

                await serviceColeccion.Agregar(coleccion);

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
