using System;
using System.Collections.Generic;

namespace RainbowColor
{
    public partial class Endereco
    {
        public Endereco()
        {
            ClienteEnderecos = new HashSet<ClienteEndereco>();
        }

        public int Id { get; set; }
        public string Logradouro { get; set; } = null!;
        public int Numero { get; set; }
        public string Bairro { get; set; } = null!;
        public string? Complemento { get; set; }
        public string IdUsuario { get; set; } = null!;

        public virtual AspNetUser IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<ClienteEndereco> ClienteEnderecos { get; set; }
    }
}
