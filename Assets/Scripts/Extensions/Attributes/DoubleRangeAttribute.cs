using UnityEngine;

namespace Extensions
{
    public class DoubleRangeAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;

        public DoubleRangeAttribute(float min, float max)
        {
            Min = min; 
            Max = max;
        }
    }
}