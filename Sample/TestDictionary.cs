using SerializableDictionary;
using System.Collections.Generic;
using UnityEngine;

public class TestDictionary : MonoBehaviour
{

    public SerializableDictionary<Vector2Int, ListWrapper<EffectSO>> testDictionary = new SerializableDictionary<Vector2Int, ListWrapper<EffectSO>>();
    public List<ListWrapper<int>> lsit2D = new List<ListWrapper<int>>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
