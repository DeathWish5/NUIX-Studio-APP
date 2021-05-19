using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

#if OCULUSINTEGRATION_PRESENT
#endif
/// <summary>
/// One hand gesture basic class
/// </summary>
public abstract class BaseGestureWidget : ItemWidget
{
    public Handedness _handedness;

    [SerializeField] public bool isTrigger;
    [SerializeField] public SensorWidget _trigger;

    public BaseGestureWidget(Handedness handedness = Handedness.Right)
    {
        _handedness = handedness;
    }

    public abstract bool GestureCondition();

    public virtual void GestureEventUnTrigger()
    {
        isTrigger = false;
        _trigger.SensorUntrigger();
    }

    public virtual void GestureEventTrigger()
    {
        isTrigger = true;
        _trigger.SensorTrigger();
    }

    public void Update()
    {
        if (GestureCondition() && !isTrigger)
        {
            GestureEventTrigger();
        } 
        else
        {
            isTrigger = false;
            if(!GestureCondition())
                GestureEventUnTrigger();
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public override void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }

    public const float ANGLE_THREADHOLD = 0.866F;

    public bool ThumbAngleWithY(out float value)
    {
        if(HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, _handedness, out var palmPose) &&
            HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, _handedness, out var thumbTipPose))
        {
            Vector3 vec = thumbTipPose.Position - palmPose.Position;
            value = Vector3.Dot(vec.normalized, Vector3.up);
            return true;
        }
        value = 0F;
        return false;
    }

    public bool IndexAngleWithY(out float value)
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, _handedness, out var indexKuncklePose) &&
            HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, _handedness, out var indexTipPose))
        {
            Vector3 vec = indexTipPose.Position - indexKuncklePose.Position;
            value = Vector3.Dot(vec.normalized, Vector3.up);
            return true;
        }
        value = 0F;
        return false;
    }

    public bool MiddleAngleWithY(out float value)
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleKnuckle, _handedness, out var middleKnucklePose) &&
            HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, _handedness, out var middleTipPose))
        {
            Vector3 vec = middleTipPose.Position - middleKnucklePose.Position;
            value = Vector3.Dot(vec.normalized, Vector3.up);
            return true;
        }
        value = 0F;
        return false;
    }

    public bool IsFist()
    {
        return HandPoseUtils.IsThumbGrabbing(_handedness)
            //&& HandPoseUtils.IsRingFingerGrabbing(_handedness)
            //&& HandPoseUtils.IsPinkyFingerGrabbing(_handedness)
            && HandPoseUtils.IsMiddleGrabbing(_handedness)
            && HandPoseUtils.IsIndexGrabbing(_handedness);
    }

    public bool IsThumbUp()
    {
        return !HandPoseUtils.IsThumbGrabbing(_handedness)
            && HandPoseUtils.IsMiddleGrabbing(_handedness)
            && HandPoseUtils.IsIndexGrabbing(_handedness);
    }

    public bool IsIndexUp()
    {
        return HandPoseUtils.IsThumbGrabbing(_handedness) &&
            // && HandPoseUtils.IsMiddleGrabbing(_handedness)
            !HandPoseUtils.IsIndexGrabbing(_handedness);
    }

    public bool isThumbGrabbing()
    {
        return HandPoseUtils.IsThumbGrabbing(_handedness);
    }

    public bool isIndexGrabbing()
    {
        return HandPoseUtils.IsIndexGrabbing(_handedness);
    }

    public bool isMiddleGrabbing()
    {
        return HandPoseUtils.IsMiddleGrabbing(_handedness);
    }
}