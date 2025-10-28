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

    // 🔹 Mantém referências únicas locais entre cenas
    private static GameObject _localUI;
    private static GameObject _localCamera;

    void Awake()
    {
        // Evita múltiplas instâncias do GameManager
        if (FindObjectsOfType<NetworkGameManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Garante sincronização automática de cenas entre jogadores
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("❌ Não conectado ao Photon!");
            return;
        }

        string cenaAtual = SceneManager.GetActiveScene().name;
        Debug.Log($"[NetworkGameManager] Cena iniciada: {cenaAtual}");
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

            // Delay pequeno pra garantir que tudo foi carregado
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

        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;
        GameObject chosenPrefab = actorID == 1 ? player1Prefab : player2Prefab;

        string spawnTag = actorID == 1 ? "Spawn 1" : "Spawn 2";
        GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnTag);

        if (spawnObj == null)
        {
            Debug.LogError($"[NetworkGameManager] Nenhum objeto com a tag '{spawnTag}' encontrado!");
            return;
        }

        Vector3 spawnPos = spawnObj.transform.position;
        Quaternion spawnRot = spawnObj.transform.rotation;

        // Instancia o jogador em rede
        GameObject player = PhotonNetwork.Instantiate(chosenPrefab.name, spawnPos, spawnRot);

        // Marca o identificador do jogador
        PlayerIdentifier identifier = player.GetComponent<PlayerIdentifier>();
        if (identifier != null)
            identifier.actorID = actorID;

        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            // ============================
            // 🔹 UI ÚNICA LOCAL
            // ============================
            if (_localUI == null)
            {
                _localUI = Instantiate(playerUiPrefab);
                DontDestroyOnLoad(_localUI);

                Canvas canvas = _localUI.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    canvas.gameObject.tag = actorID == 1 ? "CanvasP1" : "CanvasP2";
                    Debug.Log($"[NetworkGameManager] Canvas criado com tag: {canvas.gameObject.tag}");
                }
            }
            else
            {
                Debug.Log($"[NetworkGameManager] UI já existente — reutilizando o Canvas atual");
            }

            // ============================
            // 🔹 CÂMERA ÚNICA LOCAL
            // ============================
            if (_localCamera == null)
            {
                _localCamera = Instantiate(cameraPrefab);
                DontDestroyOnLoad(_localCamera);
                Debug.Log($"[NetworkGameManager] Câmera criada e persistente.");
            }
            else
            {
                Debug.Log($"[NetworkGameManager] Câmera já existente — reutilizando a atual");
            }

            // Vincula a câmera ao player
            CameraIsometricaComRotacao camScript = _localCamera.GetComponent<CameraIsometricaComRotacao>();
            if (camScript != null)
            {
                camScript.player = player.transform;
            }

            // Busca o joystick e vincula ao player
            FixedJoystick joystick = _localUI.GetComponentInChildren<FixedJoystick>();
            MovimentacaoIsometrica mov = player.GetComponent<MovimentacaoIsometrica>();

            if (mov != null && joystick != null && camScript != null)
            {
                mov.ConfigurarReferencias(joystick, camScript.transform);
                Debug.Log($"[NetworkGameManager] Joystick e câmera configurados com sucesso para Player {actorID}");
            }
            else
            {
                Debug.LogWarning("[NetworkGameManager] ⚠️ Referências não foram configuradas corretamente!");
            }

            // Desativa painel de diálogo no início
            PlayerUIReferences uiRefs = _localUI.GetComponent<PlayerUIReferences>();
            if (uiRefs != null && uiRefs.painelDialogo != null)
                uiRefs.painelDialogo.SetActive(false);
        }

        hasSpawned = true;
        Debug.Log($"[NetworkGameManager] Player {actorID} spawnado com sucesso");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[NetworkGameManager] Jogador entrou na sala: {newPlayer.NickName}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[NetworkGameManager] Jogador saiu da sala: {otherPlayer.NickName}");
    }
}
