using System.Collections.Generic;

public interface IStatProvider {

    public Dictionary<StatTypeSO, IStatBase> GetStats();
}