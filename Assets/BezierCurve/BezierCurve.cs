using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BizereCurve
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField] private List<BezierPoint> _points = new List<BezierPoint>();
        [SerializeField] private float _curveStep = 0.1f;
        [SerializeField] private bool _drawPoints = false;
        
        private LineRenderer _lineRenderer;
        private int _lastChildren;

        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
        

        private void Update()
        {
            UpdateLine();
            
            #if UNITY_EDITOR
            if (transform.childCount != _lastChildren)
            {
                _points = GetComponentsInChildren<BezierPoint>().ToList();
                _lastChildren = transform.childCount;
            }
            #endif
        }
        
        private void UpdateLine()
        {
            if(_points.Count < 2)
                return;
            
            List<Vector3> segmentPoints = new List<Vector3>();
            
            for (int i = 1; i < _points.Count; i++)
            {
                if (_points[i] == null)
                {
                    _points.Remove(_points[i]);
                    break;
                }
                    
                segmentPoints.AddRange(GetSegmentPoints(_points[i-1], _points[i], _curveStep));
            }

            _lineRenderer.positionCount = segmentPoints.Count;
            _lineRenderer.SetPositions(segmentPoints.ToArray());

        }

        private List<Vector3> GetSegmentPoints(BezierPoint pointFirst, BezierPoint pointLast, float distToPoint)
        {
            float step = 0;
            
            Vector3 posFirst = pointFirst.GetPos;
            Vector3 posFirstTop = pointFirst.GetTargetRightPos;
            Vector3 posLast = pointLast.GetPos;
            Vector3 posLastTop = pointLast.GetTargetLeftPos;
            
            List<Vector3> curvePoints = new List<Vector3>();
            
            while (step <= 1.1)
            {
                Vector3 curveFirstPoint = Vector3.Lerp(posFirst, posFirstTop, step);
                Vector3 curveTopPoint = Vector3.Lerp(posFirstTop, posLastTop, step);
                Vector3 curveFirstMiddle = Vector3.Lerp(curveFirstPoint, curveTopPoint, step);
                Vector3 curveLastPoint = Vector3.Lerp(posLastTop, posLast, step);
                Vector3 curveLastMiddle = Vector3.Lerp(curveTopPoint, curveLastPoint, step);
                Vector3 curvePoint = Vector3.Lerp(curveFirstMiddle, curveLastMiddle, step);
                
                curvePoints.Add(curvePoint);
                step += distToPoint;
            }
            
            return curvePoints;
        }

        #region EDITOR
        
        [ContextMenu("AddPoint")]
        private void AddPoint()
        {
            Vector3 startPos = transform.position;
            Vector3 direction = transform.forward;
            
            startPos += 2f *_points.Count * direction;
            
            var go = new GameObject($"Point {_points.Count + 1}");
            go.transform.parent = transform;
            go.transform.position = startPos;
            
            BezierPoint point = go.AddComponent<BezierPoint>();
            _points.Add(point);
        }
        
        private void OnDrawGizmos()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                if (_points[i] == null)
                {
                    _points.Remove(_points[i]);
                    UpdateLine();
                    break;
                }
                
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_points[i].GetTargetRightPos, 0.1f);
                Gizmos.DrawWireSphere(_points[i].GetTargetLeftPos, 0.1f);
                Gizmos.DrawLine(_points[i].GetPos, _points[i].GetTargetRightPos);
                Gizmos.DrawLine(_points[i].GetPos, _points[i].GetTargetLeftPos);
            }
            
            if(_drawPoints == false)
                return;
            
            for (int i = 0; i < _lineRenderer.positionCount; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_lineRenderer.GetPosition(i), 0.1f);
            }
        }
        
        #endregion
    }
}
