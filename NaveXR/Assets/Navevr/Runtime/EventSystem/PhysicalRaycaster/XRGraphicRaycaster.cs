using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nave.VR
{
    /// <summary>
    /// XR UI射线检测
    /// </summary>
    public class XRGraphicRaycaster : GraphicRaycaster
    {
        #region UNITY_EDITOR

        [MenuItem("NaveVR/Compoment/XRGraphicRaycaster")]
        private static void AddToUGUI()
        {
            if (Selection.activeObject) {
                var ugui = Selection.activeObject as GameObject;
                AddToUGUI(ugui);
            }
        }

        #endregion

        public static XRGraphicRaycaster AddToUGUI(GameObject ugui)
        {
            var graphic = ugui.GetComponent<GraphicRaycaster>();
            if (!(graphic is XRGraphicRaycaster)) {
                GameObject.DestroyImmediate(graphic);
                graphic = ugui.AddComponent<XRGraphicRaycaster>();
            }
            return graphic as XRGraphicRaycaster;
        }

        private Canvas mCanvas;

        private Canvas UICanvas
        {
            get
            {
                if (mCanvas != null) return mCanvas;
                mCanvas = GetComponent<Canvas>();
                return mCanvas;
            }
        }

        private Camera xrPointCamera;

        public override Camera eventCamera
        {
            get
            {
                if (xrPointCamera != null) return xrPointCamera;
                return base.eventCamera;
            }
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            //XR输入事件
            var xrPointData = eventData as XRPointEventData;
            if(xrPointData != null){
                xrPointCamera = xrPointData.raycastCamera;
                xrPointCamera.farClipPlane = xrPointData.hitDistance;
                if(xrPointData.useFingerRaycast){
                    Vector3 raycasterDirection;
                    if (FingerRaycast(xrPointData, out raycasterDirection)){
                        xrPointCamera.transform.LookAt(xrPointCamera.transform.position + raycasterDirection);
                        base.Raycast(eventData, resultAppendList);
                    }
                }
                else{
                    base.Raycast(eventData, resultAppendList);
                }
            }
            //原生输入事件
            else{
                xrPointCamera = null;
                base.Raycast(eventData, resultAppendList);
            }
        }

        /// <summary>
        /// 手指检测最近的平面
        /// </summary>
        private bool FingerRaycast(XRPointEventData eventData, out Vector3 raycasterDirection)
        {
            raycasterDirection = Vector3.forward;

            if (UICanvas == null) return false;
            if (UICanvas.renderMode == RenderMode.ScreenSpaceOverlay) return false;
            var canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(UICanvas);
            if (canvasGraphics == null || canvasGraphics.Count == 0) return false;

            float maxSqrDistance = eventData.hitDistance * eventData.hitDistance;
            int lengthOfGraphics = canvasGraphics.Count;
            float minSqrDistance = maxSqrDistance + 1f;

            for (int i = 0; i < lengthOfGraphics; i++){
                Graphic graphic = canvasGraphics[i];

                if (graphic.depth == -1 || !graphic.raycastTarget || graphic.canvasRenderer.cull)
                    continue;

                RectTransform rectTransform = graphic.rectTransform;
                if (ignoreReversedGraphics){
                    var rayDir = eventData.origin - rectTransform.position;
                    if (Vector3.Dot(rayDir, rectTransform.forward) > 0) continue;
                }

                float sqrDistance = maxSqrDistance + 1;
                if(rectTransform.InRectangle(eventData.origin, out sqrDistance)){
                    if (sqrDistance < minSqrDistance && sqrDistance <= maxSqrDistance)
                    {   
                        minSqrDistance = sqrDistance;
                        raycasterDirection = rectTransform.forward;
                    }
                }
            }
            return minSqrDistance <= maxSqrDistance;
        }
    }
}
