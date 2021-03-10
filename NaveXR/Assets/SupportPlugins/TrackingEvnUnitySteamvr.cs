#if NAVEVR_STEAMVR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Nave.VR;
using U3DInputDevices = UnityEngine.XR.InputDevices;
using NaveInputDevices = Nave.VR.InputDevices;
using Valve.VR;

[XREnv(name = "UnitySteamvr", lib = XRLib.OpenVR)]
public class TrackingEvnUnitySteamvr : Nave.VR.TrackingEvnBase
{
    protected override IEnumerator InitEvnAsync(System.Action<string> onResult)
    {
        SteamVR.Initialize(true);
        yield return null;
        while (SteamVR.initializedState == SteamVR.InitializedStates.Initializing) yield return 0;

        if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
        {
            onResult?.Invoke(string.Empty);
            var actionSet = SteamVR_Input.GetActionSet("key");
            actionSet?.Activate(SteamVR_Input_Sources.LeftHand);
            actionSet?.Activate(SteamVR_Input_Sources.RightHand);
        }
        else
        {
            onResult?.Invoke("UnitySteamvrEvn初始化失败");
        }
        yield return null;
    }

    public override void Release()
    {
        base.Release();
        SteamVR.SafeDispose();
    }

    protected override void FillMetadata(HandAnchor anchor, ref XRNodeState xRNode)
    {
        //base.UpdateHandNodeState(hand, ref xRNode);
        float thumb = 0f, index = 0f, middle = 0f;

        //psotion && rotation
        xRNode.TryGetPosition(out anchor.position);
        xRNode.TryGetRotation(out anchor.rotation);

        var inputSource = SteamVR_Input_Sources.LeftHand;
        string skeletonHandName = "SkeletonLeftHand";
        if (anchor.type == NodeType.RightHand)
        {
            inputSource = SteamVR_Input_Sources.RightHand;
            skeletonHandName = "skeletonRightHand";
        }

        //SteamVR_Action_Boolean key_MenuKey = SteamVR_Input.GetBooleanAction("MenuKey");
        //SteamVR_Action_Boolean key_SystemKey = SteamVR_Input.GetBooleanAction("SystemKey");

        //trigger
        SteamVR_Action_Boolean triggerPressed = SteamVR_Input.GetBooleanAction("Trigger");
        //SteamVR_Action_Single triggerTouchValue = SteamVR_Input.GetSingleAction("triggerTouchValue");
        anchor.triggerPressed = triggerPressed.GetState(inputSource);
        anchor.triggerTouchValue = Mathf.Lerp(anchor.triggerTouchValue, anchor.triggerPressed ? 1f : 0f, Time.deltaTime * 5);
        index = anchor.triggerTouchValue;

        //grip
        SteamVR_Action_Boolean gripPressed = SteamVR_Input.GetBooleanAction("SideTrigger");
        //SteamVR_Action_Single gripTouchValue = SteamVR_Input.GetSingleAction("gripTouchValue");
        anchor.gripPressed = gripPressed.GetState(inputSource);
        anchor.gripTouchValue = Mathf.Lerp(anchor.gripTouchValue, anchor.gripPressed ? 1f : 0f, Time.deltaTime * 5);
        middle = anchor.gripTouchValue;

        //primary2DAxis
        SteamVR_Action_Vector2 primary2DAxis = SteamVR_Input.GetVector2Action("ThumbStick");
        SteamVR_Action_Boolean primary2DAxisClick = SteamVR_Input.GetBooleanAction("PadDown");
        anchor.primary2DAxis = primary2DAxis.GetAxis(inputSource);
        anchor.primary2DAxisTouch = anchor.primary2DAxis.sqrMagnitude > 0.02f;
        anchor.primary2DAxisPressed = primary2DAxisClick.GetState(inputSource);
        thumb = Mathf.Max(thumb, (anchor.primary2DAxisPressed || anchor.primary2DAxisTouch) ? 1f : 0f);
        thumb = Mathf.Max(thumb, anchor.primary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);

        //primary
        SteamVR_Action_Boolean primaryPressed = SteamVR_Input.GetBooleanAction("BKey");
        anchor.primaryPressed = primaryPressed.GetState(inputSource);
        anchor.primaryTouchValue = (anchor.primaryPressed) ? 1f : 0f;
        thumb = Mathf.Max(thumb, anchor.primaryTouchValue);

        //secondary
        SteamVR_Action_Boolean secondaryPressed = SteamVR_Input.GetBooleanAction("AKey");
        anchor.secondaryPressed = secondaryPressed.GetState(inputSource);
        anchor.secondaryTouchValue = (anchor.secondaryPressed) ? 1f : 0f;
        thumb = Mathf.Max(thumb, anchor.secondaryTouchValue);

        //fingers
        var skeletonHand = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>(skeletonHandName);
        if (skeletonHand.poseIsValid)
        {
            anchor.handPoseChanged = skeletonHand.poseChanged;
            anchor.fingerCurls[0] = skeletonHand.thumbCurl;
            anchor.fingerCurls[1] = skeletonHand.indexCurl;
            anchor.fingerCurls[2] = skeletonHand.middleCurl;
            anchor.fingerCurls[3] = skeletonHand.ringCurl;
            anchor.fingerCurls[4] = skeletonHand.pinkyCurl;
        }
        else
        {
            anchor.handPoseChanged = true;
            anchor.fingerCurls[0] = thumb;
            anchor.fingerCurls[1] = index;
            anchor.fingerCurls[2] = middle;
            anchor.fingerCurls[3] = middle;
            anchor.fingerCurls[4] = middle;
        }
    }

    Dictionary<int, int> steamvrHandPoseToHumanoid = new Dictionary<int, int>()
    {
        [SteamVR_Skeleton_JointIndexes.thumbProximal] = 0,
        [SteamVR_Skeleton_JointIndexes.thumbMiddle] = 1,
        [SteamVR_Skeleton_JointIndexes.thumbDistal] = 2,

        [SteamVR_Skeleton_JointIndexes.indexProximal] = 3,
        [SteamVR_Skeleton_JointIndexes.indexMiddle] = 4,
        [SteamVR_Skeleton_JointIndexes.indexDistal] = 5,

        [SteamVR_Skeleton_JointIndexes.middleProximal] = 6,
        [SteamVR_Skeleton_JointIndexes.middleMiddle] = 7,
        [SteamVR_Skeleton_JointIndexes.middleDistal] = 8,

        [SteamVR_Skeleton_JointIndexes.ringProximal] = 9,
        [SteamVR_Skeleton_JointIndexes.ringMiddle] = 10,
        [SteamVR_Skeleton_JointIndexes.ringDistal] = 11,

        [SteamVR_Skeleton_JointIndexes.pinkyProximal] = 12,
        [SteamVR_Skeleton_JointIndexes.pinkyMiddle] = 13,
        [SteamVR_Skeleton_JointIndexes.pinkyDistal] = 14,
    };
}
#endif