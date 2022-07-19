using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RainbowColor.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RainbowColor.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly RainbowColorContext _rainbowColorContext;
        private readonly IHttpContextAccessor _accessor;

        public ClienteController(ILogger<ClienteController> logger,
            RainbowColorContext rainbowColorContext, 
            IHttpContextAccessor accessor)
        {
            _logger = logger;
            _rainbowColorContext = rainbowColorContext;
            _accessor = accessor;
        }

        //Cliente
        public IActionResult Index()
        {
            return View(_rainbowColorContext.Clientes.ToList());
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Cliente cliente)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                cliente.IdUsuario = idUsuario;
                _rainbowColorContext.Clientes.Add(cliente);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Cliente cadastrado com sucesso.";
            }
            catch(Exception ex)
            {
                TempData["Erro"] = "Problema ao cadastrar cliente, favor contatar o administrador!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var cliente = _rainbowColorContext.Clientes.Where(x => x.IdCliente == id).FirstOrDefault();
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Editar(Cliente cliente)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                cliente.IdUsuario = idUsuario;
                _rainbowColorContext.Clientes.Update(cliente);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Cliente editado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao editar cliente, favor contatar o administrador!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Deletar(int? id)
        {
            if (id == null || _rainbowColorContext.Clientes == null)
            {
                return NotFound();
            }

            var cliente = _rainbowColorContext.Clientes
                .FirstOrDefault(m => m.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public IActionResult DeletarConfirmado(int id)
        {
            try
            {
                //Busca o cliente pelo id
                var cliente = _rainbowColorContext.Clientes.Find(id);

                //Verifica se existe algum pedido para esse cliente para impedir a exclusão
                var existePedido = _rainbowColorContext.Pedidos.Any(x => x.ClienteId == id);

                if(existePedido)
                { 
                    TempData["Erro"] = "Não é possível deletar o cliente, já existem pedidos para o mesmo.";
                    return RedirectToAction("Deletar", new { id = id });
                }

                //Se encontrado, exclui o cliente
                if (cliente != null)
                {
                    _rainbowColorContext.Clientes.Remove(cliente);
                }

                //Salva a exclusão
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Cliente deletado com sucesso.";
            }
            catch(Exception ex)
            {
                TempData["Erro"] = "Problema ao deletado cliente, favor contatar o administrador!";
            }


            //Volta para a Index dessa controller
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}