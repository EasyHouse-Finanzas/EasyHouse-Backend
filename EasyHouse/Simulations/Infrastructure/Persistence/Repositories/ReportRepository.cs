using EasyHouse.Shared.Infrastructure;
using EasyHouse.Simulations.Domain.Models.Entities;
using EasyHouse.Simulations.Domain.Models.Repository;

namespace EasyHouse.Simulations.Infrastructure.Persistence.Repositories;

public class ReportRepository : BaseRepository<Report>, IReportRepository
{
    public ReportRepository(AppDbContext context) : base(context) { }
}