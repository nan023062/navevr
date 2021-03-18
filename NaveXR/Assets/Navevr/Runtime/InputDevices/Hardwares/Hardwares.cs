using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nave.VR
{
    [Serializable]
    internal class Hardwares : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector] public Hardware test;

        [CustomEditor(typeof(Hardwares))]
        class SelfEditor : Editor
        {
            private SerializedProperty m_HardwarePrefabs;

            private System.Action m_DelayCall;

            private void OnEnable()
            {
                m_HardwarePrefabs = serializedObject.FindProperty("hardwarePrefabs");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var self = (Hardwares)target;

                for (int i = 0; i < m_HardwarePrefabs.arraySize; i++) {
                    var define = m_HardwarePrefabs.GetArrayElementAtIndex(i);
                    using (new EditorGUILayout.VerticalScope("box")) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            var hardware = define.objectReferenceValue as Hardware;
                            if(EditorGUILayout.Toggle(self.test == hardware, GUILayout.Width(20))) {
                                self.test = hardware;
                            }
                            define.objectReferenceValue = EditorGUILayout.ObjectField(define.objectReferenceValue, typeof(Hardware));
                            if (GUILayout.Button("-", GUILayout.Width(20))) {
                                var lst = self.hardwarePrefabs.ToList();
                                lst.RemoveAt(i);
                                self.hardwarePrefabs = lst.ToArray();
                                break;
                            }
                        }
                    }
                }

                using (new EditorGUILayout.VerticalScope("box")) {
                    if (GUILayout.Button("+")) {
                        if (self.hardwarePrefabs == null)
                            self.hardwarePrefabs = new Hardware[0];
                        var lst = self.hardwarePrefabs.ToList();
                        lst.Add(null);
                        self.hardwarePrefabs = lst.ToArray();
                    }
                }

            }
        }
#endif
        [SerializeField,HideInInspector]
        public Hardware[] hardwarePrefabs;

        public Hardware CreateHardware(TrackingAnchor anchor)
        {
            string deviceName = InputDevices.deviceName;
            Hardware hardware = anchor.hardware;

#if UNITY_EDITOR
            if(test != null) {
                if(hardware) GameObject.Destroy(hardware);
                hardware = GameObject.Instantiate(test, anchor.transform);
                hardware.transform.localPosition = Vector3.zero;
                hardware.transform.localRotation = Quaternion.identity;
                hardware.SetNodeType(anchor.type);
                return hardware;
            }
#endif

            if (hardware != null) {
                if (hardware.TryMatchName(deviceName)) return hardware;
                else GameObject.Destroy(hardware);
                hardware = null;
            }

            Hardware __prefab = null;
            foreach (var prefab in hardwarePrefabs) {
                if (prefab.TryMatchName(deviceName)) {
                    __prefab = prefab;
                    break;
                }
            }

            if (__prefab == null) {
                Debug.LogError("ShowHardware() 没有找到匹配的设备！！！就默认使用第一个配置");
                __prefab = hardwarePrefabs[0];
            }

            hardware = GameObject.Instantiate(__prefab, anchor.transform);
            hardware.transform.localPosition = Vector3.zero;
            hardware.transform.localRotation = Quaternion.identity;
            hardware.SetNodeType(anchor.type);
            return hardware;
        }

        public void DestroyHardware(TrackingAnchor acnhor)
        {
            Hardware hardware = acnhor.hardware;
            if (hardware != null)
                GameObject.Destroy(hardware);
        }
    }
}
