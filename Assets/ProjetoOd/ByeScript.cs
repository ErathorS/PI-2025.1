using UnityEngine;

public class ByeScript : MonoBehaviour
{
    public void Sair() 
    {
        Debug.Log("[ByeScript] Encerrando aplicação...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}