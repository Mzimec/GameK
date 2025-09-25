using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect {
    void Apply(CharacterCore owner);
    void Remove(CharacterCore owner);
    bool IsValid { get; }
}


public class StatModifierEffect<T> : IEffect {
    private readonly EStatCategory _stat;
    private readonly IStatModifier<T> _modifier;

    public StatModifierEffect(EStatCategory stat, IStatModifier<T> modifier) {
        _stat = stat;
        _modifier = modifier;
    }

    public void Apply(CharacterCore owner) {
        if (owner.Stats.GetStat(_stat) is IModifiableStat<T> modifiableStat) {
            modifiableStat.AddModifier(_modifier);
        }
        else {
            Debug.LogWarning($"Stat {_stat} is not modifiable on {owner}");
        }
    }

    public void Remove(CharacterCore owner) {
        if (owner.Stats.GetStat(_stat) is IModifiableStat<T> modifiableStat) {
            modifiableStat.RemoveModifier(_modifier);
        }
    }

    public bool IsValid => true;
}


public class TriggeredActionEffect<T> : IEffect, IDisposable
    where T : IActionEvent {
    private readonly IGameAction _action;
    private IDisposable _subscription;

    public TriggeredActionEffect(IGameAction action) {
        _action = action;
    }

    public void Apply(CharacterCore owner) {
        _subscription = owner.EventBus.Subscribe<T>(evt => {
            var ctx = evt.ActionContext;
            _action.Execute(ctx);
        }
        );
    }

    public void Remove(CharacterCore owner) {
        Dispose();
    }

    public bool IsValid => true;

    public void Dispose() {
        _subscription?.Dispose();
    }
}

public class ActionEffect : IEffect {
    private readonly IGameAction _applyAction;
    private readonly IGameAction _removeAction;

    public ActionEffect(IGameAction applyAction, IGameAction removeAction = null) {
        _applyAction = applyAction;
        _removeAction = removeAction;
    }

    public void Apply(CharacterCore owner) {
        if (owner is ITargetable targetable) _applyAction.Execute(new ActionContext(owner, targetable, null, null));
    }

    public void Remove(CharacterCore owner) {
        if (owner is ITargetable targetable) _removeAction?.Execute(new ActionContext(owner, targetable, null, null));
    }

    public bool IsValid => true;
}
