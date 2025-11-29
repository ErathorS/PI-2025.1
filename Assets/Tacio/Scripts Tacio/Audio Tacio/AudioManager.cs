using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Referência do AudioSource")]
    public AudioSource musicSource;

    [Header("Músicas")]
    public AudioClip menuMusic;   // Menu, Load, Lobby
    public AudioClip fase1Music;  // Fase 1
    
    public AudioClip fase2Music;  // Fase 2
    public AudioClip fase3Music;  // Fase 3

    private void Awake()
    {
        // Singleton: garante que só exista um AudioManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Garante que temos um AudioSource
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

    // Chamado sempre que uma cena termina de carregar
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrocarMusicaPorCena(scene.name);
    }

    public void TrocarMusicaPorCena(string sceneName)
    {
        AudioClip novaMusica = null;

        // Ajuste os nomes das cenas de acordo com o que está no Build Settings
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

        // Se encontrou uma música e ela é diferente da atual, troca
        if (novaMusica != null && musicSource != null && musicSource.clip != novaMusica)
        {
            musicSource.clip = novaMusica;
            musicSource.Play();
        }
    }
}
