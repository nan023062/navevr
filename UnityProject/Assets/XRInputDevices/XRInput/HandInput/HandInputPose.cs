using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

namespace NaveXR.InputDevices
{
    public class HandInputPose : HandInputBase
    {
        public HandPoseData handPoseData { private set; get; }

        public bool isLeft { private set; get; }

        public HandInputPose(bool isLeft = true) : base(XRKeyCode.HandPose)
        {
            handPoseData = new HandPoseData();
            this.isLeft = isLeft;
        }

        public override void UpdateState(UnityEngine.XR.InputDevice device)
        {
            Hand hand;
            if (device.TryGetFeatureValue(CommonUsages.handData, out hand))
                handPoseData.FilledWithUnityXRHand(ref hand);
        }

        public void ApplyHumanoidHand(Animator animator)
        {
            handPoseData.ApplyHumanoidHand(animator, isLeft);
        }

        public class HandPoseData
        {
            public const int handNum = 2;

            public const int boneNum = 15;

            static HumanBodyBones[,] HumanBodys = new HumanBodyBones[handNum, boneNum]{
                {
                    HumanBodyBones.LeftThumbProximal,
                    HumanBodyBones.LeftThumbIntermediate,
                    HumanBodyBones.LeftThumbDistal,

                    HumanBodyBones.LeftIndexProximal,
                    HumanBodyBones.LeftIndexIntermediate,
                    HumanBodyBones.LeftIndexDistal,

                    HumanBodyBones.LeftMiddleProximal,
                    HumanBodyBones.LeftMiddleIntermediate,
                    HumanBodyBones.LeftMiddleDistal,

                    HumanBodyBones.LeftRingProximal,
                    HumanBodyBones.LeftRingIntermediate,
                    HumanBodyBones.LeftRingDistal,

                    HumanBodyBones.LeftLittleProximal,
                    HumanBodyBones.LeftLittleIntermediate,
                    HumanBodyBones.LeftLittleDistal,
                },
                {
                    HumanBodyBones.RightThumbProximal,
                    HumanBodyBones.RightThumbIntermediate,
                    HumanBodyBones.RightThumbDistal,

                    HumanBodyBones.RightIndexProximal,
                    HumanBodyBones.RightIndexIntermediate,
                    HumanBodyBones.RightIndexDistal,

                    HumanBodyBones.RightMiddleProximal,
                    HumanBodyBones.RightMiddleIntermediate,
                    HumanBodyBones.RightMiddleDistal,

                    HumanBodyBones.RightRingProximal,
                    HumanBodyBones.RightRingIntermediate,
                    HumanBodyBones.RightRingDistal,

                    HumanBodyBones.RightLittleProximal,
                    HumanBodyBones.RightLittleIntermediate,
                    HumanBodyBones.RightLittleDistal,
                }
            };

            public struct Bone
            {
                public Vector3 position;
                public Quaternion rotation;
            }

            public Bone[] bones = new Bone[boneNum];

            public HandPoseData()
            {
                for (int i = 0; i < boneNum; i++)
                {
                    bones[i] = new Bone();
                    bones[i].position = Vector3.zero;
                    bones[i].rotation = Quaternion.identity;
                }
            }

            public void ApplyHumanoidHand(Animator animator, bool leftHand)
            {
                int index = leftHand ? 0 : 1;
                for (int i = 0; i < boneNum; i++)
                {
                    HumanBodyBones humanBody = HumanBodys[index, i];
                    Transform bone = animator.GetBoneTransform(humanBody);
                    if (bone != null)
                    {
                        bone.localPosition = bones[i].position;
                        bone.localRotation = bones[i].rotation;
                    }
                }
            }

            public void FilledWithUnityXRHand(ref UnityEngine.XR.Hand hand)
            {
                var fingerBones = new List<UnityEngine.XR.Bone>();

                for (int i = 0; i < 5; i++)
                {
                    fingerBones.Clear();
                    hand.TryGetFingerBones((HandFinger)i, fingerBones);
                    for (int j = 0; j < fingerBones.Count; j++)
                    {
                        var fingerBone = fingerBones[j];
                        var bone = bones[i * 3 + j];

                        Vector3 position = bone.position;
                        fingerBone.TryGetPosition(out position);
                        bone.position = position;

                        Quaternion rotation = bone.rotation;
                        fingerBone.TryGetRotation(out rotation);
                        bone.rotation = rotation;
                    }
                }
            }
        }
    }
}
