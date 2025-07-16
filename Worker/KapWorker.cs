using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Worker.Jobs;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;


namespace Worker
{
    public class KapWorker : BackgroundService
    {
        private readonly KapJob _job;

        public KapWorker(KapJob job)
        {
            _job = job;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Worker başlatıldı");
            // Job her dakika çalışacak şekilde zamanlanıyor
            RecurringJob.AddOrUpdate<KapJob>(
                "fetch-kap",
                job => job.Run(),
                Cron.Minutely);

            return Task.CompletedTask;
        }
    }
}
