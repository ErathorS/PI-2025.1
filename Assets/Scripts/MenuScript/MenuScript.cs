using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void FadeOut(string Cena) 
    {
        FadeControler.Fall = true;
        StartCoroutine(TimeSec(Cena));
        
    }


    public void Jogar()
    {
        SceneManager.LoadScene("Loading");
    }public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void Credits()
    {
         SceneManager.LoadScene("Credits");
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
