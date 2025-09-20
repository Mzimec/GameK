public interface IVitals {
    public IResourceStat<int> Health { get; }
    public void TakeDamage(int damage);
    public void TakeHeal(int heal);
    public bool IsKnockedOut();
}

public abstract class VitalsBase : IVitals {
    protected IResourceStat<int> _health;
    public IResourceStat<int> Health => _health;

    public VitalsBase(IResourceStat<int> health) {
        this._health = health;
    }

    public virtual void TakeDamage(int damage) => Health.ModifyCurrentValue(-damage);
    public virtual void TakeHeal(int heal) => Health.ModifyCurrentValue(heal);

    public virtual bool IsKnockedOut() => Health.Value <= 0;
}

public class CharacterVitals : 
    VitalsBase, 
    IRegeneratable<int, IModifiableStat<int>>, 
    IComponent<CharacterCore> {

    private CharacterCore _owner;
    private IModifiableStat<int> _regeneration;

    public CharacterVitals(IResourceStat<int> health, IModifiableStat<int> regeneration) : base(health) {
        this._regeneration = regeneration;
    }
    public CharacterCore Owner => _owner;
    public IModifiableStat<int> Regeneration => _regeneration;    
    public void Regenerate() => Health.ModifyCurrentValue(_regeneration.Value);

}