using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

namespace Nave.VR
{
    public class HandInputPose : HandInputBase
    {
        public HandSkeleton pose { private set; get; }

        public float[] handPose_Value { private set; get; }

        public bool isLeft { private set; get; }

        public bool poseValid { private set; get; } = false;

        public bool poseChanged { private set; get; } = false;

        public HandInputPose(bool isLeft = true) : base(InputKey.Pose)
        {
            pose = new HandSkeleton();
            handPose_Value = new float[5] { 0, 0, 0, 0, 0 };
            this.isLeft = isLeft;
        }

        internal override void UpdateState(TrackingAnchor input)
        {
            HandAnchor hand = input as HandAnchor;
            poseChanged = hand.handPoseChanged;
            if(poseChanged) Array.Copy(hand.fingerCurls, handPose_Value, handPose_Value.Length);
        }

        public void ApplyHumanoidHand(Animator animator)
        {
            if (!poseValid) return;
            pose?.ApplyHumanoidHand(animator, isLeft);
        }
    }
}
