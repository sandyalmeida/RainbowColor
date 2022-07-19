using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RainbowColor.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RainbowColor.Controllers
{
    [Authorize]
    public class ProdutoController : Controller
    {
        private readonly ILogger<ProdutoController> _logger;
        private readonly RainbowColorContext _rainbowColorContext;
        private readonly IHttpContextAccessor _accessor;

        public ProdutoController(ILogger<ProdutoController> logger,
            RainbowColorContext rainbowColorContext, 
            IHttpContextAccessor accessor)
        {
            _logger = logger;
            _rainbowColorContext = rainbowColorContext;
            _accessor = accessor;
        }

        public IActionResult Index()
        {
            return View(_rainbowColorContext.Produtos.Include(p=>p.Categoria).ToList());
        }

        [HttpGet]
        public IActionResult Criar()
        {
            //Obtem as Categorias, as converte em um objeto SelectListItem e coloca na ViewBag.Categorias
            ViewBag.Categorias = new SelectList(_rainbowColorContext.Categoria, "Id", "Descricao");
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Produto Produto)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Produto.IdUsuario = idUsuario;
                _rainbowColorContext.Produtos.Add(Produto);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Produto cadastrado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao cadastrar produto, favor contatar o administrador!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            ViewBag.Categorias = new SelectList(_rainbowColorContext.Categoria, "Id", "Descricao");
            var Produto = _rainbowColorContext.Produtos.Where(x => x.Id == id).FirstOrDefault();
            return View(Produto);
        }

        [HttpPost]
        public IActionResult Editar(Produto Produto)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Produto.IdUsuario = idUsuario;
                _rainbowColorContext.Produtos.Update(Produto);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Produto editado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao editar produto, favor contatar o administrador!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Deletar(int? id)
        {
            if (id == null || _rainbowColorContext.Produtos == null)
            {
                return NotFound();
            }

            var produto = _rainbowColorContext.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefault(m => m.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }
        public IActionResult DeletarConfirmado(int id)
        {
            try
            {
                //Busca o produto pelo id
                var produto = _rainbowColorContext.Produtos.Find(id);

                //Verifica se existe algum pedido para esse produto para impedir a exclusão
                var existePedido = _rainbowColorContext.PedidoProdutos.Any(x => x.ProdutoId == id);

                if (existePedido)
                {
                    TempData["Erro"] = "Não é possível deletar o produto, já existem pedidos com o mesmo.";
                    return RedirectToAction("Deletar", new { id = id });
                }

                //Se encontrado, exclui o produto
                if (produto != null)
                {
                    _rainbowColorContext.Produtos.Remove(produto);
                }

                //Salva a exclusão
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Produto deletado com sucesso.";
            }
            catch(Exception ex)
            {
                TempData["Erro"] = "Problema ao editar produto, favor contatar o administrador!";
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