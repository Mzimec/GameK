/// <summary>
/// Represents a generic component that belongs to an owner entity of type <typeparamref name="TOwner"/>.
/// </summary>
/// <typeparam name="TOwner">The type of the owner entity this component belongs to.</typeparam>
public interface IComponent<TOwner> {
    public TOwner Owner { get; }
}