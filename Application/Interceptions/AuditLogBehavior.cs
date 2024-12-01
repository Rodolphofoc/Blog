using Applications.Interceptions.Model;
using Applications.Interfaces.Repository;
using Domain.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Applications.Interceptions
{
    public class AuditLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuditLogBehavior(IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";

            if (request is IAuditLoggable loggable)
            {
                var log = new AuditLogEntity
                {
                    ActionType = loggable.ActionType,        
                    TableName = loggable.TableName,          
                    EntityId = loggable.EntityId,            
                    Changes = loggable.GetChanges(),         
                    User = user,
                    Timestamp = DateTime.UtcNow              
                };

                await _auditLogRepository.AddAsync(log);  

                await _unitOfWork.CompleteAsync();
            }

            return response;
        }
    }

}
