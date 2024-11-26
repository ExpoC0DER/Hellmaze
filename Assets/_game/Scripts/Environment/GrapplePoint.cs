using System;
using UnityEditor;
using UnityEngine;

namespace _game.Scripts
{
    public class GrapplePoint : MonoBehaviour
    {
        [SerializeField] private Renderer _wallRenderer;
        [SerializeField] private Vector3 _arrivalOffset;

        public Vector3 ArrivalPos { get { return transform.position - _arrivalOffset; } }

        private Collider _collider;
        private Material _wallMat;
        private Material _myMat;
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        private void Start()
        {
            _wallMat = _wallRenderer.material;
            _myMat = GetComponent<Renderer>().material;
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            float wallDissolve = _wallMat.GetFloat(Dissolve);
            _myMat.SetFloat(Dissolve, wallDissolve);
            if (wallDissolve < 0.1f)
            {
                _collider.enabled = true;
            }
            else
            {
                _collider.enabled = false;
            }
        }

        private void OnDrawGizmosSelected() { Gizmos.DrawSphere(transform.position - _arrivalOffset, 0.1f); }
    }
}
