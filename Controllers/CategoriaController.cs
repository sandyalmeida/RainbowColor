using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RainbowColor.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RainbowColor.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ILogger<CategoriaController> _logger;
        private readonly RainbowColorContext _rainbowColorContext;
        private readonly IHttpContextAccessor _accessor;

        public CategoriaController(ILogger<CategoriaController> logger,
            RainbowColorContext rainbowColorContext,
            IHttpContextAccessor accessor)
        {
            _logger = logger;
            _rainbowColorContext = rainbowColorContext;
            _accessor = accessor;
        }

        public IActionResult Index()
        {
            return View(_rainbowColorContext.Categoria.ToList());
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Categoria Categoria)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Categoria.IdUsuario = idUsuario;
                _rainbowColorContext.Categoria.Add(Categoria);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Categoria cadastrada com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao cadastrar categoria, favor contatar o administrador!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var Categoria = _rainbowColorContext.Categoria.Where(x => x.Id == id).FirstOrDefault();
            return View(Categoria);
        }

        [HttpPost]
        public IActionResult Editar(Categoria Categoria)
        {
            try
            {
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Categoria.IdUsuario = idUsuario;
                _rainbowColorContext.Categoria.Update(Categoria);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Categoria editada com sucesso.";
            }
            catch(Exception ex)
            {
                TempData["Erro"] = "Problema ao editar categoria, favor contatar o administrador!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Deletar(int? id)
        {
            if (id == null || _rainbowColorContext.Categoria == null)
            {
                return NotFound();
            }

            var categoria = _rainbowColorContext.Categoria
                .FirstOrDefault(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }
        public IActionResult DeletarConfirmado(int id)
        {
            try
            {
                //Busca o categoria pelo id
                var categoria = _rainbowColorContext.Categoria.Find(id);

                //Verifica se existe algum produto cadastrado com essa categoria
                var existeProduto = _rainbowColorContext.Produtos.Any(x => x.IdCategoria == id);

                if (existeProduto)
                {
                    TempData["Erro"] = "Não é possível deletar a categoria, já existe produtos cadastrados com a mesma.";
                    return RedirectToAction("Deletar", new { id = id });
                }

                //Se encontrado, exclui o categoria
                if (categoria != null)
                {
                    _rainbowColorContext.Categoria.Remove(categoria);
                }

                //Salva a exclusão
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Categoria deletada com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao deletar categoria, favor contatar o administrador!";
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