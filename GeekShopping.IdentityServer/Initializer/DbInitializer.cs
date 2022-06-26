using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly MySQLContext _context;
        private readonly UserManager<ApplicationUser> _user; //gerencia o usuário criado
        private readonly RoleManager<ApplicationUser> _role; //manipula objetos no banco

        public DbInitializer(MySQLContext context, 
                             UserManager<ApplicationUser> user, 
                             RoleManager<ApplicationUser> role)
        {
            _context = context;
            _user = user;
            _role = role;
        }

        //obs: metodos asyncronos são usados para ter certeza que a aplicação não pule para o proximo passo
        //sem fazer o que é pedido
        public void Initialize()
        {
            if (_role.FindByNameAsync(IdentityConfiguration.Admin)
                .Result != null) return; //verifica se no banco há alguém com perfil de admin
                                         //se há ele sai do inicializador

            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

        }
    }
}
