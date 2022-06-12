using UnityEngine;

namespace TorqueGames.EditorUtils.Runtime
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string Condition;
        public readonly bool Invert;
        
        public readonly string Comparision;
        public readonly object OtherValue;

        //only bool values
        public ShowIfAttribute(string condition, bool invert = false)
        {
            Invert = invert;
            Condition = condition;
        }
        
        public ShowIfAttribute(string condition, string comparision, object otherValue)
        {
            Condition = condition;
            Comparision = comparision;
            OtherValue = otherValue;
        }
    }
}