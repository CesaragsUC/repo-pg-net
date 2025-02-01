using System.ComponentModel.DataAnnotations.Schema;

namespace HybridRepoNet;
public abstract class BaseEntity : IEntity
{
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }


    private readonly List<BaseEvent> _domainEvents = new();

    public Guid Id { get; set; }

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
