using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RainbowColor.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RainbowColor.Controllers
{
    [Authorize]
    public class EnderecosController : Controller
    {
        private readonly ILogger<EnderecosController> _logger;
        private readonly RainbowColorContext _rainbowColorContext;
        private readonly IHttpContextAccessor _accessor;

        public EnderecosController(ILogger<EnderecosController> logger,
            RainbowColorContext rainbowColorContext, 
            IHttpContextAccessor accessor)
        {
            _logger = logger;
            _rainbowColorContext = rainbowColorContext;
            _accessor = accessor;
        }

        //Enderecos
        public IActionResult Index(int idCliente)
        {
            //return View(_rainbowColorContext.ClienteEnderecos.Select(x => x.IdCliente == id).ToList());
            ViewData["NomeCliente"] = _rainbowColorContext.Clientes.Find(idCliente).Nome;
            ViewData["IdCliente"] = idCliente;
            return View(_rainbowColorContext.Enderecos.Where(x => x.ClienteEnderecos.Any(e => e.IdCliente == idCliente)).ToList());
        }

        [HttpGet]
        public IActionResult Criar(int idCliente)
        {
            ViewData["IdCliente"] = idCliente;
            ViewData["NomeCliente"] = _rainbowColorContext.Clientes.Find(idCliente).Nome;
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Endereco enderecos, int idCliente)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                enderecos.IdUsuario = idUsuario;

                //Grava o endereço
                _rainbowColorContext.Enderecos.Add(enderecos);
                _rainbowColorContext.SaveChanges();

                //Monta o ClienteEndereco
                ClienteEndereco clienteEndereco = new ClienteEndereco();
                clienteEndereco.IdCliente = idCliente;
                clienteEndereco.IdEndereco = enderecos.Id;

                //Grava o ClienteEndereco
                _rainbowColorContext.ClienteEnderecos.Add(clienteEndereco);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Endereço cadastrado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao criar endereço, favor contatar o administrador!";
            }

            return RedirectToAction("Index", new { idCliente = idCliente });
        }

        public IActionResult Editar(int id, int idCliente)
        {
            ViewData["IdCliente"] = idCliente;
            ViewData["NomeCliente"] = _rainbowColorContext.Clientes.Find(idCliente).Nome;
            var Enderecos = _rainbowColorContext.Enderecos.Where(x => x.Id == id).FirstOrDefault();
            return View(Enderecos);
        }

        [HttpPost]
        public IActionResult Editar(Endereco enderecos, int idCliente)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                enderecos.IdUsuario = idUsuario;
                _rainbowColorContext.Enderecos.Update(enderecos);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Endereço editado com sucesso.";
            }
            catch(Exception ex)
            {
                TempData["Erro"] = "Problema ao editar endereço, favor contatar o administrador!";
            }

            return RedirectToAction("Index", new { idCliente = idCliente });
        }

        public IActionResult Deletar(int? id, int idCliente)
        {
            ViewData["IdCliente"] = idCliente;
            if (id == null || _rainbowColorContext.Enderecos == null)
            {
                return NotFound();
            }

            var Enderecos = _rainbowColorContext.Enderecos
                .FirstOrDefault(m => m.Id == id);

            if (Enderecos == null)
            {
                return NotFound();
            }

            return View(Enderecos);
        }

        public IActionResult DeletarConfirmado(int id, int idCliente)
        {
            try
            {
                //Busca o Enderecos pelo id
                var endereco = _rainbowColorContext.Enderecos.Find(id);

                //Verifica se existe algum pedido para esse feito para esse endereço
                var existePedido = _rainbowColorContext.Pedidos.Any(x => x.IdEndereco == id);

                //Se existe, impedimos a deleção
                if (existePedido)
                {
                    TempData["Erro"] = "Não é possível deletar o endereço, já existem pedidos para o mesmo.";
                    return RedirectToAction("Deletar", new { id = id });
                }

                //Se encontrado, exclui o Enderecos
                if (endereco != null)
                {
                    //Primeiro exclui-se a tabela N pra N
                    var clienteEndereco = _rainbowColorContext.ClienteEnderecos.FirstOrDefault(x => x.IdCliente == idCliente && x.IdEndereco == id);

                    _rainbowColorContext.ClienteEnderecos.Remove(clienteEndereco);

                    _rainbowColorContext.Enderecos.Remove(endereco);

                    //Salva a exclusão
                    _rainbowColorContext.SaveChanges();

                    TempData["Sucesso"] = "Endereço deletado com sucesso.";
                }

            }
            catch(Exception ex)
            {
                TempData["Erro"] = "Problema ao deletar endereço, favor contatar o administrador!";
            }

            //Volta para a Index dessa controller
            return RedirectToAction("Index", new { idCliente = idCliente});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}