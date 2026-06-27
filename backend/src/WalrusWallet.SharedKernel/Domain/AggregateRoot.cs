namespace WalrusWallet.SharedKernel.Domain;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : struct, IEquatable<TId>, IStronglyTypedId<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent @event) => _domainEvents.Add(@event);
    public void ClearDomainEvents() => _domainEvents.Clear();
}