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
    public class PedidoController : Controller
    {
        private readonly RainbowColorContext _rainbowColorContext;
        private readonly IHttpContextAccessor _accessor;

        public PedidoController(RainbowColorContext rainbowColorContext,
            IHttpContextAccessor accessor)
        {
            _rainbowColorContext = rainbowColorContext;
            _accessor = accessor;
        }

        //Tela principal do carrinho de compras
        [Route("carrinho")]
        public IActionResult IndexCarrinho(PedidoViewModel? pedidoVM)
        {
            //ViewBag.Clientes = new SelectList(_rainbowColorContext.Clientes, "IdCliente", "Nome");

            var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var idCliente = pedidoVM.ClienteId == 0 ? _rainbowColorContext.Clientes.FirstOrDefault().IdCliente : pedidoVM.ClienteId;

            if (pedidoVM.Id == 0)
            {
                var pedido = _rainbowColorContext.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.PedidoProdutos)
                    .ThenInclude(p => p.Produto)
                    .Where(x => x.IdUsuario == idUsuario && x.Carrinho && x.ClienteId == idCliente)
                    .FirstOrDefault();
                if(pedido != null)
                {
                    pedidoVM.Id = pedido.Id;
                    pedidoVM.Quantidade = pedido.Quantidade;
                    pedidoVM.ClienteId = pedido.ClienteId;
                    pedidoVM.Carrinho = pedido.Carrinho;
                    pedidoVM.IdUsuario = pedido.IdUsuario;
                    pedidoVM.ValorTotal = pedido.ValorTotal;
                    pedidoVM.Produtos = _rainbowColorContext.
                        Produtos.
                        Include(p => p.PedidoProdutos.Where(x => x.PedidoId == pedido.Id)).ToList();
                    pedidoVM.PedidoProdutos = pedido.PedidoProdutos;
                }
            }

            pedidoVM.Clientes = new SelectList(_rainbowColorContext.Clientes, "IdCliente", "Nome");
            return View(pedidoVM);
        }

        //Método que realiza a troca de cliente e retorna para tela de lista de produtos
        public IActionResult TrocarCliente(PedidoViewModel pedido)
        {
            return RedirectToAction("ListaProdutos", pedido);
        }

        //Método que realiza a troca de cliente e retorna para tela de lista de carrinho
        public IActionResult TrocarClienteCarrinho(PedidoViewModel pedido)
        {
            return RedirectToAction("IndexCarrinho", pedido);
        }

        //Tela que lista os produtos para serem adicionados ao carrinho
        public IActionResult ListaProdutos(PedidoViewModel? pedido)
        {
            PedidoViewModel pedidoViewModel = new PedidoViewModel();
            pedidoViewModel.ClienteId = pedido.ClienteId > 0 ? pedido.ClienteId : _rainbowColorContext.Clientes.FirstOrDefault().IdCliente;
            pedidoViewModel.Produtos = _rainbowColorContext.Produtos.ToList();

            ViewBag.Clientes = new SelectList(_rainbowColorContext.Clientes, "IdCliente", "Nome");

            return View(pedidoViewModel);
        }

        //Método que realiza a adição do produto no carrinho
        public IActionResult Adicionar(int idProduto, int idCliente, int Quantidade)
        {
            try
            {
                //Busca o usuário logado
                var idUsuario = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //Busca o produto para sabermos o preço unitário
                Produto produto = _rainbowColorContext.Produtos.Find(idProduto);

                //Checa se a quantidade solicitada é menor que o estoque do produto
                if(produto.Estoque < Quantidade)
                {
                    TempData["Erro"] = "A quantidade de itens solicitados é maior que o estoque do mesmo.";
                    return RedirectToAction("ListaProdutos");
                }

                //Checa se existe Carrinho criado para o cliente
                var pedido = _rainbowColorContext.Pedidos.Where(x => x.ClienteId == idCliente && x.Carrinho).FirstOrDefault();

                //se não existe, cria-se o carrinho
                if (pedido == null)
                {
                    pedido = new Pedido();
                    pedido.ClienteId = idCliente;
                    pedido.Carrinho = true;
                    pedido.IdUsuario = idUsuario;
                    _rainbowColorContext.Pedidos.Add(pedido);
                    _rainbowColorContext.SaveChanges();
                }

                //Adiciona o produto na PedidoProduto
                PedidoProduto pedidoProduto = new PedidoProduto();
                pedidoProduto.PedidoId = pedido.Id;
                pedidoProduto.ProdutoId = idProduto;
                pedidoProduto.Quantidade = Quantidade;
                pedidoProduto.Valor = produto.Valor * pedidoProduto.Quantidade;
                pedidoProduto.IdUsuario = idUsuario;

                _rainbowColorContext.PedidoProdutos.Add(pedidoProduto);
                _rainbowColorContext.SaveChanges();

                //Atualiza o Valor Total do Pedido
                pedido.ValorTotal = (decimal) _rainbowColorContext.
                    PedidoProdutos
                    .Where(x => x.PedidoId == pedido.Id)
                    .Sum(x => x.Valor);
                _rainbowColorContext.SaveChanges();
                TempData["Sucesso"] = "Produto adicionado ao carrinho com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao adicionar o produto ao carrinho, favor contatar o administrador!";
            }

            //Volta para a Index dessa controller
            return RedirectToAction("ListaProdutos");
        }

        //Método que realiza a troca de cliente e retorna para tela de lista de carrinho
        public IActionResult Remover(int idProdutoPedido)
        {
            try
            {
                //Busca o produto pelo id
                var produto = _rainbowColorContext.PedidoProdutos.Find(idProdutoPedido);

                //Se encontrado, exclui o produto do carrinho
                if (produto != null)
                {
                    _rainbowColorContext.PedidoProdutos.Remove(produto);
                }

                //Salva a exclusão
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Produto removido do carrinho com sucesso.";
            }
            catch(Exception ex)
            {
                TempData["Erro"] = "Problema ao remover produto, favor contatar o administrador!";
            }

            //Volta para a Index dessa controller
            return RedirectToAction("IndexCarrinho");
        }

        //Método que realiza a troca de endereço e retorna para tela de lista de realizar pedido
        public IActionResult TrocarEndereco(PedidoViewModel pedido)
        {
            return RedirectToAction("ConferirCarrinho", pedido);
        }

        //Carregamento da tela de ConferirCarrinho
        public IActionResult ConferirCarrinho(PedidoViewModel pedidoVM)
        {
            Pedido pedido = _rainbowColorContext.Pedidos.Include(p => p.Cliente).FirstOrDefault(p => p.Id == pedidoVM.Id);
            
            pedidoVM.Id = pedido.Id;
            pedidoVM.Carrinho = pedido.Carrinho;
            pedidoVM.ClienteId = pedido.ClienteId;
            pedidoVM.IdUsuario = pedido.IdUsuario;
            pedidoVM.ValorTotal = pedido.ValorTotal;
            pedidoVM.Produtos = _rainbowColorContext.
                        Produtos.
                        Include(p => p.PedidoProdutos.Where(x => x.PedidoId == pedido.Id)).ToList();
            pedidoVM.PedidoProdutos = pedido.PedidoProdutos;
            
            var enderecosCliente = _rainbowColorContext.Enderecos
                .Include(x => x.ClienteEnderecos)
                .Where(ce => ce.ClienteEnderecos.Any(x=>x.IdCliente == pedido.ClienteId))
                .ToList();

            if(enderecosCliente.Count() <= 0)
            {
                TempData["Erro"] = "Favor cadastrar ao menos um endereço para o cliente " + pedido.Cliente.Nome + " .";
                return RedirectToAction("IndexCarrinho");
            }

            pedidoVM.IdEndereco = enderecosCliente.FirstOrDefault().ClienteEnderecos.Select(x=>x.Id).FirstOrDefault();
            ViewBag.Enderecos = new SelectList(enderecosCliente
                .Select(s=> new 
                {   Id = s.Id,
                    Descricao = s.Logradouro + " nº " + s.Numero + ", Bairro: " + s.Bairro}), 
                "Id", "Descricao");

            return View(pedidoVM);
        }

        //Método que efetua o pedido, transformando o carrinho em pedido
        public IActionResult EfetuarPedido(int idPedido, int idEndereco)
        {
            try
            {
                var pedido = _rainbowColorContext.Pedidos.Find(idPedido);

                //diminui o estoque dos produtos da lista
                foreach(var pedidoProduto in pedido.PedidoProdutos)
                {
                    var produto = _rainbowColorContext.Produtos.Find(pedidoProduto.ProdutoId);

                    if(pedidoProduto.Quantidade > produto.Estoque)
                    {
                        TempData["Erro"] = "O estoque do produto " + produto.Nome + " não é suficiente para atender esse pedido!";
                        return RedirectToAction("IndexCarrinho");
                    }
                    else
                    {
                        //subtraindo a quantidade de itens do estoque
                        produto.Estoque = produto.Estoque - pedidoProduto.Quantidade;
                    }
                }

                //para que o carrinho vire um pedido, basta alteramos o campo Carrinho para false e adicionar o endereço
                pedido.Carrinho = false;
                pedido.IdEndereco = idEndereco;

                _rainbowColorContext.Pedidos.Update(pedido);
                _rainbowColorContext.SaveChanges();

                TempData["Sucesso"] = "Pedido realizado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Problema ao efetuar pedido, favor contatar o administrador!";
            }

            return RedirectToAction("IndexCarrinho");
        }

        //Carregamento da tela de Lista de pedidos
        public IActionResult ListaPedidos()
        {
            //busca os pedidos que NÃO são carrinho
            var pedidos = _rainbowColorContext
                .Pedidos
                //.Include(p=>p.PedidoProdutos)
                //.ThenInclude(p=>p.Produto)
                .Include(p=>p.Cliente)
                .ThenInclude(p=>p.ClienteEnderecos)
                .ThenInclude(p=>p.Endereco)
                .Where(p=>!p.Carrinho)
                .ToList();

            return View(pedidos);
        }

        //Carregamento da tela de detalhes do pedido
        public IActionResult VerPedido(int idPedido)
        {
            var pedido = _rainbowColorContext
                .Pedidos
                .Include(p => p.PedidoProdutos)
                .ThenInclude(p => p.Produto)
                .Include(p => p.Cliente)
                .ThenInclude(p => p.ClienteEnderecos)
                .ThenInclude(p => p.Endereco)
                .Where(p => !p.Carrinho)
                .FirstOrDefault(x=>x.Id == idPedido);

            PedidoViewModel pedidoVM = new PedidoViewModel();
            pedidoVM.Id = pedido.Id;
            pedidoVM.Cliente = pedido.Cliente;
            pedidoVM.ClienteEndereco = pedido.ClienteEndereco;
            pedidoVM.PedidoProdutos = pedido.PedidoProdutos;
            pedidoVM.ValorTotal = pedido.ValorTotal;

            return View(pedidoVM);
        }
    }
}
