using UnityEngine;
using System;

namespace SliceVisualizer.Models
{
    public static class ScoreScalingModeExt
    {
        private static float ApplyScalingFunction(float distance, float min, float max, Func<float, float> transform)
        {
            var sign = Mathf.Sign(distance);
            distance = Mathf.Abs(distance);

            if (distance < min)
            { return 0.0f; }

            var tMin = transform(min);
            var tMax = transform(max);
            distance = (transform(distance) - tMin) / (tMax - tMin);

            return distance * sign;
        }

        public static float ApplyScaling(this ScoreScalingMode mode, float offset, float min, float max)
        {
            Func<float, float> transform = mode switch
            {
                ScoreScalingMode.Linear => (x => x),
                ScoreScalingMode.Log => Mathf.Log,
                ScoreScalingMode.Sqrt => Mathf.Sqrt,
                _ => (x => x),
            };

            offset = ApplyScalingFunction(offset, min, max, transform);

            return Mathf.Clamp(offset, -0.5f, 0.5f);
        }
    }

}