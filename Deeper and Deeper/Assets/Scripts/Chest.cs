using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject _openChestPrefab;

    public GameObject _chestRewardPrefab;

    public int OpenChest() // returns the number of grenades (0 or 1)
    {
        var grenades = Random.Range(0, 1f) > 0.8f ? 1 : 0;

        var indicator = Instantiate(_chestRewardPrefab, transform.parent);
        indicator.transform.position = transform.position;
        indicator.GetComponent<ChestRewardIndicator>().Init(grenades == 0);

        var copy = Instantiate(_openChestPrefab, transform.parent);
        copy.transform.position = transform.position;
        Destroy(gameObject);

        return grenades;
    }
}
