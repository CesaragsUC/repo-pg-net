using MediatR;

namespace RepoPgNet;

//put this in Domain layer
public class DomainEvent : IDomainEvent
{
    private readonly IMediator _mediator;

    public DomainEvent(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();

            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent).ConfigureAwait(false);
            }
        }
    }
}
