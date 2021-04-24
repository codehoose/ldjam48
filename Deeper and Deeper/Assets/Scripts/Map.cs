using System.Collections.Generic;

public class Map
{
    public BlockType[] _blocks;

    public List<int> _tesseracts;

    public List<int> _chests;

    public int _start;

    public int _exit;

    public Map()
    {
        _tesseracts = new List<int>();
        _chests = new List<int>();
    }
}