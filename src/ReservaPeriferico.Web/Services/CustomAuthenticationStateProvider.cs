using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ReservaPeriferico.Web.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                
                if (httpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    // Usar o usuário autenticado do HttpContext
                    var authState = new AuthenticationState(httpContext.User);
                    return Task.FromResult(authState);
                }
            }
            catch (Exception ex)
            {
                // Log de erro apenas em caso de exceção
                Console.WriteLine($"Erro no CustomAuthProvider: {ex.Message}");
            }

            // Retornar usuário não autenticado
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
} 