using Hangfire;
using Microsoft.Extensions.Logging;

namespace ReservaPeriferico.Web.Services;

public class HangfireTestService
{
    private readonly ILogger<HangfireTestService> _logger;

    public HangfireTestService(ILogger<HangfireTestService> logger)
    {
        _logger = logger;
    }

    public string EnqueueTestJob()
    {
        var jobId = BackgroundJob.Enqueue(() => TestJob());
        _logger.LogInformation("Job de teste enfileirado com ID: {JobId}", jobId);
        return jobId;
    }

    public string EnqueueDelayedTestJob(int delaySeconds = 30)
    {
        var jobId = BackgroundJob.Schedule(() => TestJob(), TimeSpan.FromSeconds(delaySeconds));
        _logger.LogInformation("Job de teste agendado com ID: {JobId} para {DelaySeconds} segundos", jobId, delaySeconds);
        return jobId;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 5, 10, 15 })]
    public void TestJob()
    {
        _logger.LogInformation("=== JOB DE TESTE EXECUTADO COM SUCESSO ===");
        _logger.LogInformation("Timestamp: {Timestamp}", DateTime.Now);
        _logger.LogInformation("Thread ID: {ThreadId}", Environment.CurrentManagedThreadId);
        
        // Simular algum trabalho
        Thread.Sleep(2000);
        
        _logger.LogInformation("=== JOB DE TESTE FINALIZADO ===");
    }
}




