using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RainbowColor
{
    public partial class PedidoViewModel
    {
        public PedidoViewModel()
        {
            PedidoProdutos = new HashSet<PedidoProduto>();
        }

        public int Id { get; set; }
        public int ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public int Quantidade { get; set; }
        public int? IdEndereco { get; set; }
        public bool Carrinho { get; set; }
        public string IdUsuario { get; set; } = null!;

        [NotMapped]
        public virtual Cliente Cliente { get; set; } = null!;
        [NotMapped]
        public virtual SelectList Clientes { get; set; } = null!;
        public virtual AspNetUser IdUsuarioNavigation { get; set; } = null!;
        [NotMapped]
        public virtual ICollection<PedidoProduto> PedidoProdutos { get; set; } = null!;
        [NotMapped]
        public virtual ClienteEndereco ClienteEndereco { get; set; } = null!;
        [NotMapped]
        public virtual ICollection<Produto> Produtos { get; set; } = null!;
    }
}
