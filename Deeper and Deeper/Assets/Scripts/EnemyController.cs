using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Map _map;
    public GameObject _enemyPrefab;
    public GameObject _enemySpawnPrefab;

    private List<EnemyMove> _enemyMovements;
    private List<GameObject> _spawned;
    private List<EnemySpawner> _spawners;
    private List<GameObject> _stunnedEnemies;

    [HideInInspector]
    public int _maxEnemiesAtStart = 2;
    public int _maxEnemies = 3;
    private int _playerMoveCount;
    private AStar _astar;

    [Tooltip("The number of moves the player makes that causes new enemies to spawn")]
    [HideInInspector]
    private int _spawnAfterMoves = 5;


    void Awake()
    {
        _spawned = new List<GameObject>();
        _spawners = new List<EnemySpawner>();
        _enemyMovements = new List<EnemyMove>();
        _stunnedEnemies = new List<GameObject>();
        _astar = new AStar(8, 8);
    }

    // Calculate the number of enemies near the player in the cardinal NSEW directions only
    public int EnemiesNearPlayer(Vector3 pos)
    {
        return _spawned.Select(go => go.transform.position)
                       .Where(p => p + Vector3.up == pos ||
                              p + Vector3.down == pos ||
                              p + Vector3.left == pos ||
                              p + Vector3.right == pos)
                       .Count();
    }

    public void HitEnemy(GameObject enemy)
    {
        if (enemy.GetComponent<Enemy>().TakeHit())
        {
            _spawned.Remove(enemy);
        }
        else
        {
            if (!_stunnedEnemies.Contains(enemy))
                _stunnedEnemies.Add(enemy);
        }
    }

    public void ClearEnemies()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        foreach (var spawn in _spawners)
        {
            Destroy(spawn.gameObject);
        }

        _spawned.Clear();
        _spawners.Clear();
        _enemyMovements.Clear();
    }

    public void PlayerMoved(PlayerMovement player)
    {
        _playerMoveCount++;
        UpdateSpawners();
        ShouldWeGenerateEnemy();

        var x = Mathf.RoundToInt(player.transform.position.x);
        var y = -Mathf.RoundToInt(player.transform.position.y);

        foreach (var enemy in _spawned)
        {
            if (enemy.activeInHierarchy)
            {
                var sx = (int)enemy.transform.position.x;
                var sy = -(int)enemy.transform.position.y;

                if (_astar.WalkToTarget(sx, sy, x, y, GetUsableBlocks()))
                {
                    _enemyMovements.Add(new EnemyMove(enemy, _astar.NextTile.X, _astar.NextTile.Y));
                }
            }
        }

        var enemiesNearPlayer = EnemiesNearPlayer(new Vector3(x, -y, 0));
        if (enemiesNearPlayer > 0)
        {
            player.TakeHit(enemiesNearPlayer);
        }
    }

    private void ShouldWeGenerateEnemy()
    {
        if ((_playerMoveCount %= _spawnAfterMoves) == 0 && _spawned.Count < _maxEnemies)
        {
            GenerateEnemy();
        }
    }

    private void UpdateSpawners()
    {
        int i = 0;
        while (i < _spawners.Count)
        {
            _spawners[i]._playerMoved++;
            if (_spawners[i]._playerMoved == _spawners[i]._spawnMoves)
            {
                Destroy(_spawners[i].gameObject);
                _spawners.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    public void GenerateEnemies()
    {
        // Get rid of previous enemies
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _maxEnemiesAtStart; i++)
        {
            GenerateEnemy();
        }
    }

    private void GenerateEnemy()
    {
        List<int> used = new List<int>();
        int[] usableBlocks = GetUsableBlocks();

        int index;
        do
        {
            index = usableBlocks[Random.Range(0, usableBlocks.Length)];
        } while (used.Contains(index));

        var x = index % 8;
        var y = index / 8;

        var copy = Instantiate(_enemyPrefab, transform);
        copy.transform.position = Vector3.zero;
        copy.transform.localPosition = new Vector3(x, -y, 0);
        copy.SetActive(false);

        var spawner = Instantiate(_enemySpawnPrefab, transform);
        spawner.transform.position = Vector3.zero;
        spawner.transform.localPosition = new Vector3(x, -y, 0);
        var sp = spawner.GetComponent<EnemySpawner>();
        sp._spawnMoves = Random.Range(2, 4);
        sp.enemy = copy;
        _spawners.Add(sp);

        _spawned.Add(copy);
    }

    public IEnumerator Move()
    {
        var _speed = 5;
        
        float time = 0;
        while (time < 1f)
        {
            foreach (var em in _enemyMovements)
            {
                if (_stunnedEnemies.Contains(em.GameObject))
                {
                    // Don't move stunned enemies
                    continue;
                }

                var start = em.GameObject.transform.position;
                var target = new Vector3(em.X, -em.Y, 0);

                em.GameObject.transform.position = Vector3.Lerp(start, target, time);
            }
            time += Time.deltaTime * _speed;
            yield return null;
        }

        // Stunned enemies are only stunned when they're shot
        _stunnedEnemies.Clear();

        _enemyMovements.Clear();
    }

    private int[] GetUsableBlocks()
    {
        List<int> blocks = new List<int>();
        var enemies = EnemiesToIndexInGrid();
        for (int i = 0; i < _map._blocks.Length; i++)
        {
            if (_map._blocks[i] == BlockType.Walk &&
                i != _map._start &&
                i != _map._exit &&
                !_map._tesseracts.Contains(i) &&
                !_map._chests.Contains(i) &&
                !enemies.Contains(i))
            {
                blocks.Add(i);
            }
        }

        return blocks.ToArray();
    }

    private int[] EnemiesToIndexInGrid()
    {
        List<int> ints = new List<int>();
        foreach (var enemy in _spawned)
        {
            var p = enemy.transform.position;
            var index = (int)(-p.y * 8 + p.x * 8);
            ints.Add(index);
        }

        return ints.ToArray();
    }
}
