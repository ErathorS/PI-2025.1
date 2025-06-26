using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo_bounce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 4f;
        float value = Mathf.Sin(Time.time * speed);
        float value2 = Mathf.Sin(Time.time * speed/2);
        transform.Translate(0, value/100, 0);
        transform.Rotate(0, 0, value2/100);
    }
}
