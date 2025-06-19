using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FadeControler : MonoBehaviour
{
    static public bool Fall = false;

    private void Update()
    {
        if (Fall && transform.position.y > 20) 
        {
            transform.Translate(0, -0.35f, 0);
        }
        else 
        {
            Fall = false;
        }
    }

}
