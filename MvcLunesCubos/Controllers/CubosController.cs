using Microsoft.AspNetCore.Mvc;
using MvcLunesCubos.Models;
using MvcLunesCubos.Services;

namespace MvcLunesCubos.Controllers
{
    public class CubosController : Controller
    {
        private ServiceApiCubos service;
        public CubosController(ServiceApiCubos service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this.service.GetCubosAsync();
            return View(cubos);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string marca)
        {
            List<Cubo> cubos = await this.service.GetCubosMarcaAsync(marca);
            return View(cubos);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(Cubo cubo)
        {
            await this.service.InsertarCubo(cubo.IdCubo, cubo.nombre, cubo.marca, cubo.imagen, cubo.precio);
            return RedirectToAction("Index");
        }

    }
}
