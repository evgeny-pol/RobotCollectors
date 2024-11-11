using UnityEngine;

public static class AnimatorParams
{
    public static readonly int IsWalking = Animator.StringToHash(nameof(IsWalking));
    public static readonly int PickUpObject = Animator.StringToHash(nameof(PickUpObject));
    public static readonly int PlaceDownObject = Animator.StringToHash(nameof(PlaceDownObject));
}
