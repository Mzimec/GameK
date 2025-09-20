using UnityEngine;

[CreateAssetMenu(fileName = "AttackRollDataSO", menuName = "Scriptable Objects/Rolls/AttackRollDataSO")]
public class AttackRollDataSO : ScriptableObject, IRollResolver {
    public RollRecord ResolveRoll(ActionContext ctx) {
        return new RollRecord
        {
            Roll = Random.Range(1, 21), // Simulate a d20 roll
            //TargetNumber = ctx.source.GetAttackTargetNumber(), // Assume CharacterCore has a method to get attack target number
            //Log = $"Rolled an attack: {ctx.source.name} attacks with target number {ctx.source.GetAttackTargetNumber()}"
        };
    }
}
