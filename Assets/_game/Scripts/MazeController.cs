using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _game.Scripts
{
    public class MazeController : MonoBehaviour
    {
        [SerializeField] private Transform _wallPrefab;
        [SerializeField] private MazeNode _mazeNodePrefab;
        [SerializeField, Range(0, 30)]
        private int _mazeSizeX;
        [SerializeField, Range(0, 30)]
        private int _mazeSizeY;
        [SerializeField, OnValueChanged(nameof(UpdateIndicators))]
        private bool _showIndicators;
        [SerializeField]
        private float _clockSpeed;

        private MazeNode[,] _maze;
        private (int x, int y) _originPos;
        private (int x, int y) _previousPos;

        private (int x, int y) _originPos2;
        private (int x, int y) _previousPos2;

        [Button]
        private void Start()
        {
            StopAllCoroutines();
            CreateMaze();
            StartCoroutine(nameof(MoveOriginClock));
        }

        [Button]
        private void Stop() { StopAllCoroutines(); }

        [Button("Create Maze")]
        private void CreateMazeBtn() { CreateMaze(); }

        private IEnumerator MoveOriginClock()
        {
            MoveOrigin();
            yield return new WaitForSeconds(_clockSpeed);
            StartCoroutine(nameof(MoveOriginClock));
        }

        private void CreateMaze()
        {
            DeleteChildren();
            _maze = new MazeNode[_mazeSizeX, _mazeSizeY];

            for(int x = 0; x < _mazeSizeX; x++)
            {
                for(int y = 0; y < _mazeSizeY; y++)
                {
                    MazeNode temp = Instantiate(_mazeNodePrefab, new Vector3((x - _mazeSizeX / 2) * 4, 0, (y - _mazeSizeY / 2) * 4), Quaternion.identity, transform);
                    temp.State = x == _mazeSizeX - 1 ? MNState.Down : MNState.Right;
                    if (x == _mazeSizeX - 1 && y == 0)
                    {
                        temp.State = MNState.Empty;
                        _originPos = (x, y);
                    }

                    // if (y == _mazeSize - 1 && x == 0)
                    // {
                    //     temp.State = MNState.Empty;
                    //     _originPos2 = (x, y);
                    // }
                    _maze[x, y] = temp;
                }
            }

            for(int x = 0; x < _mazeSizeX; x++)
            {
                for(int y = 0; y < _mazeSizeY; y++)
                {
                    if (CanMove((x - 1, y)))
                    {
                        if (_maze[x - 1, y]._wallsRight != null && _maze[x - 1, y]._wallsDown.Count != 0)
                            _maze[x, y]._wallsLeft = _maze[x - 1, y]._wallsRight;
                        else
                        {
                            Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position - _maze[x, y].transform.right * 2, Quaternion.identity, transform);
                            newWall.Rotate(Vector3.up, 90);
                            _maze[x, y]._wallsLeft = new List<GameObject>();
                            foreach (Transform child in newWall)
                            {
                                _maze[x, y]._wallsLeft.Add(child.gameObject);
                            }
                        }
                    }
                    else
                    {
                        Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position - _maze[x, y].transform.right * 2, Quaternion.identity, transform);
                        newWall.Rotate(Vector3.up, 90);
                        _maze[x, y]._wallsLeft = new List<GameObject>();
                        foreach (Transform child in newWall)
                        {
                            _maze[x, y]._wallsLeft.Add(child.gameObject);
                        }
                    }

                    if (CanMove((x + 1, y)))
                    {
                        if (_maze[x + 1, y]._wallsLeft != null && _maze[x + 1, y]._wallsDown.Count != 0)
                            _maze[x, y]._wallsRight = _maze[x + 1, y]._wallsLeft;
                        else
                        {
                            Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position + _maze[x, y].transform.right * 2, Quaternion.identity, transform);
                            newWall.Rotate(Vector3.up, 90);
                            _maze[x, y]._wallsRight = new List<GameObject>();
                            foreach (Transform child in newWall)
                            {
                                _maze[x, y]._wallsRight.Add(child.gameObject);
                            }
                        }
                    }
                    else
                    {
                        Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position + _maze[x, y].transform.right * 2, Quaternion.identity, transform);
                        newWall.Rotate(Vector3.up, 90);
                        _maze[x, y]._wallsRight = new List<GameObject>();
                        foreach (Transform child in newWall)
                        {
                            _maze[x, y]._wallsRight.Add(child.gameObject);
                        }
                    }

                    if (CanMove((x, y + 1)))
                    {
                        if (_maze[x, y + 1]._wallsDown != null && _maze[x, y + 1]._wallsDown.Count != 0)
                            _maze[x, y]._wallsUp = _maze[x, y + 1]._wallsDown;
                        else
                        {
                            Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position + _maze[x, y].transform.forward * 2, Quaternion.identity, transform);
                            _maze[x, y]._wallsUp = new List<GameObject>();
                            foreach (Transform child in newWall)
                            {
                                _maze[x, y]._wallsUp.Add(child.gameObject);
                            }
                        }
                    }
                    else
                    {
                        Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position + _maze[x, y].transform.forward * 2, Quaternion.identity, transform);
                        _maze[x, y]._wallsUp = new List<GameObject>();
                        foreach (Transform child in newWall)
                        {
                            _maze[x, y]._wallsUp.Add(child.gameObject);
                        }
                    }

                    if (CanMove((x, y - 1)))
                    {
                        if (_maze[x, y - 1]._wallsUp != null && _maze[x, y - 1]._wallsDown.Count != 0)
                            _maze[x, y]._wallsDown = _maze[x, y - 1]._wallsUp;
                        else
                        {
                            Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position - _maze[x, y].transform.forward * 2, Quaternion.identity, transform);
                            _maze[x, y]._wallsDown = new List<GameObject>();
                            foreach (Transform child in newWall)
                            {
                                _maze[x, y]._wallsDown.Add(child.gameObject);
                            }
                        }
                    }
                    else
                    {
                        Transform newWall = Instantiate(_wallPrefab, _maze[x, y].transform.position - _maze[x, y].transform.forward * 2, Quaternion.identity, transform);
                        _maze[x, y]._wallsDown = new List<GameObject>();
                        foreach (Transform child in newWall)
                        {
                            _maze[x, y]._wallsDown.Add(child.gameObject);
                        }
                    }
                }
            }
            UpdateIndicators();
            if (Application.isPlaying)
            {
                print("up");
                UpdateWalls(force: true);
            }
        }

        private void UpdateIndicators()
        {
            for(int x = 0; x < _mazeSizeX; x++)
            {
                for(int y = 0; y < _mazeSizeY; y++)
                {
                    _maze[x, y].SetIndicators(_showIndicators);
                }
            }
        }


        [Button]
        private void MoveOrigin()
        {
            (int x, int y) newPos;
            int failSafe = 0;
            do
            {
                newPos = _originPos;
                switch (Random.Range(0, 4))
                {
                    case 0:
                        newPos.x -= 1;
                        break;
                    case 1:
                        newPos.x += 1;
                        break;
                    case 2:
                        newPos.y -= 1;
                        break;
                    case 3:
                        newPos.y += 1;
                        break;
                }
                failSafe++;
                if (failSafe <= 1000)
                    continue;
                Debug.LogError("failsafe");
                break;
            }
            while (!CanMove(newPos) || newPos == _originPos || newPos == _previousPos);

            _maze[newPos.x, newPos.y].State = MNState.Empty;
            if (newPos.x == _originPos.x)
            {
                _maze[_originPos.x, _originPos.y].State = newPos.y > _originPos.y ? MNState.Up : MNState.Down;
            }
            if (newPos.y == _originPos.y)
            {
                _maze[_originPos.x, _originPos.y].State = newPos.x > _originPos.x ? MNState.Right : MNState.Left;
            }

            _previousPos = _originPos;
            _originPos = newPos;

            // (int x, int y) newPos2;
            // int failSafe2 = 0;
            // do
            // {
            //     newPos2 = _originPos2;
            //     switch (Random.Range(0, 4))
            //     {
            //         case 0:
            //             newPos2.x -= 1;
            //             break;
            //         case 1:
            //             newPos2.x += 1;
            //             break;
            //         case 2:
            //             newPos2.y -= 1;
            //             break;
            //         case 3:
            //             newPos2.y += 1;
            //             break;
            //     }
            //     failSafe2++;
            //     if (failSafe2 <= 1000)
            //         continue;
            //     Debug.LogError("failsafe");
            //     break;
            // }
            // while (!CanMove(newPos2) || newPos2 == _originPos2 || newPos2 == _previousPos2);
            //
            // _maze[newPos2.x, newPos2.y].State = MNState.Empty;
            // if (newPos2.x == _originPos2.x)
            // {
            //     _maze[_originPos2.x, _originPos2.y].State = newPos2.y > _originPos2.y ? MNState.Up : MNState.Down;
            // }
            // if (newPos2.y == _originPos2.y)
            // {
            //     _maze[_originPos2.x, _originPos2.y].State = newPos2.x > _originPos2.x ? MNState.Right : MNState.Left;
            // }
            //
            // _previousPos2 = _originPos2;
            // _originPos2 = newPos2;
            UpdateWalls();
        }

        private void UpdateWalls(bool force = false)
        {
            for(int x = 0; x < _mazeSizeX; x++)
            {
                for(int y = 0; y < _mazeSizeY; y++)
                {
                    if (_maze[x, y].State is MNState.Left)
                    {
                        bool wallUp = CanMove((x, y + 1)) && _maze[x, y + 1].State == MNState.Down;
                        bool wallDown = CanMove((x, y - 1)) && _maze[x, y - 1].State == MNState.Up;
                        bool wallRight = CanMove((x + 1, y)) && _maze[x + 1, y].State == MNState.Left;
                        _maze[x, y].SetWalls(wallUp: !wallUp, wallDown: !wallDown, wallRight: !wallRight, force: force);
                    }
                    if (_maze[x, y].State is MNState.Right)
                    {
                        bool wallUp = CanMove((x, y + 1)) && _maze[x, y + 1].State == MNState.Down;
                        bool wallDown = CanMove((x, y - 1)) && _maze[x, y - 1].State == MNState.Up;
                        bool wallLeft = CanMove((x - 1, y)) && _maze[x - 1, y].State == MNState.Right;
                        _maze[x, y].SetWalls(wallUp: !wallUp, wallDown: !wallDown, wallLeft: !wallLeft, force: force);
                    }
                    if (_maze[x, y].State is MNState.Up)
                    {
                        bool wallDown = CanMove((x, y - 1)) && _maze[x, y - 1].State == MNState.Up;
                        bool wallRight = CanMove((x + 1, y)) && _maze[x + 1, y].State == MNState.Left;
                        bool wallLeft = CanMove((x - 1, y)) && _maze[x - 1, y].State == MNState.Right;
                        _maze[x, y].SetWalls(wallLeft: !wallLeft, wallDown: !wallDown, wallRight: !wallRight, force: force);
                    }
                    if (_maze[x, y].State is MNState.Down)
                    {
                        bool wallUp = CanMove((x, y + 1)) && _maze[x, y + 1].State == MNState.Down;
                        bool wallRight = CanMove((x + 1, y)) && _maze[x + 1, y].State == MNState.Left;
                        bool wallLeft = CanMove((x - 1, y)) && _maze[x - 1, y].State == MNState.Right;
                        _maze[x, y].SetWalls(wallUp: !wallUp, wallLeft: !wallLeft, wallRight: !wallRight, force: force);
                    }
                    if (_maze[x, y].State is MNState.Empty)
                    {
                        bool wallUp = CanMove((x, y + 1)) && _maze[x, y + 1].State == MNState.Down;
                        bool wallDown = CanMove((x, y - 1)) && _maze[x, y - 1].State == MNState.Up;
                        bool wallRight = CanMove((x + 1, y)) && _maze[x + 1, y].State == MNState.Left;
                        bool wallLeft = CanMove((x - 1, y)) && _maze[x - 1, y].State == MNState.Right;
                        _maze[x, y].SetWalls(wallUp: !wallUp, wallDown: !wallDown, wallRight: !wallRight, wallLeft: !wallLeft, force: force);
                    }
                }
            }
        }

        private bool CanMove((int x, int y) position)
        {
            if (position.x < 0 || position.x >= _mazeSizeX)
                return false;

            if (position.y < 0 || position.y >= _mazeSizeY)
                return false;

            return true;
        }

        private void DeleteChildren()
        {
            while (transform.childCount > 0)
            {
                foreach (Transform child in transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
