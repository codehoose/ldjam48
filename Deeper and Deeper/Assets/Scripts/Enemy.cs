using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int _health = 3;

    public bool TakeHit()
    {
        _health--;
        if (_health == 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
