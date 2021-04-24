using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerMovement _playerMovement;

    public EnemyController _enemyController;

    public MapGeneration _mapGeneration;

    public MapBuilder _mapBuilder;

    public bool _isRunning = true;

    IEnumerator Start()
    {
        InitSequence();

        //var map = _mapGeneration.Generate();
        //_mapBuilder.BuildMap(map);
        //_playerMovement._collisions = map._blocks;
        //_playerMovement.transform.position = GetPlayerStart(map);

        //_enemyController._map = map;
        //_enemyController.GenerateEnemies();

        while (_isRunning)
        {
            yield return _playerMovement.Move();
            yield return _enemyController.Move();

            if (_playerMovement._doExitSequence)
            {
                yield return ExitSequence();
                // TODO: Load new map
            }
        }
    }

    private IEnumerator ExitSequence()
    {
        // Make it a little more difficult each level.
        _enemyController._maxEnemies++;
        if ((_enemyController._maxEnemies % 5) == 0)
        {
            _enemyController._maxEnemiesAtStart += 2;
        }

        InitSequence();

        yield return null;
    }

    private void InitSequence()
    {
        _enemyController.ClearEnemies();
        _playerMovement.Reset();

        var map = _mapGeneration.Generate();
        _mapBuilder.BuildMap(map);
        _playerMovement._collisions = map._blocks;
        _playerMovement.transform.position = GetPlayerStart(map);

        _enemyController._map = map;
        _enemyController.GenerateEnemies();
    }

    private Vector3 GetPlayerStart(Map map)
    {
        var x = map._start % 8;
        var y = -map._start / 8;
        return new Vector3(x, y, 0);
    }
}
