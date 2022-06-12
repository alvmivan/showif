using TorqueGames.EditorUtils.Runtime;
using UnityEditor;
using UnityEngine;

namespace TorqueGames.EditorUtils
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        private bool ShouldShow(SerializedProperty property)
        {
            if (attribute is not ShowIfAttribute showIfAttribute) return true;

            var otherProp = property.serializedObject.FindProperty(showIfAttribute.Condition);

            if (otherProp == null) return true;

            if (otherProp.propertyType is SerializedPropertyType.Boolean)
            {
                return showIfAttribute.Invert ^ otherProp.boolValue;
            }

            var comparer = showIfAttribute.Comparision;

            return !PropertyComparers.TryGetValue(comparer, out var comparerFunction) ||
                   comparerFunction(otherProp, showIfAttribute.OtherValue);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(property) ? EditorGUIUtility.singleLineHeight : 0;
        }
    }
}