using UnityEngine;

public static class MathUtils
{
    public const float CircleDegrees = 360;
    public const float QuarterCircleDegrees = CircleDegrees / 4;

    public static float SquaredDistanceTo(this Vector3 from, Vector3 to)
    {
        return (from - to).sqrMagnitude;
    }
}
