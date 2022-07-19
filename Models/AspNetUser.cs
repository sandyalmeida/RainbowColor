using System;
using System.Collections.Generic;

namespace RainbowColor
{
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            Categoria = new HashSet<Categoria>();
            Clientes = new HashSet<Cliente>();
            Enderecos = new HashSet<Endereco>();
            PedidoProdutos = new HashSet<PedidoProduto>();
            Pedidos = new HashSet<Pedido>();
            Produtos = new HashSet<Produto>();
            Roles = new HashSet<AspNetRole>();
        }

        public string Id { get; set; } = null!;
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<Categoria> Categoria { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Endereco> Enderecos { get; set; }
        public virtual ICollection<PedidoProduto> PedidoProdutos { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }

        public virtual ICollection<AspNetRole> Roles { get; set; }
    }
}
