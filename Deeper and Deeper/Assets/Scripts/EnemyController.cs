using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Map _map;
    public GameObject _enemyPrefab;
    public GameObject _enemySpawnPrefab;
    private List<GameObject> _spawned;
    private List<EnemySpawner> _spawners;

    private int _maxEnemies = 2;
    private int _playerMoveCount;

    [Tooltip("The number of moves the player makes that causes new enemies to spawn")]
    [HideInInspector]
    private int _spawnAfterMoves = 5;

    void Awake()
    {
        _spawned = new List<GameObject>();
        _spawners = new List<EnemySpawner>();
    }

    public void HitEnemy(GameObject enemy)
    {
        if (enemy.GetComponent<Enemy>().TakeHit())
        {
            _spawned.Remove(enemy);
        }
    }

    public void ClearEnemies()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    public void PlayerMoved()
    {
        _playerMoveCount++;
        UpdateSpawners();
        ShouldWeGenerateEnemy();
    }

    private void ShouldWeGenerateEnemy()
    {
        if ((_playerMoveCount %= _spawnAfterMoves) == 0 && _spawned.Count < 3) // Max 4 enemies
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

        for (int i = 0; i < _maxEnemies; i++)
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
        yield return null;
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
