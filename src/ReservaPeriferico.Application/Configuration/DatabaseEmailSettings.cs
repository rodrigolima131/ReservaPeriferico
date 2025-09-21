using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Application.Configuration;

public class DatabaseEmailSettings
{
    private readonly IParametroService _parametroService;

    public DatabaseEmailSettings(IParametroService parametroService)
    {
        _parametroService = parametroService;
    }

    public async Task<string> GetSmtpServerAsync()
    {
        return await _parametroService.GetParameterAsync(ParametroChave.EmailSmtpServer) ?? "smtp.gmail.com";
    }

    public async Task<int> GetSmtpPortAsync()
    {
        var port = await _parametroService.GetParameterAsync<int>(ParametroChave.EmailSmtpPort);
        return port > 0 ? port : 587;
    }

    public async Task<string> GetUsernameAsync()
    {
        return await _parametroService.GetParameterAsync(ParametroChave.EmailUsername) ?? string.Empty;
    }

    public async Task<string> GetPasswordAsync()
    {
        return await _parametroService.GetParameterAsync(ParametroChave.EmailPassword) ?? string.Empty;
    }

    public async Task<bool> GetEnableSslAsync()
    {
        var ssl = await _parametroService.GetParameterAsync<bool>(ParametroChave.EmailEnableSsl);
        return ssl;
    }

    public async Task<string> GetFromEmailAsync()
    {
        return await _parametroService.GetParameterAsync(ParametroChave.EmailFromEmail) ?? string.Empty;
    }

    public async Task<string> GetFromNameAsync()
    {
        return await _parametroService.GetParameterAsync(ParametroChave.EmailFromName) ?? "Sistema de Reservas";
    }

    public async Task<int> GetTimeoutAsync()
    {
        var timeout = await _parametroService.GetParameterAsync<int>(ParametroChave.EmailTimeout);
        return timeout > 0 ? timeout : 30000;
    }

    // Método para obter todas as configurações de uma vez
    public async Task<EmailSettings> GetEmailSettingsAsync()
    {
        return new EmailSettings
        {
            SmtpServer = await GetSmtpServerAsync(),
            SmtpPort = await GetSmtpPortAsync(),
            Username = await GetUsernameAsync(),
            Password = await GetPasswordAsync(),
            EnableSsl = await GetEnableSslAsync(),
            FromEmail = await GetFromEmailAsync(),
            FromName = await GetFromNameAsync(),
            Timeout = await GetTimeoutAsync()
        };
    }
}




