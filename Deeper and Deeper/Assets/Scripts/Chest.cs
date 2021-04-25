using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject _openChestPrefab;

    public void OpenChest()
    {
        var copy = Instantiate(_openChestPrefab, transform.parent);
        copy.transform.position = transform.position;
        Destroy(gameObject);
    }
}
