using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
/// <summary>
/// Unique Gesture item of Thumbs up. Its value - the angle between the thumb and Y-axis.
/// </summary>
public class PlayPauseWidget : BaseGestureWidget
{
    public override bool GestureCondition()
    {
        return IsFist();
    }
}
