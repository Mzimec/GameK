using System.Globalization;
using System.Text;
using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;


namespace SerializableDictionary {
    public class SerializableDictionaryUxmlConverter<TKey, TValue> : UxmlAttributeConverter<SerializableDictionary<TKey, TValue>> {

        static string ValueToString(object v) => Convert.ToString(v, CultureInfo.InvariantCulture);

        public override string ToString(SerializableDictionary<TKey, TValue> source) {
            var sb = new StringBuilder();
            foreach (var pair in source) {
                sb.Append($"{ValueToString(pair.Key)}|{ValueToString(pair.Value)},");
            }
            return sb.ToString();
        }

        public override SerializableDictionary<TKey, TValue> FromString(string source) {
            var result = new SerializableDictionary<TKey, TValue>();
            var items = source.Split(',');
            foreach (var item in items) {
                var fields = item.Split('|');
                var key = (TKey)Convert.ChangeType(fields[0], typeof(TKey));
                var value = (TValue)Convert.ChangeType(fields[1], typeof(TValue));

                result.keys.Add(key);
                result.values.Add(value);
            }
            result.TransferSerializedKeys();
            return result;
        }
    }

    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer {
        SerializedProperty m_Property;
        SerializedProperty m_Keys;
        SerializedProperty m_Values;

        bool IsNotUniqueKey(SerializedProperty keys, int index) {
            for (int i = 0; i < m_Keys.arraySize; i++) {
                if (index == i) continue;
                if (m_Keys.GetArrayElementAtIndex(i).boxedValue.Equals(keys.boxedValue)) {
                    return true;
                }
            }
            return false;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            m_Property = property;
            m_Keys = property.FindPropertyRelative("keys");
            m_Values = property.FindPropertyRelative("values");

            Type keyType = fieldInfo.FieldType.GetGenericArguments()[0];
            Type valueType = fieldInfo.FieldType.GetGenericArguments()[1];

            var container = new Foldout()
            {
                text = property.displayName,
                viewDataKey = $"{property.serializedObject.targetObject.GetInstanceID()}.{property.name}",
            };


            var list = new ListView()
            {
                showAddRemoveFooter = true,
                showBorder = true,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showFoldoutHeader = true,
                showBoundCollectionSize = false,
                reorderable = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                headerTitle = $"Dictionary: {keyType.Name} | {valueType.Name}",
                bindingPath = m_Keys.propertyPath,
                overridingAddButtonBehavior = OnAddButton,
                bindItem = BindListItem,
                onRemove = OnRemove,
            };

            container.Add(list);

            var removeDuplicatesButton = new Button(OnRemoveDuplicates)
            {
                text = "Remove Duplicates",
                tooltip = "Remove duplicate keys from the dictionary.",
            };
            container.Add(removeDuplicatesButton);

            return container;
        }

        void BindListItem(VisualElement item, int index) {
            item.Clear();
            item.Unbind();

            var keyProperty = m_Keys.GetArrayElementAtIndex(index);

            item.Add(new PropertyField(keyProperty) { label = "Key" });
            item.Add(new PropertyField(m_Values.GetArrayElementAtIndex(index)) { label = "Value" });

            var duplicateWarningLabel = new Label("<b>WARNING: Duplicate key found!</b>");
            item.Add(duplicateWarningLabel);
            duplicateWarningLabel.visible = IsNotUniqueKey(keyProperty, index);

            item.TrackSerializedObjectValue(m_Property.serializedObject, (so) => {
                var latestKeyProp = m_Keys.GetArrayElementAtIndex(index);
                duplicateWarningLabel.visible = IsNotUniqueKey(latestKeyProp, index);
            });

            item.Bind(m_Property.serializedObject);
        }

        void OnAddButton(BaseListView baseListView, Button button) {
            m_Keys.InsertArrayElementAtIndex(m_Keys.arraySize);
            m_Values.InsertArrayElementAtIndex(m_Values.arraySize);
            m_Property.serializedObject.ApplyModifiedProperties();
        }

        void OnRemove(BaseListView listView) {
            if ((m_Keys.arraySize > 0) &&
                (listView.selectedIndex >= 0) &&
                (listView.selectedIndex < m_Keys.arraySize)) {
                int indexToRemove = listView.selectedIndex;

                m_Keys.DeleteArrayElementAtIndex(indexToRemove);
                m_Values.DeleteArrayElementAtIndex(indexToRemove);
                m_Property.serializedObject.ApplyModifiedProperties();
            }
        }

        void OnRemoveDuplicates() {
            List<int> indicesToRemove = new List<int>();

            for (int i = 0; i < m_Keys.arraySize; i++) {
                SerializedProperty firstKey = m_Keys.GetArrayElementAtIndex(i);

                for (int j = i + 1; j < m_Keys.arraySize; j++) {
                    SerializedProperty otherKey = m_Keys.GetArrayElementAtIndex(j);

                    if (firstKey.boxedValue.Equals(otherKey.boxedValue) &&
                        !indicesToRemove.Contains(j)) {
                        indicesToRemove.Add(j);
                    }
                }
            }

            for (int i = indicesToRemove.Count - 1; i >= 0; i--) {
                int indexToRemove = indicesToRemove[i];
                m_Keys.DeleteArrayElementAtIndex(indexToRemove);
                m_Values.DeleteArrayElementAtIndex(indexToRemove);
            }
            m_Property.serializedObject.ApplyModifiedProperties();
        }
    }
}
