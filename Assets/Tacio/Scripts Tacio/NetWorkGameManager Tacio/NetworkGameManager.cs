using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{
    private bool hasSpawned = false;

    [Header("Prefabs dos Jogadores")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    [Header("Outros Prefabs")]
    public GameObject cameraPrefab;    
    public GameObject playerUiPrefab;  
    public Transform[] spawnPoints;    

    void Awake()
    {
        // Evita múltiplas instâncias do GameManager
        if (FindObjectsOfType<NetworkGameManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // 🔹 Garante sincronização automática de cenas
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Não conectado ao Photon!");
            return;
        }

        // 🔹 Deixa o Spawn ser controlado apenas pelo OnSceneLoaded
        string cenaAtual = SceneManager.GetActiveScene().name;
        Debug.Log($"[NetworkGameManager] Iniciando cena: {cenaAtual}. Aguardando evento OnSceneLoaded para spawn...");
    }

    private bool CenaEhJogavel(string nomeCena)
    {
        // Liste aqui as cenas onde os jogadores devem ser instanciados
        return nomeCena == "Cena de Introducao" || nomeCena == "PI Fase 1";
    }

    new void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    new void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string nomeCena = scene.name;

        if (CenaEhJogavel(nomeCena))
        {
            Debug.Log($"[NetworkGameManager] Cena jogável carregada: {nomeCena}");
            hasSpawned = false;

            // Pequeno delay pra garantir que tudo foi carregado
            Invoke(nameof(SpawnPlayer), 0.3f);
        }
        else
        {
            Debug.Log($"[NetworkGameManager] Cena '{nomeCena}' não é jogável — sem spawn.");
        }
    }

    void SpawnPlayer()
    {
        if (hasSpawned)
        {
            Debug.LogWarning("[NetworkGameManager] Spawn ignorado — já executado nesta cena.");
            return;
        }

        // 🔹 Garante que os spawn points usados sejam sempre os da cena atual
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Spawn");
        if (gos != null && gos.Length > 0)
        {
            spawnPoints = new Transform[gos.Length];
            for (int i = 0; i < gos.Length; i++)
                spawnPoints[i] = gos[i].transform;
        }
        else
        {
            Debug.LogError("[NetworkGameManager] Nenhum spawn point com tag 'Spawn' encontrado na cena!");
            return;
        }

        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;
        GameObject chosenPrefab = actorID == 1 ? player1Prefab : player2Prefab;

        int index = (actorID - 1) % spawnPoints.Length;
        Vector3 spawnPos = spawnPoints[index].position;
        Quaternion spawnRot = spawnPoints[index].rotation;

        // Instancia o jogador em rede
        GameObject player = PhotonNetwork.Instantiate(chosenPrefab.name, spawnPos, spawnRot);

        // Marca o identificador do jogador
        PlayerIdentifier identifier = player.GetComponent<PlayerIdentifier>();
        if (identifier != null)
            identifier.actorID = actorID;

        // Instancia UI e câmera apenas para o dono local
        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            // 🔹 Instancia a UI local
            GameObject uiInstance = Instantiate(playerUiPrefab);
            DontDestroyOnLoad(uiInstance);

            // Ajusta tag do Canvas (CanvasP1 / CanvasP2)
            Canvas canvas = uiInstance.GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvas.gameObject.tag = actorID == 1 ? "CanvasP1" : "CanvasP2";

            // Vincula joystick e câmera ao novo player
            MovimentacaoIsometrica mov = player.GetComponent<MovimentacaoIsometrica>();
            if (mov != null)
            {
                FixedJoystick joystick = uiInstance.GetComponentInChildren<FixedJoystick>();
                if (joystick == null)
                    joystick = FindObjectOfType<FixedJoystick>(); // busca caso o prefab não tenha

                if (joystick != null)
                {
                    mov.joystick = joystick;
                    Debug.Log($"[NetworkGameManager] Joystick vinculado ao Player {actorID}");
                }
                else
                {
                    Debug.LogWarning("[NetworkGameManager] Nenhum joystick encontrado na cena!");
                }
            }

            // Instancia câmera e define o alvo
            GameObject cam = Instantiate(cameraPrefab);
            CameraIsometricaComRotacao camScript = cam.GetComponent<CameraIsometricaComRotacao>();
            if (camScript != null)
            {
                camScript.player = player.transform;
                if (mov != null)
                    mov.cameraTransform = camScript.transform;
            }

            // Desativa painel de diálogo no início
            PlayerUIReferences uiRefs = uiInstance.GetComponent<PlayerUIReferences>();
            if (uiRefs != null)
                uiRefs.painelDialogo.SetActive(false);
        }

        hasSpawned = true;

        Debug.Log($"[NetworkGameManager] Player {actorID} spawnado com sucesso na cena {SceneManager.GetActiveScene().name}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) { }
    public override void OnPlayerLeftRoom(Player otherPlayer) { }
}
