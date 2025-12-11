using UnityEngine;
using UnityEngine.SceneManagement;


public class ByeScript : MonoBehaviour
{
    public void Sair() 
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void Voltar()
    {
        SceneManager.LoadScene("MenuJogo");
    }
}