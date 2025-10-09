using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ByeScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void Sair() 
    {
        SceneManager.LoadScene("MenuJogo");
    }
}
