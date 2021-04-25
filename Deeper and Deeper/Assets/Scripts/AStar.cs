using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Adapted from https://gist.github.com/DotNetCoreTutorials/08b0210616769e81034f53a6a420a6d9.
/// </summary>
public class AStar
{
    private int[] _map;
    private int _width;
    private int _height;

    public AstarTile NextTile { get; set; }

    public AStar(int width, int height)
    {
        // Create the map and fill in the occupied cells with 1s. Unoccupied (walkable) cells 
        // are 0s.
        _map = new int[width * height];
        _width = width;
        _height = height;
    }

    public bool WalkToTarget(int sx, int sy, int fx, int fy, int[] usable)
    {
        NextTile = null;

        Array.Clear(_map, 0, _map.Length);

        for (int i = 0; i < _map.Length; i++)
        {
            if (!usable.Contains(i))
            {
                _map[i] = 1;
            }
        }

        // Set the start to walkable ;)
        _map[sy * 8 + sx] = 0;

        var start = new AstarTile() { X = sx, Y = sy };
        var finish = new AstarTile() { X = fx, Y = fy };

        start.SetDistance(finish.X, finish.Y);

        var activeTiles = new List<AstarTile>();
        activeTiles.Add(start);
        var visitedTiles = new List<AstarTile>();

        while (activeTiles.Any())
        {
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

            if (checkTile != null && checkTile.X == finish.X && checkTile.Y == finish.Y)
            {
                // Walk back through the tiles to the tile AFTER the start.
                // That's going to be our next tile
                var tile = checkTile;
                while (tile?.Parent != null && tile?.Parent != start)
                {
                    tile = tile?.Parent;
                }

                NextTile = tile;
                // Only return true if we have a tile and it's NOT the player's tile
                return NextTile != null && NextTile.X != fx && NextTile.Y != fy;
            }

            visitedTiles.Add(checkTile);
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableTiles(_map, checkTile, finish);

            foreach (var walkableTile in walkableTiles)
            {
                //We have already visited this tile so we don't need to do so again!
                if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    continue;

                //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                {
                    var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                    if (existingTile.CostDistance > checkTile.CostDistance)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                }
                else
                {
                    //We've never seen this tile before so add it to the list. 
                    activeTiles.Add(walkableTile);
                }
            }
        }

        // No path found
        return false;
    }


    private List<AstarTile> GetWalkableTiles(int[] map, AstarTile currentTile, AstarTile targetTile)
    {
        var possibleTiles = new List<AstarTile>()
            {
                new AstarTile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new AstarTile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
                new AstarTile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new AstarTile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

        possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

        return possibleTiles
                .Where(tile => tile.X >= 0 && tile.X < _width)
                .Where(tile => tile.Y >= 0 && tile.Y < _height)
                .Where(tile => map[tile.Y * 8 + tile.X] == 0) // || tile.Y =   // map[tile.Y * 8 + tile.X] == 'B')
                .ToList();
    }

    public string Debug()
    {
        var s = "";
        for (int i = 0; i < _map.Length; i++)
        {
            s += _map[i] == 0 ? "0" : "X";
            if (((i + 1) % _width) == 0)
            {
                s += "\r\n";
            }
        }

        return s;
    }
}

public class AstarTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Cost { get; set; }
    public int Distance { get; set; }
    public int CostDistance => Cost + Distance;
    public AstarTile Parent { get; set; }

    //The distance is essentially the estimated distance, ignoring walls to our target. 
    //So how many tiles left and right, up and down, ignoring walls, to get there. 
    public void SetDistance(int targetX, int targetY)
    {
        Distance = Mathf.Abs(targetX - X) + Mathf.Abs(targetY - Y);
    }
}

