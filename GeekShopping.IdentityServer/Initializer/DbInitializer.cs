using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _user; //gerencia o usuário criado
        private readonly RoleManager<IdentityRole> _role; //manipula objetos no banco

        public DbInitializer(ApplicationDbContext context, 
                             UserManager<ApplicationUser> user, 
                             RoleManager<IdentityRole> role)
        {
            _context = context;
            _user = user;
            _role = role;
        }

        //obs: metodos asyncronos são usados para ter certeza que a aplicação não pule para o proximo passo
        //sem fazer o que é pedido

        //metodo utilizado para popular banco criando um admin e um usuario
        public void Initialize()
        {
            if (_role.FindByNameAsync(IdentityConfiguration.Admin)
                .Result != null) return; //verifica se no banco há alguém com perfil de admin
                                         //se há ele sai do inicializador

            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

            // Cria admin
            ApplicationUser admin = new ApplicationUser()
            {
                UserName = "jose-admin",
                Email = "jose-admin@gmail.com.br",
                EmailConfirmed = true,
                PhoneNumber = "+55 (79) 12345-6789",
                FirstName = "José",
                LastName = "Admin"
            };

            _user.CreateAsync(admin, "Jose123$").GetAwaiter().GetResult();
            _user.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();
            var adminClaims = _user.AddClaimsAsync(admin, new Claim[]
            {
                
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName, admin.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)

            }).Result;           
            
            
            // Cria user
            ApplicationUser client = new ApplicationUser()
            {
                UserName = "jose-client",
                Email = "jose-client@gmail.com.br",
                EmailConfirmed = true,
                PhoneNumber = "+55 (79) 12345-6789",
                FirstName = "José",
                LastName = "client"
            };

            _user.CreateAsync(client, "Jose123$").GetAwaiter().GetResult();
            _user.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();
            var clientClaims = _user.AddClaimsAsync(client, new Claim[]
            {
                
                new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
                new Claim(JwtClaimTypes.GivenName, client.FirstName),
                new Claim(JwtClaimTypes.FamilyName, client.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)

            }).Result;

        }
    }
}
