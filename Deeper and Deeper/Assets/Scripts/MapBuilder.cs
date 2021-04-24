using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    // Takes in an array of BlockType and renders out the blocks to the screen.
    // Each block is a prefab (ugh) but what are you going to do, it's a jam :P

    private Dictionary<BlockType, GameObject> _prefabs;

    public GameObject walk;
    public GameObject wall;
    public GameObject chest;
    public GameObject playerSpawn;
    public GameObject tesseract;
    public GameObject exit;

    void Awake()
    {
        _prefabs = new Dictionary<BlockType, GameObject>();
        _prefabs.Add(BlockType.Walk, walk);
        _prefabs.Add(BlockType.Chest, chest);
        _prefabs.Add(BlockType.PlayerSpawn, playerSpawn);
        _prefabs.Add(BlockType.Exit, exit);
        _prefabs.Add(BlockType.Tesseract, tesseract);
        _prefabs.Add(BlockType.Wall, wall);
    }

    public void BuildMap(Map map)
    {
        // Get rid of previous blocks
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < map._blocks.Length; i++)
        {
            var y = i / 8;
            var x = i % 8;

            var blockType = map._blocks[i];
            if (map._exit == i)
                blockType = BlockType.Exit;

            var copy = Instantiate(_prefabs[blockType], transform);
            copy.transform.position = Vector3.zero;
            copy.transform.localPosition = new Vector3(x, -y, 0);

            if (map._tesseracts.Contains(i))
            {
                var tess = Instantiate(_prefabs[BlockType.Tesseract], transform);
                tess.transform.position = Vector3.zero;
                tess.transform.localPosition = new Vector3(x, -y, 0);
            }
        }
    }
}
