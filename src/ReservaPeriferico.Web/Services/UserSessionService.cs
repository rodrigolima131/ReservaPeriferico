using System.Security.Claims;

namespace ReservaPeriferico.Web.Services
{
    public class UserSessionService
    {
        private static int _instanceCount = 0;
        private string? _userName;
        private string? _userEmail;
        private bool _isAuthenticated;
        private DateTime _lastUpdate;
        
        public UserSessionService()
        {
        }

        public void SetUserInfo(string name, string email)
        {
            _userName = name;
            _userEmail = email;
            _isAuthenticated = true;
            _lastUpdate = DateTime.Now;
        }

        public void ClearUserInfo()
        {
            _userName = null;
            _userEmail = null;
            _isAuthenticated = false;
            _lastUpdate = DateTime.Now;
        }

        public string GetUserName()
        {
            return _userName ?? "Usuário do Sistema";
        }

        public string GetUserEmail()
        {
            return _userEmail ?? "Sistema de Reserva";
        }

        public bool IsAuthenticated()
        {
            return _isAuthenticated;
        }

        public DateTime GetLastUpdate()
        {
            return _lastUpdate;
        }
    }
} 