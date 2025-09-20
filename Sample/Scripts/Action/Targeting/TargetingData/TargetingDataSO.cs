using System.Collections.Generic;
using UnityEngine;

public interface ITargetingData {
    public IEnumerable<ITargetable> SelectTargets(ActionContext ctx, Vector3Int target) {
        return new List<ITargetable>();
    }
}

public abstract class TargetingDataSO : ScriptableObject, ITargetingData {
    [SerializeField] private TargetTypeSO targetType;

    public abstract IEnumerable<ITargetable> SelectTargets(ActionContext context, Vector3Int target);

    private bool IsValidTarget(ITargetable entity, CharacterCore source) {

        return true;
    }
}