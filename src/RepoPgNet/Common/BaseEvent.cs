using MediatR;
namespace RepoPgNet;

public abstract class BaseEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
