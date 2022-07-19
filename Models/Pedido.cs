using System;
using System.Collections.Generic;

namespace RainbowColor
{
    public partial class Pedido
    {
        public Pedido()
        {
            PedidoProdutos = new HashSet<PedidoProduto>();
        }

        public int Id { get; set; }
        public int ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public int Quantidade { get; set; }
        public int? IdEndereco { get; set; }
        //Se true, então é carrinho. Se false, então é um pedido.
        public bool Carrinho { get; set; }
        public string IdUsuario { get; set; } = null!;

        public virtual Cliente Cliente { get; set; } = null!;
        public virtual ClienteEndereco ClienteEndereco { get; set; } = null!;
        public virtual AspNetUser IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<PedidoProduto> PedidoProdutos { get; set; }
    }
}
