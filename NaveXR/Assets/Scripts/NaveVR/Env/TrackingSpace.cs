using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nave.VR
{
    /// <summary>
    /// VR Tracking Acnhors
    /// </summary>
    public abstract class TrackingSpace : MonoBehaviour
    {
        private const string c_HeadAnchor = "TrackingSpace/HeadAnchor";

        private const string c_LeftHandAnchor = "TrackingSpace/LeftHandAnchor";

        private const string c_RightHandAnchor = "TrackingSpace/RightHandAnchor";

        private const string c_PelivsAnchor = "TrackingSpace/PelivsAnchor";

        private const string c_LeftFootAnchor = "TrackingSpace/LeftFootAnchor";

        private const string c_RightFootAnchor = "TrackingSpace/RightFootAnchor";

        [Header("Tracking Anchors")]

        public HeadAnchor headAnchor;

        public LeftHandAnchor leftHandAnchor;

        public RightHandAnchor rightHandAnchor;

        public TrackingAnchor pelivsAnchor;

        public TrackingAnchor leftFootAnchor;

        public TrackingAnchor rightFootAnchor;

        [Header("Hardware Prefabs Defs"), SerializeField]
        internal Hardwares hardwarePrefabsDefs;

        protected virtual void Awake()
        {
            headAnchor.transform = transform.Find(c_HeadAnchor);

            leftHandAnchor.transform = transform.Find(c_LeftHandAnchor);

            rightHandAnchor.transform = transform.Find(c_RightHandAnchor);

            pelivsAnchor.transform = transform.Find(c_PelivsAnchor);

            leftFootAnchor.transform = transform.Find(c_LeftFootAnchor);

            rightFootAnchor.transform = transform.Find(c_RightFootAnchor);
        }

        private void ApplyCurrentPoseToAnchors()
        {
            //将Pose数据应用到transform
            headAnchor.ApplyPoseToTransform();

            leftHandAnchor.ApplyPoseToTransform();

            rightHandAnchor.ApplyPoseToTransform();

            pelivsAnchor.ApplyPoseToTransform();

            leftFootAnchor.ApplyPoseToTransform();

            rightFootAnchor.ApplyPoseToTransform();
        }
        
        public void ProcessTrackingSpace()
        {
            OnPreProcessTrackingAnchors();

            OnProcessTrackingAnchors();

            OnPostProcessTrackingAnchors();

            ApplyCurrentPoseToAnchors();
        }

        /// <summary>
        /// Before Tracking Anchors Processing
        /// </summary>
        protected abstract void OnPreProcessTrackingAnchors();

        /// <summary>
        /// On Tracking Anchors Processing
        /// </summary>
        protected abstract void OnProcessTrackingAnchors();

        /// <summary>
        /// After Tracking Anchors Processed
        /// </summary>
        protected abstract void OnPostProcessTrackingAnchors();

        [Header("Draw Gizmos")]
        [SerializeField] Color color = Color.red;

        private Mesh mesh = null;

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.vertices = new Vector3[] {
                    new Vector3(-1,-1,-1),
                    new Vector3(1,-1,-1),
                    new Vector3(-1,1,-1),
                    new Vector3(1,1,-1),
                    new Vector3(-1,-1,1),
                    new Vector3(1,-1,1),
                    new Vector3(-1,1,1),
                    new Vector3(1,1,1),
                };
                mesh.triangles = new int[] {
                    0,2,3, 0,3,1,
                    1,3,7, 1,7,5,
                    4,6,2, 4,2,0,
                    4,0,1, 4,1,5,
                    2,6,7, 2,7,3,
                    6,4,5, 6,5,7
                };
                mesh.RecalculateNormals();
            }

            TrackingAnchor[] anchors = new TrackingAnchor[] {
                headAnchor, leftHandAnchor, rightHandAnchor,
                pelivsAnchor, leftFootAnchor, rightFootAnchor
            };

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            foreach (var anchor in anchors) {
                if(anchor.transform != null)
                    Gizmos.DrawMesh(mesh, anchor.transform.position, 
                        anchor.transform.rotation, new Vector3(0.02f, 0.02f, 0.1f));
            }
            Gizmos.color = oldColor;
        }
    }

    

}
