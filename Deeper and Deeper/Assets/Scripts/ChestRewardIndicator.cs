using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRewardIndicator : MonoBehaviour
{
    public Sprite[] _sprites;


    public SpriteRenderer _renderer;

    public void Init(bool isNothing)
    {
        var sprite = isNothing ? _sprites[0] : _sprites[1];

        StartCoroutine(ShowIndicator(sprite));
    }

    IEnumerator ShowIndicator(Sprite sprite)
    {
        _renderer.sprite = sprite;
        _renderer.enabled = true;

        var transparent = new Color(1, 1, 1, 0);
        var target = transform.position + Vector3.up * 0.5f;
        var start = transform.position;

        float time = 0f;
        while (time < 1f)
        {
            var color = Color.Lerp(Color.white, transparent, time);
            transform.position = Vector3.Lerp(start, target, time);
            time += Time.deltaTime * 2f;
            yield return null;
        }

        Destroy(gameObject);
    }
}
