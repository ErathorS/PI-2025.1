using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class TutorialSlide : MonoBehaviour
{
    public bool Slide1 = true;
    public GameObject Slide1Obj, Slide2Obj;
    public void Next() 
    {
        if (Slide1)
        {
            Slide1 = false;
        }
        else 
        {
            Slide1 = true;
        }
        switch (Slide1) 
        {
            case true:
                Slide1Obj.SetActive(true);
                Slide2Obj.SetActive(false);
                break;
            case false:
                Slide1Obj.SetActive(false);
                Slide2Obj.SetActive(true);
                break;
        }
    }
}
