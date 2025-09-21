using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Infrastructure.Data;
using ReservaPeriferico.Infrastructure.Extensions;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using ReservaPeriferico.Web.Services;
using ReservaPeriferico.Application.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using ReservaPeriferico.Web.Services;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Application.Services;
using ReservaPeriferico.Core.Enums;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configurar Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<DatabaseEmailSettings>();

// Configurar serviços de infraestrutura
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configurar Hangfire
var hangfireConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(hangfireConnectionString, new PostgreSqlStorageOptions
    {
        QueuePollInterval = TimeSpan.FromSeconds(15),
        InvisibilityTimeout = TimeSpan.FromMinutes(30),
        UseNativeDatabaseTransactions = true,
        DistributedLockTimeout = TimeSpan.FromMinutes(10)
    }));

// Adicionar Hangfire Server
builder.Services.AddHangfireServer(options =>
{
    options.ServerTimeout = TimeSpan.FromMinutes(4);
    options.ServerCheckInterval = TimeSpan.FromSeconds(5);
    options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
});

// Registrar serviços de job
builder.Services.AddScoped<IEmailJobService, EmailJobService>();
builder.Services.AddScoped<HangfireTestService>();

// Configurar MudBlazor
builder.Services.AddMudServices();

// Configurar ThemeService
builder.Services.AddScoped<ThemeService>();

builder.Services.AddHttpContextAccessor();


// Adicionar serviços de autenticação e provedor de estado
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
    options.Cookie.Name = "ReservaPeriferico.Auth";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    // Usar User Secrets para as credenciais do Google OAuth
    var clientId = builder.Configuration["GoogleOAuth:ClientId"];
    var clientSecret = builder.Configuration["GoogleOAuth:ClientSecret"];

    // Debug: verificar se as credenciais estão sendo carregadas
    Console.WriteLine($"Google OAuth - ClientId: {clientId}");
    Console.WriteLine($"Google OAuth - ClientSecret: {clientSecret?.Substring(0, Math.Min(10, clientSecret?.Length ?? 0))}...");
    
    // Assegurar que as credenciais foram carregadas
    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
    {
        Console.WriteLine("ERRO: Credenciais do Google não configuradas no User Secrets!");
        Console.WriteLine("Execute: dotnet user-secrets set \"GoogleOAuth:ClientId\" \"SEU_CLIENT_ID\"");
        Console.WriteLine("Execute: dotnet user-secrets set \"GoogleOAuth:ClientSecret\" \"SEU_CLIENT_SECRET\"");
        throw new InvalidOperationException("Google OAuth credentials not configured in User Secrets");
    }
    
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.SaveTokens = true;
    
               // Usar o callback padrão do Google OAuth
           options.CallbackPath = "/signin-google";

    // Adicionar escopos para obter informações de perfil e e-mail
    options.Scope.Add("profile");
    options.Scope.Add("email");

               // Debug: verificar configuração
           Console.WriteLine($"Google OAuth configurado com CallbackPath: {options.CallbackPath}");
           Console.WriteLine($"Google OAuth configurado com ClientId: {options.ClientId}");
           Console.WriteLine($"Google OAuth configurado com SaveTokens: {options.SaveTokens}");
    
    // Configurar a validação do cookie de correlação, importante para o estado
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                      // Eventos básicos do Google OAuth (sem salvar usuário por enquanto)
       options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
       {
           OnRemoteFailure = context =>
           {
               Console.WriteLine($"Google OAuth Remote Failure: {context.Failure?.Message}");
               context.Response.Redirect("/login?error=remote_failure");
               context.HandleResponse();
               return Task.CompletedTask;
           },
           OnTicketReceived = context =>
           {
               // Extrair informações do usuário para salvar no UserSessionService
               var name = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
               var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
               
               // IMPORTANTE: Salvar no UserSessionService através do HttpContext
               if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
               {
                   // Obter o UserSessionService do HttpContext
                   var userSessionService = context.HttpContext.RequestServices.GetRequiredService<UserSessionService>();
                   userSessionService.SetUserInfo(name, email);
               }
               
               // Redirecionar direto para dashboard (o usuário já foi salvo na primeira vez)
               context.Response.Redirect("/dashboard");
               context.HandleResponse();
               
               return Task.CompletedTask;
           },
           OnCreatingTicket = context =>
           {
               Console.WriteLine("Google OAuth Creating Ticket...");
               return Task.CompletedTask;
           },
           OnAccessDenied = context =>
           {
               Console.WriteLine("Google OAuth Access Denied");
               return Task.CompletedTask;
           },
           OnRedirectToAuthorizationEndpoint = context =>
           {
               Console.WriteLine($"Redirecting to Google: {context.RedirectUri}");
               context.Response.Redirect(context.RedirectUri);
               return Task.CompletedTask;
           }
       };
});

// Configurar o provedor de autenticação para o Blazor
        // Usar o CustomAuthenticationStateProvider personalizado
        
        // Remover qualquer AuthenticationStateProvider padrão
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(AuthenticationStateProvider));
        if (descriptor != null)
        {
            builder.Services.Remove(descriptor);
        }
        
        // Adicionar o CustomAuthenticationStateProvider
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

               // Configurar serviços personalizados
        builder.Services.AddSingleton<UserSessionService>();  // MUDANÇA: Singleton em vez de Scoped
        
        builder.Services.AddScoped<UserInfoService>();
       
       var app = builder.Build();
       


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // O HSTS é importante para HTTPS em produção.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

       // Middleware de Autenticação e Autorização devem vir depois de UseRouting
       app.UseAuthentication();
       app.UseAuthorization();

       // Configurar Hangfire Dashboard (apenas em desenvolvimento)
       if (app.Environment.IsDevelopment())
       {
           app.UseHangfireDashboard("/hangfire", new DashboardOptions
           {
               Authorization = new[] { new HangfireAuthorizationFilter() },
               DashboardTitle = "ReservaPeriferico - Jobs",
               StatsPollingInterval = 2000
           });
       }
       
       // Middleware personalizado para salvar usuário Google automaticamente
       app.Use(async (context, next) =>
       {
           // Verificar se é uma requisição para dashboard e se o usuário está autenticado
           if (context.Request.Path.StartsWithSegments("/dashboard") && 
               context.User?.Identity?.IsAuthenticated == true)
           {
               try
               {
                   // Verificar se precisa salvar/atualizar usuário
                   var email = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                   if (!string.IsNullOrEmpty(email))
                   {
                       // Obter o serviço de usuário
                       var usuarioService = context.RequestServices.GetRequiredService<IUsuarioService>();
                       
                       // Verificar se o usuário já existe
                       var usuarios = await usuarioService.GetAllAsync();
                       var usuarioExistente = usuarios.FirstOrDefault(u => u.Email == email);
                       
                       if (usuarioExistente == null)
                       {
                           // Extrair informações do usuário
                           var name = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                           var givenName = context.User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
                           var surname = context.User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
                           
                           // Criar novo usuário
                           var novoUsuario = new ReservaPeriferico.Application.DTOs.UsuarioDto
                           {
                               Nome = name ?? $"{givenName} {surname}".Trim(),
                               Email = email,
                               Matricula = email.Split('@')[0],
                               Ativo = true,
                               DataCadastro = DateTime.Now
                           };
                           
                           var resultado = await usuarioService.CreateAsync(novoUsuario);
                           Console.WriteLine($"=== USUÁRIO CRIADO AUTOMATICAMENTE COM ID: {resultado.Id} ===");
                       }
                       else
                       {
                           // Atualizar usuário existente
                           var name = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                           var givenName = context.User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
                           var surname = context.User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
                           
                           usuarioExistente.Nome = name ?? $"{givenName} {surname}".Trim();
                           usuarioExistente.Ativo = true;
                           
                           var resultado = await usuarioService.UpdateAsync(usuarioExistente.Id, usuarioExistente);
                           Console.WriteLine($"=== USUÁRIO ATUALIZADO AUTOMATICAMENTE: {resultado.Id} ===");
                       }
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"Erro no middleware de usuário: {ex.Message}");
               }
           }
           
           await next();
       });

       // Mapear endpoints de autenticação
       app.MapGet("/auth/google", async (HttpContext context) =>
       {
           var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault() ?? "/dashboard";
           var properties = new AuthenticationProperties { RedirectUri = returnUrl };
           
           Console.WriteLine("=== AUTH/GOOGLE ENDPOINT CHAMADO ===");
           Console.WriteLine($"ReturnUrl: {returnUrl}");
           Console.WriteLine($"RedirectUri: {properties.RedirectUri}");
           Console.WriteLine("=== INICIANDO CHALLENGE ===");
           
           return Results.Challenge(properties, new[] { GoogleDefaults.AuthenticationScheme });
       });
       

       


       app.MapGet("/auth/logout", async (HttpContext context) =>
       {
           await context.SignOutAsync();
           return Results.Redirect("/login");
       });
       
       // Endpoint para login manual
       app.MapGet("/auth/login-manual", (HttpContext context, UserSessionService userSessionService) =>
       {
           // Salvar usuário admin no UserSessionService
           userSessionService.SetUserInfo("Administrador", "admin@teste.com");
           
           // Redirecionar para dashboard
           return Results.Redirect("/dashboard");
       });
       
       // Endpoints de teste para Hangfire
       app.MapGet("/test/hangfire/immediate", (HangfireTestService hangfireTest) =>
       {
           var jobId = hangfireTest.EnqueueTestJob();
           return Results.Ok(new { message = "Job de teste enfileirado", jobId });
       });
       
       app.MapGet("/test/hangfire/delayed/{delaySeconds:int}", (int delaySeconds, HangfireTestService hangfireTest) =>
       {
           var jobId = hangfireTest.EnqueueDelayedTestJob(delaySeconds);
           return Results.Ok(new { message = $"Job de teste agendado para {delaySeconds} segundos", jobId });
       });
       
       // Endpoints para teste de parâmetros
       app.MapGet("/test/parametros", async (IParametroService parametroService) =>
       {
           var parametros = await parametroService.GetAllParametersAsync();
           return Results.Ok(parametros);
       });
       
       app.MapGet("/test/parametros/email", async (IParametroService parametroService) =>
       {
           var smtpServer = await parametroService.GetParameterAsync(ParametroChave.EmailSmtpServer);
           var smtpPort = await parametroService.GetParameterAsync<int>(ParametroChave.EmailSmtpPort);
           var fromEmail = await parametroService.GetParameterAsync(ParametroChave.EmailFromEmail);
           
           return Results.Ok(new { smtpServer, smtpPort, fromEmail });
       });
       
       // Endpoint para salvar usuário após autenticação Google
       app.MapGet("/auth/save-user", async (HttpContext context, IUsuarioService usuarioService, UserSessionService userSessionService) =>
       {
           if (context.User?.Identity?.IsAuthenticated == true)
           {
               try
               {
                   // Extrair informações do usuário
                   var email = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                   var name = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                   var givenName = context.User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
                   var surname = context.User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
                   
                   if (!string.IsNullOrEmpty(email))
                   {
                       // Verificar se o usuário já existe
                       var usuarios = await usuarioService.GetAllAsync();
                       var usuarioExistente = usuarios.FirstOrDefault(u => u.Email == email);
                       
                       if (usuarioExistente == null)
                       {
                           // Criar novo usuário
                           var novoUsuario = new ReservaPeriferico.Application.DTOs.UsuarioDto
                           {
                               Nome = name ?? $"{givenName} {surname}".Trim(),
                               Email = email,
                               Matricula = email.Split('@')[0],
                               Ativo = true,
                               DataCadastro = DateTime.Now
                           };
                           
                           var resultado = await usuarioService.CreateAsync(novoUsuario);
                           
                           // SALVAR NO USER SESSION SERVICE (GLOBAL)
                           userSessionService.SetUserInfo(resultado.Nome, resultado.Email);
                       }
                       else
                       {
                           // Atualizar usuário existente
                           usuarioExistente.Nome = name ?? $"{givenName} {surname}".Trim();
                           usuarioExistente.Ativo = true;
                           
                           var resultado = await usuarioService.UpdateAsync(usuarioExistente.Id, usuarioExistente);
                           
                           // SALVAR NO USER SESSION SERVICE (GLOBAL)
                           userSessionService.SetUserInfo(resultado.Nome, resultado.Email);
                       }
                       
                       // Redirecionar para dashboard
                       return Results.Redirect("/dashboard");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"Erro ao salvar usuário: {ex.Message}");
               }
           }
           
           // Se não estiver autenticado, redirecionar para login
           return Results.Redirect("/login");
       });

       app.MapBlazorHub();
       app.MapFallbackToPage("/_Host");
       


// Garantir que o banco de dados esteja atualizado com as migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
