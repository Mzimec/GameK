using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelfTargetData", menuName = "Action/Targeting Data/Self Target Data")]
public class SelfTargetDataSO : TargetingDataSO {
    public override IEnumerable<ITargetable> SelectTargets(ActionContext context, Vector3Int target) {
        var results = new List<ITargetable>();
        if (context.Source is ITargetable targetable) {
            results.Add(targetable);
        }
        return results;
    }
}