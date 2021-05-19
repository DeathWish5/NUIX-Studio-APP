using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Unique Gesture item of Thumbs up. Its value - the angle between the thumb and Y-axis.
/// </summary>

public class GestureDebug : BaseGestureWidget
{
    public TextMeshPro gestureLog;
    float thumbAngleWithY;
    float indexAngleWithY;
    float middleAngleWithY;
    bool isThumbUp;
    bool isIndexUp;
    bool isThumbGrabbing;
    bool isIndexGrabbing;
    bool isMiddleGrabbing;

    public void Update()
    {
        ThumbAngleWithY(out var thumb);
        IndexAngleWithY(out var index);
        MiddleAngleWithY(out var middle);

        thumbAngleWithY = thumb;
        indexAngleWithY = index;
        middleAngleWithY = middle;
        isThumbUp = IsThumbUp();
        isIndexUp = IsIndexUp();
        isThumbGrabbing = isThumbGrabbing();
        isMiddleGrabbing = isMiddleGrabbing();
        isIndexGrabbing = isIndexGrabbing();
        gestureLog.SetText(buildText());
    }

    string buildText()
    {
        return string.Format("thumbAngleWithY : {0:f2} \n indexAngleWithY : {1:f2} \n middleAngleWithY : {2:f2} \n" +
                             "isThumbUp : {3} \n isIndexUp: {4} \n thumbGrab: {5} \n indexGrab: {6} \n middleGrab: {7}",
                                thumbAngleWithY, indexAngleWithY, middleAngleWithY, isThumbUp.ToString(), isIndexUp.ToString(),
                                isThumbGrabbing.ToString(), isIndexGrabbing.ToString(), isMiddleGrabbing.ToString());
    }

    public override bool GestureCondition()
    {
        return false;
    }
}
