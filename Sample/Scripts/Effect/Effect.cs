using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect {
    public void ApplyEffect(IEnumerable<IGameAction> actions);
    public void RemoveEffect(IEnumerable<IGameAction> actions);
    public bool IsValid();
}

public class EffectTemplateSO : ScriptableObject {
    // This class can be expanded to include properties and methods
    // that define the template for creating Effect instances.
}

public class Effect : IEffect {

    public Effect(EffectTemplateSO template) { }
    public virtual void ApplyEffect(IEnumerable<IGameAction> actions) {
        // Default implementation does nothing
    }
    public virtual void RemoveEffect(IEnumerable<IGameAction> actions) {
        // Default implementation does nothing
    }
    public virtual bool IsValid() {
        return true; // Default implementation always returns true
    }


}
