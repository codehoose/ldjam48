using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TmxMapBuilder : MonoBehaviour
{
    private Tile[] _tiles;

    public TextAsset mapFile;
    public Tilemap tilemap;

    public string spriteSheetName;


    private void Start()
    {
        Build();
    }

    private Tile CreateTile(Sprite sprite)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        return tile;
    }

    public void Build()
    {
        _tiles = Resources.LoadAll<Sprite>(spriteSheetName)
                          .Select(CreateTile)
                          .ToArray();

        // HACK: All 1st column tiles are walkable.
        for (var i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].colliderType = i % 10 == 0 ? Tile.ColliderType.None : Tile.ColliderType.Grid;
        }

        var tmxFile = JsonUtility.FromJson<TmxFile>(mapFile.text);

        tilemap.ClearAllEditorPreviewTiles();

        for (var y = 0; y < 9; y++)
        {
            for (var x = 0; x < 16; x++)
            {
                var idx = (8 - y) * 16 + x;
                var id = tmxFile.layers[0].data[idx] - 1;
                if (id >=0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), _tiles[id]);
                }
            }
        }
    }
}
