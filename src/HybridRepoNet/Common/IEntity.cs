namespace HybridRepoNet;
public interface IEntity
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

}
