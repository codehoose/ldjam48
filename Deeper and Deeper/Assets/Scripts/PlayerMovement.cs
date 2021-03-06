using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private GameObject _touchedTesseract;

    private GameObject _touchedChest;

    private GameObject _enemyTarget;

    private bool _exitRequested;

    private Dictionary<Direction, Vector3> _vectors;

    private Direction _direction = Direction.None;

    public EnemyController _enemyController;

    public float _speed = 5f;

    public bool _godMode = false;

    [HideInInspector]
    public BlockType[] _collisions;

    public int _tesseracts;

    public bool _paused;

    [HideInInspector]
    public int _health = 10;

    [HideInInspector]
    public bool _doExitSequence;

    public int _grenades;

    public Image _healthMeter;

    public TMPro.TextMeshProUGUI _grenadeIndicator;

    public void Reset()
    {
        _paused = false;
        _exitRequested = false;
        _doExitSequence = false;

        _touchedTesseract = null;
        _touchedChest = null;
        _enemyTarget = null;
    }

    void Awake()
    {
        _vectors = new Dictionary<Direction, Vector3>();
        _vectors.Add(Direction.Up, Vector3.up);
        _vectors.Add(Direction.Down, Vector3.down);
        _vectors.Add(Direction.Left, Vector3.left);
        _vectors.Add(Direction.Right, Vector3.right);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tesseract")
        {
            _touchedTesseract = collision.gameObject;
        }
        else if (collision.tag == "Chest")
        {
            _touchedChest = collision.gameObject;
        }
        else if (collision.tag == "Exit")
        {
            _exitRequested = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Chest")
        {
            _touchedChest = null;
        }
    }

    public IEnumerator Move()
    {
        if (_paused)
        {
            yield return null;
        }


        if (_direction == Direction.None)
        {
            yield return null;
        }
        else if (EnemyInSight())
        {
            _direction = Direction.None;
            _enemyController.HitEnemy(_enemyTarget);
            _enemyTarget = null;

            // Even if the player is firing, the other enemies can move...
            _enemyController.PlayerMoved(this);
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
            if (_collisions[index] != BlockType.Walk && _collisions[index] != BlockType.Exit)
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

                // Did we touch a tesseract?
                if (_touchedTesseract != null)
                {
                    Destroy(_touchedTesseract);
                    _touchedTesseract = null;
                    _tesseracts++;
                    _tesseracts = Mathf.Min(_tesseracts, 5); // Max five tesseracts
                }
                else if (_exitRequested)
                {
                    _doExitSequence = true;
                }
            }
            else
            {
                yield return DoTheShimmy();
            }

            // Tell the enemy controller - This will be used to spawn enemies
            _enemyController.PlayerMoved(this);

            // Set to new position to make sure it's 100% on cell and reset direction
            transform.position = target;
            _direction = Direction.None;
        }
    }

    public void TakeHit(int hits)
    {
        StartCoroutine(DoTheShimmy());

        if (_godMode) return;
        _health -= hits;
        _health = Mathf.Max(0, _health);
        _healthMeter.fillAmount = _health / 10f;
    }

    void Update()
    {
        if (_paused)
        {
            return;
        }

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
        } else if (Input.GetKeyDown(KeyCode.Space) && _touchedChest != null && _tesseracts > 0)
        {
            _tesseracts--;
            _grenades += _touchedChest.GetComponent<Chest>().OpenChest();
            _grenadeIndicator.text = _grenades.ToString();
            _touchedChest = null;
        }
    }

    private bool EnemyInSight()
    {
        // Only ever called if direction is valid so no need to check that here
        var dir = _vectors[_direction];
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, 10);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Wall")
                {
                    return false;
                }
                else if (hit.collider.tag == "Enemy")
                {
                    _enemyTarget = hit.collider.gameObject;
                    return true;
                }
            }
        }

        return false;
    }


    private IEnumerator DoTheShimmy()
    {
        float shimmyTime = 0f;
        var pos = Clamp();
        while (shimmyTime < 1f)
        {
            transform.position = pos + Shimmy();
            shimmyTime += Time.deltaTime * 10f;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        transform.position = pos;
    }

    private Vector3 Clamp()
    {
        var x = Mathf.RoundToInt(transform.position.x);
        var y = Mathf.RoundToInt(transform.position.y);
        return new Vector3(x, y, 0);
    }

    private Vector3 Shimmy()
    {
        var vec = Random.insideUnitCircle / 10f;
        return new Vector3(vec.x, vec.y, 0);
    }
}
