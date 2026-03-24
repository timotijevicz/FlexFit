using FlexFit.Application.Commands;
using FlexFit.Domain.Models;
using FlexFit.Infrastructure.UnitOfWorkLayer;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlexFit.Application.Services
{
    public class ReservationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

        public ReservationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessNoShowsAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessNoShowsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var now = DateTime.UtcNow;
            
            var expiredReservations = await uow.Reservations.GetExpiredReservationsAsync(now);

            foreach (var reservation in expiredReservations)
            {
                try
                {
                    reservation.Status = "NoShow";
                    await uow.Reservations.UpdateAsync(reservation);
                    // No need for uow.SaveAsync() here as UpdateAsync should handle it for MongoDB
                    // and its internal SQL sync.

                    if (reservation.Id != null)
                    {
                        await mediator.Send(new ProcessNoShowPenaltyCommand(reservation.Id));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Greska pri obradi no-show rezervacije {reservation.Id}: {ex.Message}");
                }
            }
        }
    }
}
