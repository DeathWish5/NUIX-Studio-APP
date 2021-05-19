using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
/// <summary>
/// Unique Gesture item of Thumbs up. Its value - the angle between the thumb and Y-axis.
/// </summary>

public class VolumeDownWidget : BaseGestureWidget
{
    public override bool GestureCondition()
    {
        return IsIndexUp() && IndexAngleWithY(out var value) && value < -ANGLE_THREADHOLD;
    }
}