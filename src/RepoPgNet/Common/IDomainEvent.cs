﻿namespace RepoPgNet;

// put in domain layer
public interface IDomainEvent
{
    Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents);
}