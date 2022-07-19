using System;
using System.Collections.Generic;

namespace RainbowColor
{
    public partial class Categoria
    {
        public Categoria()
        {
            Produtos = new HashSet<Produto>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; } = null!;
        public string IdUsuario { get; set; } = null!;

        public virtual AspNetUser IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
