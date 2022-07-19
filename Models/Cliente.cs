using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RainbowColor
{
    public partial class Cliente
    {
        public Cliente()
        {
            ClienteEnderecos = new HashSet<ClienteEndereco>();
            Pedidos = new HashSet<Pedido>();
        }

        public int IdCliente { get; set; }
        public string Nome { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        [DisplayName("CPF / CNPJ")]
        public string CpfCnpj { get; set; } = null!;
        public string IdUsuario { get; set; } = null!;

        public virtual AspNetUser IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<ClienteEndereco> ClienteEnderecos { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
