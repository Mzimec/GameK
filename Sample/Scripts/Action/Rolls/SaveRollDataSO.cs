using UnityEngine;

[CreateAssetMenu(fileName = "SaveRollDataSO", menuName = "Scriptable Objects/Rolls/SaveRollDataSO")]
public class SaveRollDataSO : ScriptableObject, IRollResolver {
    [SerializeField] private int difficultyClass;
    public RollRecord ResolveRoll(ActionContext ctx) {
        return new RollRecord
        {
            Roll = Random.Range(1, 21), // Simulate a d20 roll
            TargetNumber = difficultyClass,
            Log = $"Rolled a save: {difficultyClass}"
        };
    }
}
