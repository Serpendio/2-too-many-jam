using System;
using NaughtyAttributes;
using Random = UnityEngine.Random;

namespace Core
{
    [Serializable]
    public class PotentiallyRandomFloat
    {
        public bool Range;

        [AllowNesting] [HideIf("Range")] public float Value;

        [AllowNesting] [ShowIf("Range")] public float MinValue;
        [AllowNesting] [ShowIf("Range")] public float MaxValue;

        public float GetValue() => Range ? Random.Range(MinValue, MaxValue) : Value;
    }
}