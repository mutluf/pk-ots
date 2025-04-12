using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ots.Api.Domain;
using Ots.Base;

namespace Ots.Api;

public class OtsDbContext : DbContext
{
    private readonly IAppSession appSession;

    public OtsDbContext(DbContextOptions<OtsDbContext> options, IAppSession appSession) : base(options)
    {
        this.appSession = appSession;
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entyList = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity
         && (e.State == EntityState.Deleted || e.State == EntityState.Added || e.State == EntityState.Modified));

        var auditLogs = new List<AuditLog>();

        foreach (var entry in entyList)
        {
            var baseEntity = (BaseEntity)entry.Entity;
            var properties = entry.Properties.ToList();
            var changedProperties = properties.Where(p => p.IsModified).ToList();
            var changedValues = changedProperties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
            var originalValues = properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
            var changedValuesString = JsonConvert.SerializeObject(changedValues.Select(kvp => new { Key = kvp.Key, Value = kvp.Value }));
            var originalValuesString = JsonConvert.SerializeObject(originalValues.Select(kvp => new { Key = kvp.Key, Value = kvp.Value }));


            var auditLog = new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = baseEntity.Id.ToString(),
                Action = entry.State.ToString(),
                Timestamp = DateTime.Now,
                UserName = appSession?.UserName ?? "anonymous",
                ChangedValues = changedValuesString,
                OriginalValues = originalValuesString,
            };

            if (entry.State == EntityState.Added)
            {
                baseEntity.InsertedDate = DateTime.Now;
                baseEntity.InsertedUser = appSession?.UserName ?? "anonymous";
                baseEntity.IsActive = true;
            }
            else if (entry.State == EntityState.Modified)
            {
                baseEntity.UpdatedDate = DateTime.Now;
                baseEntity.UpdatedUser = appSession?.UserName ?? "anonymous";
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                baseEntity.IsActive = false;
                baseEntity.UpdatedDate = DateTime.Now;
                baseEntity.UpdatedUser = appSession?.UserName ?? "anonymous";
            }

            auditLogs.Add(auditLog);
        }

        if (auditLogs.Any())
        {
            Set<AuditLog>().AddRange(auditLogs);
        }

        return base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OtsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
