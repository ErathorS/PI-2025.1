using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Referência do AudioSource")]
    public AudioSource musicSource;

    [Header("Músicas")]
    public AudioClip menuMusic;  
    public AudioClip fase1Music;  
    
    public AudioClip fase2Music; 
    public AudioClip fase3Music; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrocarMusicaPorCena(scene.name);
    }

    public void TrocarMusicaPorCena(string sceneName)
    {
        AudioClip novaMusica = null;

        if (sceneName == "MenuJogo" || sceneName == "Loading" || sceneName == "Lobby" || sceneName == "Credits" || sceneName == "Tutorial" || sceneName == "Tutorial2")
        {
            novaMusica = menuMusic;
        }
        else if (sceneName == "Cena de Introducao 1" || sceneName == "PI Fase 1")
        {
            novaMusica = fase1Music;
        }
        else if (sceneName == "Cena de Introducao 2" || sceneName == "PI Fase 2")
        {
            novaMusica = fase2Music;
        }
        else if (sceneName == "Cena de Introducao 3" || sceneName == "PI Fase 3")
        {
            novaMusica = fase3Music;
        }

        if (novaMusica != null && musicSource != null && musicSource.clip != novaMusica)
        {
            musicSource.clip = novaMusica;
            musicSource.Play();
        }
    }
}
