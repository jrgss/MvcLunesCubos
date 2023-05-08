using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcLunesCubos.Filters;
using MvcLunesCubos.Models;
using MvcLunesCubos.Services;

namespace MvcLunesCubos.Controllers
{
    public class UsuarioController : Controller
    {
        private ServiceApiCubos service;
        public UsuarioController(ServiceApiCubos service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(int idusuario,string nombre,string email,string pass, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.InsertarUsuario(idusuario, nombre, email, pass, file.FileName,stream, "containerprivado");

            }
           
            return RedirectToAction("Index", "Cubos");
        }
     
     
        [AuthorizeCubos]
        public async Task<IActionResult> Perfil()
        {
            string token =
             HttpContext.Session.GetString("TOKEN");
            Usuario user = await
                this.service.GetPerfilUser(token);
            return View(user);
        }
        [AuthorizeCubos]
        public async Task<IActionResult> VerPedidos()
        {
            string token =
           HttpContext.Session.GetString("TOKEN");
            Usuario user = await
                this.service.GetPerfilUser(token);
            List<CompraCubo> pedidos = await this.service.Getpedidos(token);

            return View(pedidos);
        }
        [AuthorizeCubos]
        [HttpGet]
        public async Task<IActionResult> Comprar(int idcubo)
        {
            {
                string token =
               HttpContext.Session.GetString("TOKEN");
                Usuario user = await
                    this.service.GetPerfilUser(token);
                await this.service.RealizarPedido(idcubo, token);
                return RedirectToAction("Index", "Cubos");
            }
        }
      
       
    }
}
