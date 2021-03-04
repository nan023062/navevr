using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#if SUPPORT_STEAM_VR
using Valve.VR;

namespace Nave.XR
{
    [XREnv(name="UnitySteamvr",lib = XRLib.OpenVR)]
    internal class UnitySteamvrEvn : Nave.XR.BaseEvn
    {
        protected override IEnumerator InitEvnAsync(Action<string> onResult)
        {
            SteamVR.Initialize(true);
            yield return null;
            while (SteamVR.initializedState == SteamVR.InitializedStates.Initializing) yield return 0;     
            
            if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess) {   
                onResult?.Invoke(string.Empty);
                var actionSet = SteamVR_Input.GetActionSet("key");
                actionSet?.Activate(SteamVR_Input_Sources.LeftHand);
                actionSet?.Activate(SteamVR_Input_Sources.RightHand);
            }
            else {    
                onResult?.Invoke("UnitySteamvrEvn初始化失败");
            }
            yield return null;
        }

        internal override void Release()
        {
            base.Release();
            SteamVR.SafeDispose();
        }

        protected override void UpdateHandNodeState(HandMatedata hand, ref XRNodeState xRNode)
        {
            //base.UpdateHandNodeState(hand, ref xRNode);
            float thumb = 0f, index = 0f, middle = 0f;

            //psotion && rotation
            xRNode.TryGetPosition(out hand.position);
            xRNode.TryGetRotation(out hand.rotation);

            var inputSource = SteamVR_Input_Sources.LeftHand;
            string skeletonHandName = "SkeletonLeftHand";
            if (hand.type == NodeType.RightHand)
            {
                inputSource = SteamVR_Input_Sources.RightHand;
                skeletonHandName = "skeletonRightHand";
            }

            //SteamVR_Action_Boolean key_MenuKey = SteamVR_Input.GetBooleanAction("MenuKey");
            //SteamVR_Action_Boolean key_SystemKey = SteamVR_Input.GetBooleanAction("SystemKey");

            //trigger
            SteamVR_Action_Boolean triggerPressed = SteamVR_Input.GetBooleanAction("Trigger");
            //SteamVR_Action_Single triggerTouchValue = SteamVR_Input.GetSingleAction("triggerTouchValue");
            hand.triggerPressed = triggerPressed.GetState(inputSource);
            hand.triggerTouchValue = Mathf.Lerp(hand.triggerTouchValue, hand.triggerPressed ? 1f : 0f, Time.deltaTime * 5);
            index = hand.triggerTouchValue;

            //grip
            SteamVR_Action_Boolean gripPressed = SteamVR_Input.GetBooleanAction("SideTrigger");
            //SteamVR_Action_Single gripTouchValue = SteamVR_Input.GetSingleAction("gripTouchValue");
            hand.gripPressed = gripPressed.GetState(inputSource);
            hand.gripTouchValue = Mathf.Lerp(hand.gripTouchValue, hand.gripPressed ? 1f : 0f, Time.deltaTime * 5);
            middle = hand.gripTouchValue;

            //primary2DAxis
            SteamVR_Action_Vector2 primary2DAxis = SteamVR_Input.GetVector2Action("ThumbStick");
            SteamVR_Action_Boolean primary2DAxisClick = SteamVR_Input.GetBooleanAction("PadDown");
            hand.primary2DAxis = primary2DAxis.GetAxis(inputSource);
            hand.primary2DAxisTouch = hand.primary2DAxis.sqrMagnitude > 0.02f;
            hand.primary2DAxisPressed = primary2DAxisClick.GetState(inputSource);
            thumb = Mathf.Max(thumb, (hand.primary2DAxisPressed || hand.primary2DAxisTouch) ? 1f : 0f);
            thumb = Mathf.Max(thumb, hand.primary2DAxis.sqrMagnitude > 0.1f ? 1f : 0f);

            //primary
            SteamVR_Action_Boolean primaryPressed = SteamVR_Input.GetBooleanAction("BKey");
            hand.primaryPressed = primaryPressed.GetState(inputSource);
            hand.primaryTouchValue = (hand.primaryPressed) ? 1f : 0f;
            thumb = Mathf.Max(thumb, hand.primaryTouchValue);

            //secondary
            SteamVR_Action_Boolean secondaryPressed = SteamVR_Input.GetBooleanAction("AKey");
            hand.secondaryPressed = secondaryPressed.GetState(inputSource);
            hand.secondaryTouchValue = (hand.secondaryPressed) ? 1f : 0f;
            thumb = Mathf.Max(thumb, hand.secondaryTouchValue);

            //fingers
            var skeletonHand = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>(skeletonHandName);
            if (skeletonHand.poseIsValid)
            {
                hand.handPoseChanged = skeletonHand.poseChanged;
                hand.fingerCurls[0] = skeletonHand.thumbCurl;
                hand.fingerCurls[1] = skeletonHand.indexCurl;
                hand.fingerCurls[2] = skeletonHand.middleCurl;
                hand.fingerCurls[3] = skeletonHand.ringCurl;
                hand.fingerCurls[4] = skeletonHand.pinkyCurl;
            }
            else
            {
                hand.handPoseChanged = true;
                hand.fingerCurls[0] = thumb;
                hand.fingerCurls[1] = index;
                hand.fingerCurls[2] = middle;
                hand.fingerCurls[3] = middle;
                hand.fingerCurls[4] = middle;
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
}
#endif