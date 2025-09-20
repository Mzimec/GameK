using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableDictionary {


    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        // This class is a placeholder for a serializable dictionary.
        // In Unity, dictionaries are not serializable by default.
        // You can use this class to create a serializable dictionary.
        // However, you will need to implement the serialization logic yourself.
        // This is just a template to get you started.

        [SerializeField] public List<TKey> keys = new List<TKey>();
        [SerializeField] public List<TValue> values = new List<TValue>();

        // Called before serialization. Convert the dictionary to a list of keys and values.
        public void OnBeforeSerialize() { }

        // Called after deserialization. Convert the list of keys and values back into a dictionary.
        public void OnAfterDeserialize() => TransferSerializedKeys();

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

    [System.Serializable]
    public class ListWrapper<T> : List<T> { 
        public List<T> list = new List<T>();
    }

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

