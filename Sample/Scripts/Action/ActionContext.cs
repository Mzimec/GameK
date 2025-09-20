using System.Collections.Generic;
using UnityEngine;

public interface ITargetable {
    Vector3Int Position { get; }
    GameObject GameObject { get; }
}

public struct ActionContext {
    public CharacterCore Source { get; set; }

    public int Cost { get; set; }
    public Dictionary<string, object> Data { get; set; }
}
