using MvcLunesCubos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcLunesCubos.Services
{
    public class ServiceApiCubos
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;
        private ServiceStorageBlobs serviceBlob;
        public ServiceApiCubos(IConfiguration configuration,ServiceStorageBlobs service)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiCubos");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.serviceBlob = service;
        }
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token = jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return "ERROR " + response.StatusCode;
                }
            }
        }
        public async Task<List<Cubo>> GetCubosAsync()
        {
            string request = "/api/cubos";
            List<Cubo> cubos = await this.CallApiAsync<List<Cubo>>(request);
            foreach(Cubo c in cubos)
            {
                c.imagen=await serviceBlob.GetBlobUriAsync("containerpublico", c.imagen);
            }
            return cubos;
        } 
        public async Task<List<Cubo>> GetCubosMarcaAsync(string marca)
        {
            string request = "/api/cubos/getcubosmarca/"+marca;
            List<Cubo> cubos = await this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }
        public async Task InsertarCubo(int IdCubo,string nombre,string marca,string imagen,int precio)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/cubos";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                Cubo cubo = new Cubo
                {
                    IdCubo = IdCubo,
                    nombre = nombre,
                    imagen = imagen,
                    marca = marca,
                    precio = precio,
                };


                string json = JsonConvert.SerializeObject(cubo);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
           
        }  
        public async Task InsertarUsuario(int Idusuario,string nombre,string email,string pass,string imagen,Stream stream,string containername)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/usuarios";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                Usuario user = new Usuario
                {
                    IdUsuario = Idusuario,
                    Nombre = nombre,
                    Imagen = imagen,
                    Pass = pass,
                    Email = email,
                };


                string json = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
            await serviceBlob.UploadBlobAsync(containername, imagen, stream);
           
        }
        public async Task RealizarPedido(int idCubo, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/usuarios/realizarpedido";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                ModelPedidoPost model = new ModelPedidoPost
                {
                    idcubo = idCubo
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }


        }
        public async Task<Usuario> GetPerfilUser(string token)
        {
            string request = "/api/usuarios/Perfilusuario";
            Usuario user = await this.CallApiAsync<Usuario>(request, token);
            user.Imagen= await serviceBlob.GetBlobUriAsync("containerprivado", user.Imagen);
            return user;
        }
        public async Task<List<CompraCubo>>Getpedidos(string token)
        {
            string request = "/api/usuarios/Verpedidos";
            List<CompraCubo> pedidos = await this.CallApiAsync<List<CompraCubo>>(request, token);
            return pedidos;
        }

      
    }
}
