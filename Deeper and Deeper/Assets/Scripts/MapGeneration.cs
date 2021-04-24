using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public int _maxChests = 3;
    public int _maxTess = 5;

    private float _wallFudgeFactor = 0.95f;

    public Map Generate()
    {
        int[] startpos = new int[] { 0, 7, 56, 63 };
        var map = new Map();

        // General blocks - walk and walls
        var blocks = new BlockType[64];
        for (int i = 0; i < blocks.Length; i++)
        {
            if (Random.Range(0f, 1) >= _wallFudgeFactor)
                blocks[i] = BlockType.Wall;
            else
                blocks[i] = BlockType.Walk;
        }

        // Random smattering of chests
        for (int i = 0; i < _maxChests; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, 64);
            } while (startpos.Contains(index));
            map._chests.Add(index);
            blocks[index] = BlockType.Chest;
        }

        // Random smattering of tesseracts but don't overwrite the chests
        for (int i = 0; i < _maxTess; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, 64);
            } while (blocks[index] == BlockType.Chest && startpos.Contains(index));

            map._tesseracts.Add(index);
            // If it was a wall, make it walkable
            blocks[index] = BlockType.Walk;
        }

        // The start and exit blocks
        map._start = startpos[Random.Range(0, 4)];

        int exit;
        do
        {
            exit = startpos[Random.Range(0, 4)];
        } while (exit == map._start || map._tesseracts.Contains(exit));
        map._exit = exit;

        map._blocks = blocks;
        return map;
    }
}
