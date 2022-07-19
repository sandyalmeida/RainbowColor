using System;
using System.Collections.Generic;

namespace RainbowColor
{
    public partial class PedidoProduto
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string IdUsuario { get; set; } = null!;

        public virtual AspNetUser IdUsuarioNavigation { get; set; } = null!;
        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Produto Produto { get; set; } = null!;
    }
}
