using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionNodeSO : ScriptableObject, IGameAction {
    [SerializeField] private List<(ERollResult,IGameAction)> children = new List<(ERollResult, IGameAction)>();
    [SerializeField] private IRollResolver rollResolver;
    [SerializeField] private ITargetingData targetingData;

    public List<ITargetable> SelectTargets(ActionContext context, Vector3Int target) { 
        return new List<ITargetable>();
    }

    public void Execute(ActionContext context, Vector3Int t) {
        bool isRollNeeded = rollResolver != null;
        var targets = SelectTargets(context, t);
        foreach (var target in targets) {
            foreach (var action in children) {
                if (isRollNeeded) {
                    var roll = rollResolver.ResolveRoll(context);
                    var ctx = context;
                    switch(roll.result) {
                        case ERollResult.CriticalFailure:
                            if (action.Item1 == ERollResult.CriticalFailure) {
                                action.Item2.Execute(ctx, target.Position);
                            }
                            break;
                        case ERollResult.Failure:
                            if (action.Item1 == ERollResult.Failure) {
                                action.Item2.Execute(ctx, target.Position);
                            }
                            break;
                        case ERollResult.Success:
                            if (action.Item1 == ERollResult.Success) {
                                action.Item2.Execute(ctx, target.Position);
                            }
                            break;
                        case ERollResult.CriticalSuccess:
                            if (action.Item1 == ERollResult.CriticalSuccess) {
                                action.Item2.Execute(ctx, target.Position);
                            }
                            break;
                    }
                }
                else {
                    action.Item2.Execute(context, target.Position);
                }
            }
        }
    }
}