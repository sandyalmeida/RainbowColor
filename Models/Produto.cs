using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RainbowColor
{
    public partial class Produto
    {
        public Produto()
        {
            PedidoProdutos = new HashSet<PedidoProduto>();
        }

        public int Id { get; set; }
        [DisplayName("Categoria do Produto")]
        public int IdCategoria { get; set; }
        public string Nome { get; set; } = null!;
        public decimal Valor { get; set; }
        [DisplayName("Quantidade Disponível")]
        public int Estoque { get; set; }
        public string IdUsuario { get; set; } = null!;

        //propriedade quantidade para saber a quantidade de itens que será adicionada ao carrinho não mapeada no banco
        [NotMapped]
        public virtual int Quantidade { get; set; }
        public virtual Categoria Categoria { get; set; } = null!;
        public virtual AspNetUser AspNetUser { get; set; } = null!;
        public virtual ICollection<PedidoProduto> PedidoProdutos { get; set; }
    }
}
