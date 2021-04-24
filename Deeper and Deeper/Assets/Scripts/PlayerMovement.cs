using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    private Dictionary<Direction, Vector3> _vectors;

    private Direction _direction = Direction.None;

    public float _speed = 5f;

    public BlockType[] _collisions;

    void Awake()
    {
        _vectors = new Dictionary<Direction, Vector3>();
        _vectors.Add(Direction.Up, Vector3.up);
        _vectors.Add(Direction.Down, Vector3.down);
        _vectors.Add(Direction.Left, Vector3.left);
        _vectors.Add(Direction.Right, Vector3.right);
    }

    public IEnumerator Move()
    {
        if (_direction == Direction.None)
        {
            yield return null;
        }
        else if (EnemyInSight())
        {

        }
        else
        {
            var time = 0f;
            var target = transform.position + _vectors[_direction];
            var start = transform.position;

            // Keep player in bounds
            if (target.x < 0)
            {
                target.x = 0;
            }
            else if (target.x > 7)
            {
                target.x = 7;
            }

            if (target.y > 0)
            {
                target.y = 0;
            }
            else if (target.y < -7)
            {
                target.y = -7;
            }

            // Don't let the player walk on walls
            var cellX = (int)target.x;
            var cellY = (int)-target.y;
            var index = cellY * 8 + cellX;
            if (_collisions[index] != BlockType.Walk)
            {
                target = start;
            }

            if (target != start)
            {
                // Move the player
                while (time < 1f)
                {
                    transform.position = Vector3.Lerp(start, target, time);
                    time += Time.deltaTime * _speed;
                    yield return null;
                }
            }
            else
            {
                float shimmyTime = 0f;
                var pos = transform.position;
                while (shimmyTime < 1f)
                {
                    transform.position = pos + Shimmy();
                    shimmyTime += Time.deltaTime * 10f;
                    yield return null;
                }

                transform.localPosition = Vector3.zero;
            }

            // Set to new position to make sure it's 100% on cell and reset direction
            transform.position = target;
            _direction = Direction.None;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _direction = Direction.Up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _direction = Direction.Down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _direction = Direction.Left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _direction = Direction.Right;
        }
    }

    private bool EnemyInSight()
    {
        return false;
    }

    private Vector3 Shimmy()
    {
        var vec = Random.insideUnitCircle / 10f;
        return new Vector3(vec.x, vec.y, 0);
    }
}
