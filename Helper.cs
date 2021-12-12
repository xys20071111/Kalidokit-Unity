using UnityEngine;

namespace Kalidokit
{
    public class Helper
    {
        public static float Clamp(float val, float min, float max)
        {
            return Mathf.Max(Mathf.Min(val, max), min);
        }
        public static float Remap(float val, float min, float max)
        {
            return (Clamp(val, min, max) - min) / (max - min);
        }
    }
}
