using System.Collections.Generic;
using DG.Tweening;
using EditorAttributes;
using UnityEngine;

namespace _game.Scripts
{
    public class MazeNode : MonoBehaviour
    {
        [SerializeField, OnValueChanged(nameof(StateChangedCallback))]
        private MNState _state;
        public MNState State
        {
            get { return _state; }
            set
            {
                _state = value;
                if (_state != _lastState)
                {
                    SetState(value);
                    _lastState = _state;
                }
                timer = 0f;
            }
        }

        [SerializeField] private Vector2Int _fullWallRange;
        [SerializeField] private Vector2Int _bottomHoleRange;
        [SerializeField] private Vector2Int _topHoleRange;

        public List<GameObject> _wallsUp;
        public List<GameObject> _wallsDown;
        public List<GameObject> _wallsRight;
        public List<GameObject> _wallsLeft;
        //[SerializeField] private GameObject _wallUp, _wallDown, _wallRight, _wallLeft;
        [SerializeField] private Transform _arrow, _dot;

        public float duration = 5.0f; // Time to complete the entire color transition
        private float timer = 0.0f;
        private MNState _lastState = MNState.Empty;
        private bool _lastStateUp = true, _lastStateDown = true, _lastStateRight = true, _lastStateLeft = true;
        private static readonly int Dissolve1 = Shader.PropertyToID("_Dissolve");

        void Update()
        {
            // Increase the timer based on deltaTime, then normalize it (0 to 1)
            timer += Time.deltaTime;
            float normalizedTime = timer / duration;

            // First half: Red to Yellow
            if (normalizedTime <= 0.5f)
            {
                // Lerp from Red to Yellow (normalized time from 0 to 0.5)
                float t = normalizedTime / 0.5f; // Remap to 0-1
                _arrow.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.yellow, t);
                _dot.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.yellow, t);

            }
            // Second half: Yellow to Green
            else if (normalizedTime <= 1.0f)
            {
                // Lerp from Yellow to Green (normalized time from 0.5 to 1)
                float t = (normalizedTime - 0.5f) / 0.5f; // Remap to 0-1
                _arrow.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.yellow, Color.green, t);
                _dot.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.yellow, t);
            }
        }

        private void StateChangedCallback() { SetState(_state); }

        private void SetState(MNState newState)
        {
            switch (newState)
            {
                case MNState.Empty:
                    //SetWalls();
                    _dot.rotation = Quaternion.Euler(90, 0, 0);
                    _arrow.rotation = Quaternion.Euler(Vector3.zero);
                    break;
                case MNState.Up:
                    //SetWalls(wallRight: true, wallLeft: true);
                    _dot.rotation = Quaternion.Euler(Vector3.zero);
                    _arrow.rotation = Quaternion.Euler(90, -90, 0);
                    break;
                case MNState.Down:
                    //SetWalls(wallRight: true, wallLeft: true);
                    _dot.rotation = Quaternion.Euler(Vector3.zero);
                    _arrow.rotation = Quaternion.Euler(90, 90, 0);
                    break;
                case MNState.Right:
                    //SetWalls(wallUp: true, wallDown: true);
                    _dot.rotation = Quaternion.Euler(Vector3.zero);
                    _arrow.rotation = Quaternion.Euler(90, 0, 0);
                    break;
                case MNState.Left:
                    //SetWalls(wallUp: true, wallDown: true);
                    _dot.rotation = Quaternion.Euler(Vector3.zero);
                    _arrow.rotation = Quaternion.Euler(90, 180, 0);
                    break;
                case MNState.Full:
                    //SetWalls(true, true, true, true);
                    _dot.rotation = Quaternion.Euler(Vector3.zero);
                    _arrow.rotation = Quaternion.Euler(Vector3.zero);
                    break;
            }
        }

        public void SetIndicators(bool value)
        {
            _arrow.gameObject.SetActive(value);
            _dot.gameObject.SetActive(value);
        }

        public void SetWalls(bool wallUp = false, bool wallDown = false, bool wallRight = false, bool wallLeft = false, bool force = false)
        {
            if (_lastStateUp != wallUp || force)
            {
                PickWall(_wallsUp, wallUp, force);
                _lastStateUp = wallUp;
            }

            if (_lastStateDown != wallDown || force)
            {
                PickWall(_wallsDown, wallDown, force);
                _lastStateDown = wallDown;
            }

            if (_lastStateRight != wallRight || force)
            {
                PickWall(_wallsRight, wallRight, force);
                _lastStateRight = wallRight;
            }

            if (_lastStateLeft != wallLeft || force)
            {
                PickWall(_wallsLeft, wallLeft, force);
                _lastStateLeft = wallLeft;
            }
            // _wallUp.SetActive(wallUp);
            // _wallDown.SetActive(wallDown);
            // _wallRight.SetActive(wallRight);
            // _wallLeft.SetActive(wallLeft);
        }

        private void PickWall(List<GameObject> wallList, bool value, bool force = false)
        {
            if (wallList == null)
                return;

            if (value)
            {
                int randomWallId = RandomWall();
                if (force)
                    randomWallId = 0;
                for(int i = 0; i < wallList.Count; i++)
                {
                    if (i == randomWallId)
                    {
                        Dissolve(wallList[i], true);
                    }
                    else
                    {
                        if (wallList[i].GetComponent<Collider>().enabled || DOTween.IsTweening(wallList[i].GetComponent<Renderer>().material))
                            Dissolve(wallList[i], false);
                        else
                            Dissolve(wallList[i], false, true);
                    }
                }
            }
            else
            {
                foreach (GameObject t in wallList)
                {
                    Dissolve(t, false, true);
                }
            }

            // foreach (GameObject wall in wallList)
            // {
            //     //wall.SetActive(false);
            //     if (wall.GetComponent<Collider>().enabled || DOTween.IsTweening(wall.GetComponent<Renderer>().material))
            //         Dissolve(wall, false);
            //     else
            //         Dissolve(wall, false, true);
            // }
            // if (value)
            // {
            //     
            //     //wallList[randomWallId].SetActive(true);
            //     Dissolve(wallList[randomWallId], true);
            // }
        }

        private int RandomWall()
        {
            int x = Random.Range(0, 100);

            if (x >= _fullWallRange.x && x < _fullWallRange.y)
                return 0;
            if (x >= _bottomHoleRange.x && x < _bottomHoleRange.y)
                return 1;
            if (x >= _topHoleRange.x && x < _topHoleRange.y)
                return 2;

            return -1;
        }

        private static void Dissolve(GameObject wall, bool value, bool instant = false)
        {
            if (value)
            {
                Material mat = wall.GetComponent<Renderer>().material;
                mat.DOKill();
                if (instant)
                {
                    mat.SetFloat(Dissolve1, 0f);
                    wall.GetComponent<Collider>().enabled = true;
                }
                else
                {
                    mat.DOFloat(0f, Dissolve1, 1f).SetEase(Ease.OutCirc).OnComplete(() => wall.GetComponent<Collider>().enabled = true);
                }
            }
            else
            {
                Material mat = wall.GetComponent<Renderer>().material;
                mat.DOKill();
                if (instant)
                {
                    mat.SetFloat(Dissolve1, 1f);
                    wall.GetComponent<Collider>().enabled = false;
                }
                else
                {
                    mat.DOFloat(1f, Dissolve1, 1f).SetEase(Ease.InCirc).OnStart(() => wall.GetComponent<Collider>().enabled = false);
                }
            }
        }
    }

    public enum MNState
    {
        Empty,
        Up,
        Down,
        Right,
        Left,
        Full
    }
}
