using System.Diagnostics.CodeAnalysis;
using Domain.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    [ExcludeFromCodeCoverage]

    public static class ContextExtenssions
    {
        public static void DetachLocal<T>(this DbContext context, T t, object entryId)
                where T : Entity
        {
            var local = context.Set<T>().Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));

            if (!(local is null))
            {
                context.Entry(local).State = EntityState.Detached;
            }
            context.Entry(t).State = EntityState.Modified;
        }
    }
}
