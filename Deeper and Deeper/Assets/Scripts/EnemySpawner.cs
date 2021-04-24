using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector]
    public int _playerMoved;

    [HideInInspector]
    public int _spawnMoves;

    public SpriteRenderer _renderer;

    [HideInInspector]
    public GameObject enemy;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            if (_playerMoved == 1)
            {
                _renderer.enabled = true;
            }
            else if (_playerMoved == _spawnMoves && enemy != null)
            {
                enemy.SetActive(true);
            }

            yield return null;
        }
    }
}
