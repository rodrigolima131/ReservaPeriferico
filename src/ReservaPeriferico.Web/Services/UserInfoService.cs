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
                
                return "Não autenticado";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no GetUserName: {ex.Message}");
                return "Não autenticado";
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
                
                return "Não autenticado";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no GetUserEmail: {ex.Message}");
                return "Não autenticado";
            }
        }

        public bool IsAuthenticated()
        {
            try
            {
                // Primeiro verificar UserSessionService (armazenamento global)
                if (_userSessionService?.IsAuthenticated() == true)
                {
                    return true;
                }
                
                // Fallback para HttpContext
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.Identity?.IsAuthenticated == true;
            }
            catch
            {
                return false;
            }
        }

        public int? GetUserId()
        {
            try
            {
                // ✅ Obter ID do usuário dos Claims
                var user = _httpContextAccessor.HttpContext?.User;
                
                if (user?.Identity?.IsAuthenticated == true)
                {
                    // Tentar obter o ID dos claims
                    var userIdClaim = user.FindFirst("UserId")?.Value ?? 
                                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    
                    if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                    {
                        return userId;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no GetUserId: {ex.Message}");
                return null;
            }
        }

        // Métodos assíncronos para compatibilidade (delegam para os síncronos)
        public async Task<string> GetUserNameAsync() => GetUserName();
        public async Task<string> GetUserEmailAsync() => GetUserEmail();
        public async Task<bool> IsAuthenticatedAsync() => IsAuthenticated();
        public async Task<int?> GetUserIdAsync() => GetUserId();
    }
} 