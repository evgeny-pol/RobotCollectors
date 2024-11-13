using UnityEngine;

public static class MathUtils
{
    public const float CircleDegrees = 360;
    public const float QuarterCircleDegrees = CircleDegrees / 4;

    public static float SquaredDistanceTo(this Transform transform1, Transform transform2)
    {
        return (transform1.position - transform2.position).sqrMagnitude;
    }
}
