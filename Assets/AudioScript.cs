using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioScript : MonoBehaviour
{
    public AudioSource SongArea1;
    public AudioSource SongArea2;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Mapa2") 
        {
            SongArea2.volume = 0.026f;
        }
        if (SceneManager.GetActiveScene().name == "PI Diego")
        {
            SongArea1.volume = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
