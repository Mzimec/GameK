using System.Collections.Generic;
using UnityEngine;

public class GameActionSO : ScriptableObject, IGameAction {

    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private List<IGameAction> _atomicActions;

    [SerializeField] private int _baseCost;
    public int BaseCost => _baseCost;

    private int GetCost(ActionContext context) {
        // Implement cost calculation logic based on context if needed
        return _baseCost;
    }

    private bool CanExecute(ActionContext context) {
        // Implement logic to determine if the action can be executed in the given context
        return true;
    }

    public void Execute(ActionContext context, Vector3Int target) {
        if (!CanExecute(context)) return;
        foreach (var action in _atomicActions) {
            action.Execute(context, target);
        }
    }
}
