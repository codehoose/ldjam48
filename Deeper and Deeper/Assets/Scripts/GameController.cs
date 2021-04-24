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
        var map = _mapGeneration.Generate();
        _mapBuilder.BuildMap(map);
        _playerMovement._collisions = map;

        while (_isRunning)
        {
            yield return _playerMovement.Move();
            yield return _enemyController.Move();
        }
    }
}
