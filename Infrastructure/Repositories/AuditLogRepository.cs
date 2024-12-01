using Applications.Interfaces.Repository;
using Domain.Domain;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class AuditLogRepository : Repository<AuditLogEntity>, IAuditLogRepository
    {
        private readonly BlogContext _context;

        public AuditLogRepository(BlogContext context) : base(context)
        {
            _context = context;
        }
    }
}
