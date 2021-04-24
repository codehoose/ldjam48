using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuyLoco : MonoBehaviour
{
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var horiz = Input.GetAxisRaw("Horizontal"); // * Time.deltaTime;
        var vert = Input.GetAxisRaw("Vertical"); // * Time.deltaTime;

        var velocity = new Vector3(1, 0, 0) * horiz * 5;
        velocity += new Vector3(0, 1, 0) * vert * 5;

        _rb.velocity = velocity;


        //if (Mathf.Abs(horiz) < float.Epsilon && Mathf.Abs(vert) < float.Epsilon)
        //{
        //    _rb.velocity = Vector2.zero;
        //}
        //else
        //{
        //    _rb.AddForce(new Vector2(horiz, vert));
        //}
    }
}
