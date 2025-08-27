using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ReservaPeriferico.Web.Services
{
    public class UserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserSessionService _userSessionService;

        public UserInfoService(IHttpContextAccessor httpContextAccessor, UserSessionService userSessionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userSessionService = userSessionService;
        }

        public string GetUserName()
        {
            try
            {
                // Primeiro tentar usar o UserSessionService (armazenamento global)
                if (_userSessionService?.IsAuthenticated() == true)
                {
                    var sessionName = _userSessionService.GetUserName();
                    return sessionName;
                }
                
                // Fallback para HttpContext (se ainda não estiver no session)
                var user = _httpContextAccessor.HttpContext?.User;
                
                if (user?.Identity?.IsAuthenticated == true)
                {
                    var name = user.FindFirst(ClaimTypes.Name)?.Value;
                    var givenName = user.FindFirst(ClaimTypes.GivenName)?.Value;
                    var surname = user.FindFirst(ClaimTypes.Surname)?.Value;
                    
                    if (!string.IsNullOrEmpty(name))
                        return name;
                    
                    if (!string.IsNullOrEmpty(givenName) || !string.IsNullOrEmpty(surname))
                        return $"{givenName} {surname}".Trim();
                }
                
                return "Usuário do Sistema";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no GetUserName: {ex.Message}");
                return "Usuário do Sistema";
            }
        }

        public string GetUserEmail()
        {
            try
            {
                // Primeiro tentar usar o UserSessionService (armazenamento global)
                if (_userSessionService?.IsAuthenticated() == true)
                {
                    var sessionEmail = _userSessionService.GetUserEmail();
                    return sessionEmail;
                }
                
                // Fallback para HttpContext (se ainda não estiver no session)
                var user = _httpContextAccessor.HttpContext?.User;
                
                if (user?.Identity?.IsAuthenticated == true)
                {
                    var email = user.FindFirst(ClaimTypes.Email)?.Value;
                    
                    if (!string.IsNullOrEmpty(email))
                        return email;
                }
                
                return "Sistema de Reserva";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no GetUserEmail: {ex.Message}");
                return "Sistema de Reserva";
            }
        }

        public bool IsAuthenticated()
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.Identity?.IsAuthenticated == true;
            }
            catch
            {
                return false;
            }
        }

        // Métodos assíncronos para compatibilidade (delegam para os síncronos)
        public async Task<string> GetUserNameAsync() => GetUserName();
        public async Task<string> GetUserEmailAsync() => GetUserEmail();
        public async Task<bool> IsAuthenticatedAsync() => IsAuthenticated();
    }
} 