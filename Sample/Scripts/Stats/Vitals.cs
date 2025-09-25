
/// <summary>
/// Interface for managing vitals such as health.
/// </summary>
public interface IVitals {
    /// <summary>
    /// Gets the health resource stat.
    /// </summary>
    public IResourceStat<int> Health { get; }

    /// <summary>
    /// Applies damage to the entity, reducing its health.
    /// </summary>
    /// <param name="damage">Amount of the damage</param>
    public void TakeDamage(int damage);

    /// <summary>
    /// Applies healing to the entity, increasing its health.
    /// </summary>
    /// <param name="heal">Amount of the heal</param>
    public void TakeHeal(int heal);

    /// <summary>
    /// Checks if the entity is knocked out (health is zero or below).
    /// </summary>
    /// <returns>True if charcter is below zero hitpoints</returns>
    public bool IsKnockedOut();
}

/// <summary>
/// Base implementation of IVitals.
/// </summary>
public abstract class VitalsBase : IVitals {
    protected IResourceStat<int> _health;

    /// <inheritdoc/>
    public IResourceStat<int> Health => _health;

    /// <summary>
    /// Creates a new instance of VitalsBase with the specified health resource stat.
    /// </summary>
    /// <param name="health">Stat of the value int which represents health</param>
    public VitalsBase(IResourceStat<int> health) {
        this._health = health;
    }

    /// <inheritdoc/>
    public virtual void TakeDamage(int damage) => Health.ModifyCurrentValue(-damage);

    /// <inheritdoc/>
    public virtual void TakeHeal(int heal) => Health.ModifyCurrentValue(heal);

    /// <inheritdoc/>
    public virtual bool IsKnockedOut() => Health.Value <= 0;
}

/// <summary>
/// Implementation of character vitals, including health and regeneration.
/// </summary>
public class CharacterVitals : 
    VitalsBase, 
    IRegeneratable<int, IModifiableStat<int>>, 
    IComponent<CharacterCore> {

    private CharacterCore _owner;
    private IModifiableStat<int> _regeneration;

    /// <summary>
    /// Initializes a new instance of CharacterVitals with the specified health and regeneration stats.
    /// </summary>
    /// <param name="health">The health resource stat.</param>
    /// <param name="regeneration">The regeneration modifiable stat.</param>
    public CharacterVitals(IResourceStat<int> health, IModifiableStat<int> regeneration) : base(health) {
        this._regeneration = regeneration;
    }

    /// <inheritdoc/>
    public CharacterCore Owner => _owner;

    /// <summary>
    /// Gets the regeneration stat.
    /// </summary>
    public IModifiableStat<int> Regeneration => _regeneration;   
    
    /// <inheritdoc/>
    public void Regenerate() => Health.ModifyCurrentValue(_regeneration.Value);

}