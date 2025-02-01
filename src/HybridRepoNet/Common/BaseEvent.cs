using MediatR;
namespace HybridRepoNet;

public abstract class BaseEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
