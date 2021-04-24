using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Map _map;
    public GameObject _enemyPrefab;

    private int _maxEnemies = 2;

    public void GenerateEnemies()
    {
        // Get rid of previous enemies
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        int[] usableBlocks = GetUsableBlocks();
        List<int> used = new List<int>();
        for (int i = 0; i < _maxEnemies; i++)
        {
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
        }
    }


    public IEnumerator Move()
    {
        yield return null;
    }

    private int[] GetUsableBlocks()
    {
        List<int> blocks = new List<int>();
        for(int i =0;i < _map._blocks.Length; i++)
        {
            if (_map._blocks[i] == BlockType.Walk && 
                i != _map._start && 
                i != _map._exit && 
                !_map._tesseracts.Contains(i) &&
                !_map._chests.Contains(i))
            {
                blocks.Add(i);
            }
        }

        return blocks.ToArray();
    }
}
