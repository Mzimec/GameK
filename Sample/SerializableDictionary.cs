using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableDictionary {

    /// <summary>
    /// A serializable dictionary that can be used in Unity.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] public List<TKey> keys = new List<TKey>();
        [SerializeField] public List<TValue> values = new List<TValue>();

        /// <summary>
        /// Called before serialization. Convert the dictionary into a list of keys and values.
        /// </summary>
        public void OnBeforeSerialize() { }

        /// <summary>
        /// Called after deserialization. Convert the list of keys and values back into a dictionary.
        /// </summary>
        public void OnAfterDeserialize() => TransferSerializedKeys();

        /// <summary>
        /// Transfers serialized keys and values into the dictionary.
        /// </summary>
        public void TransferSerializedKeys() {
            this.Clear();

            if (keys != null && values != null) {
                for (var i = 0; i < Math.Min(keys.Count, values.Count); i++) {
                    this[keys[i]] = values[i];
                }
            }

            else {
                keys = new List<TKey>();
                values = new List<TValue>();
            }

            if (keys.Count != values.Count) {
                keys = new List<TKey>(this.Keys);
                values = new List<TValue>(this.Values);
            }
        }
    }


    /// <summary>
    /// A wrapper for a list to make it serializable by Unity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ListWrapper<T> : List<T> { 
        public List<T> list = new List<T>();
    }

    /// <summary>
    /// A VisualElement that contains serializable dictionaries for use in UXML.
    /// </summary>
    [UxmlElement]
    public partial class SerializableDictionaryVisualElement : VisualElement {
        [UxmlAttribute]
        public SerializableDictionary<int, string> dictionaryIntString;

        [UxmlAttribute]
        public SerializableDictionary<int, int> dictionaryIntInt;

        [UxmlAttribute]
        public SerializableDictionary<string, string> dictionaryStringString;

        /*[UxmlAttribute]
        public SerializableDictionary<string, int> dictionaryStringInt;*/
    }
}

