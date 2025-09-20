using System;
using UnityEngine;

public interface IGameAction {
    public void Execute(ActionContext ctx, Vector3Int target);
}

public abstract class AtomicActionSO : ScriptableObject, IGameAction {
    public abstract void Execute(ActionContext ctx, Vector3Int target);
}
