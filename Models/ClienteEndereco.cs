using System;
using System.Collections.Generic;

namespace RainbowColor
{
    public partial class ClienteEndereco
    {
        public ClienteEndereco()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdEndereco { get; set; }

        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Endereco Endereco { get; set; } = null!;
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
