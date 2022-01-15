using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pusher : MonoBehaviour
{
    public float force = 50f;
    private float startTime = 0;
    private float delayTime = 6f;

    public void setText(string text) {
        GetComponent<Text>().text = text;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(force, 0));
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime > delayTime)
        {
            Destroy(gameObject);
        }
    }
}
