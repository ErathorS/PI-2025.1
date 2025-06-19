using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void FadeOut(string Cena) 
    {
        FadeControler.Fall = true;
        StartCoroutine(TimeSec(Cena));
        
    }

    public void SairJogo() 
    {
        Application.Quit();
    }

    IEnumerator TimeSec(string Cena)
    {
        yield return new WaitForSeconds(0.8f);

        FadeControler.Fall = false;
        print("Indo para: " + Cena);
    }
}
