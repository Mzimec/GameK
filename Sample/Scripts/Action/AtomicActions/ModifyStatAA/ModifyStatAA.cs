using System;
using UnityEngine;

public abstract class ModifyStatAASO<T> : AtomicActionSO {
    [SerializeField] private StatTypeSO stat;
    [SerializeField] private T value;
    [SerializeField] private Type modifierType;
    public override void Execute(ActionContext ctx, Vector3Int target) {
        // Implementation for modifying a stat
    }
}
