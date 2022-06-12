using System;
using System.Collections.Generic;
using TorqueGames.EditorUtils.Runtime;
using UnityEditor;
using UnityEngine;
using static TorqueGames.EditorUtils.Runtime.PropertiesComparer;
using static UnityEditor.AssetDatabase;
using Object = UnityEngine.Object;

namespace TorqueGames.EditorUtils
{
    public static class PropertyComparers
    {
        private static string Equal => PropertiesComparer.Equal;
        private static string Different => PropertiesComparer.Different;
        private static string Greater => PropertiesComparer.Greater;
        private static string GreaterOrEqual => PropertiesComparer.GreaterOrEqual;
        private static string Less => PropertiesComparer.Less;
        private static string LessOrEqual => PropertiesComparer.LessOrEqual;

        public delegate bool ValuesComparer(SerializedProperty source, object target);

        private static readonly Dictionary<string, ValuesComparer> Comparers = new()
        {
            {Equal, AreEqual},
            {Different, Not(AreEqual)},
            {Greater, IsGreater},
            {GreaterOrEqual, Or(IsGreater, AreEqual)},
            {Less, Not(Or(IsGreater, AreEqual))},
            {LessOrEqual, Not(IsGreater)},
        };

        private static ValuesComparer Or(ValuesComparer a, ValuesComparer b) =>
            (source, target) => a(source, target) || b(source, target);

        private static ValuesComparer Not(ValuesComparer a) => (source, target) => !a(source, target);

        private static bool IsGreater(SerializedProperty source, object target)
        {
            if (source.TryGetNumber(out var n1) && target.TryGetNumberObj(out var n2))
            {
                return n1 > n2;
            }

            return false;
        }

        private static bool AreEqual(SerializedProperty prop, object target)
        {
            if (prop.TryGetNumber(out var n1) && target.TryGetNumberObj(out var n2))
            {
                return Math.Abs(n1 - n2) < 1e-10;
            }

            var type = prop.propertyType;

            if (target is string s && type == SerializedPropertyType.String)
                return s == prop.stringValue;
            if (target is bool b && type == SerializedPropertyType.Boolean)
                return b == prop.boolValue;
            if (target is Vector2Int v2I && type == SerializedPropertyType.Vector2Int)
                return v2I == prop.vector2IntValue;
            if (target is Vector3Int v3I && type == SerializedPropertyType.Vector3Int)
                return v3I == prop.vector3IntValue;
            if (target is Vector3 v3 && type == SerializedPropertyType.Vector3)
                return Vector3.SqrMagnitude(v3 - prop.vector3Value) < 1E-20D;
            if (target is Vector2 v2 && type == SerializedPropertyType.Vector2)
                return Vector2.SqrMagnitude(v2 - prop.vector2Value) < 1E-20D;
            if (target is Color color && type == SerializedPropertyType.Color)
                return Vector4.SqrMagnitude((Vector4) color - (Vector4) prop.colorValue) < 1E-16D;


            if (ObjectByInstanceID(target, type, prop))
            {
                return true;
            }

            if (ObjectByGuid(target, type, prop))
            {
                return true;
            }


            return false;
        }

        private static bool ObjectByGuid(object target, SerializedPropertyType type, SerializedProperty prop)
        {
            return target is string guid
                   && type == SerializedPropertyType.ObjectReference
                   && guid.StartsWith("guid:") && prop.objectReferenceValue
                   && guid[5..] == AssetPathToGUID(GetAssetPath(prop.objectReferenceValue));
        }

        private static bool ObjectByInstanceID(object target, SerializedPropertyType type, SerializedProperty prop)
        {
            return target is string instanceId
                   && type == SerializedPropertyType.ObjectReference
                   && instanceId.StartsWith("instance:")
                   && prop.objectReferenceValue
                   && int.TryParse(instanceId[9..], out var instanceIdValue)
                   && prop.objectReferenceValue.GetInstanceID() == instanceIdValue;
        }

        private static bool TryGetNumberObj(this object prop, out double number)
        {
            switch (prop)
            {
                case long l:
                    number = l;
                    return true;
                case int i:
                    number = i;
                    return true;
                case double d:
                    number = d;
                    return true;
                case float f:
                    number = f;
                    return true;
                default:
                    number = default;
                    return false;
            }
        }

        public static bool TryGetNumber(this SerializedProperty prop, out double number)
        {
            var type = prop.propertyType;
            switch (type)
            {
                case SerializedPropertyType.Integer:
                    number = prop.longValue;
                    return true;
                case SerializedPropertyType.Float:
                    number = prop.doubleValue;
                    return true;
                case SerializedPropertyType.ArraySize:
                    number = prop.intValue;
                    return true;
                case SerializedPropertyType.Enum:
                    number = prop.enumValueIndex;
                    return true;
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Color:
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.Hash128:
                default:
                    number = default;
                    return false;
            }
        }


        public static bool TryGetValue(string comparisionKey, out ValuesComparer comparer)
        {
            return Comparers.TryGetValue(comparisionKey, out comparer);
        }
    }
}