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

    public void sceneChange(string Where) 
    {
        switch (Where) 
        {
            case "Jogo":
                SceneManager.LoadScene("Loading");
                break;
            case "Credits":
                SceneManager.LoadScene("Credits");
                break;
            case "Sair":
                Application.Quit();
                break;
            case "Tut":
                SceneManager.LoadScene("Tutorial");
                break;
        }
    }

    IEnumerator TimeSec(string Cena)
    {
        yield return new WaitForSeconds(0.8f);

        FadeControler.Fall = false;
        print("Indo para: " + Cena);
    }
}
