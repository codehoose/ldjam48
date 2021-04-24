using UnityEngine;

public class EnemyMove
{
    public GameObject GameObject { get; }
    public int X { get; }
    public int Y { get; }

    public EnemyMove(GameObject go, int x, int y)
    {
        GameObject = go;
        X = x;
        Y = y;
    }
}