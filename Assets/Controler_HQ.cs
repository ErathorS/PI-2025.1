using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controler_HQ : MonoBehaviour
{
    public int Panel = 1;
    public GameObject HQ2, HQ3, HQ4, HQ5, HQ6;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Panel++;
        }

        switch (Panel) 
        {
            case 2:
                HQ2.SetActive(true);
                break;
            case 3:
                HQ3.SetActive(true);
                break;
            case 4:
                HQ4.SetActive(true);
                break;
            case 5:
                HQ5.SetActive(true);
                break;
            case 6:
                HQ6.SetActive(true);
                break;
        }

    }
}
