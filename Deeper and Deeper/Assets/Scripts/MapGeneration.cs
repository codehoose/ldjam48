using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public BlockType[] Generate()
    {
        var blocks = new BlockType[64];
        for (int i = 0; i < blocks.Length; i++)
        {
            if (UnityEngine.Random.Range(0f, 1) > 0.8f)
                blocks[i] = BlockType.Wall;
            else
                blocks[i] = BlockType.Walk;
        }

        blocks[0] = BlockType.Walk;

        return blocks;
    }
}
