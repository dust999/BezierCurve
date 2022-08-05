using UnityEngine;
#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
#endif

namespace BizereCurve
{
    public class BezierPoint: MonoBehaviour
    {
        [SerializeField, HideInInspector] private Transform _targetTransformRight;
        [SerializeField, HideInInspector] private Transform _targetTransformLeft;
        
        private void Init()
        {
            if (_targetTransformRight != null)
                return;
            
            var targetRight = new GameObject("TargetRight");
            _targetTransformRight = targetRight.transform;
            _targetTransformRight.position = transform.position + Vector3.right * 2f;
            _targetTransformRight.parent = transform;
            
            var targetLeft = new GameObject("TargetLeft");
            _targetTransformLeft = targetLeft.transform;
            _targetTransformLeft.position = transform.position + Vector3.left * 2f;
            _targetTransformLeft.parent = transform;
            
            #if UNITY_EDITOR
            EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));
            DrawIcon(gameObject, 6);
            DrawIcon(targetRight, 1);
            DrawIcon(targetLeft, 1);
            #endif
        }

        public Vector3 GetPos => transform.position;
        public Vector3 GetTargetRightPos => _targetTransformRight.position;
        public Vector3 GetTargetLeftPos => _targetTransformLeft.position;

        private void OnValidate()
        {
            Init();
        }

        #if UNITY_EDITOR
        
        private void DrawIcon(GameObject gameObject, int idx)
        {
            var largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
            var icon = largeIcons[idx];
            var egu = typeof(EditorGUIUtility);
            var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { gameObject, icon.image };
            var setIcon = egu.GetMethod("SetIconForObject", flags, null, new Type[]{typeof(UnityEngine.Object), typeof(Texture2D)}, null);
            setIcon.Invoke(null, args);
        }
        private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
        {
            GUIContent[] array = new GUIContent[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
            }
            return array;
        }
        
        #endif
    }
}