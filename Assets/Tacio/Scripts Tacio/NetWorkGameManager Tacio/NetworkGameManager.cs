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

    [Header("Cenas jogáveis (onde o player deve nascer)")]
    [SerializeField]
    private string[] cenasJogaveis = { "Cena de Introducao 1", "PI Fase 1", "ProximaFase" }; // ADICIONE A CENA FINAL AQUI

    private static GameObject _localUI;
    private static GameObject _localCamera;

    void Awake()
    {
        if (FindObjectsOfType<NetworkGameManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Não conectado ao Photon! (Se der Play direto nessa cena, é normal não spawnear nada)");
            return;
        }

        string cenaAtual = SceneManager.GetActiveScene().name;
        Debug.Log($"[NetworkGameManager] Cena iniciada: {cenaAtual}");
    }

    private bool CenaEhJogavel(string nomeCena)
    {
        foreach (var c in cenasJogaveis)
        {
            if (c == nomeCena)
                return true;
        }
        return false;
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
        Debug.Log($"[NetworkGameManager] Cena carregada: {nomeCena}");

        hasSpawned = false;

        // VERIFICA SE É A CENA FINAL E SE ESTÁ CONECTADO
        if (CenaEhJogavel(nomeCena) && PhotonNetwork.IsConnected)
        {
            Invoke(nameof(SpawnPlayer), 0.3f);
        }
        else
        {
            Debug.Log($"[NetworkGameManager] Cena '{nomeCena}' não é jogável ou não está conectado — sem spawn.");
        }
    }

    void SpawnPlayer()
    {
        if (hasSpawned)
        {
            Debug.LogWarning("[NetworkGameManager] Spawn ignorado — já executado nesta cena.");
            return;
        }

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("[NetworkGameManager] Tentou spawnar sem estar conectado ao Photon.");
            return;
        }

        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;
        GameObject chosenPrefab = actorID == 1 ? player1Prefab : player2Prefab;

        string spawnTag = actorID == 1 ? "Spawn 1" : "Spawn 2";
        GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnTag);

        // SE NÃO ENCONTRAR O SPAWN, TENTA ENCONTRAR QUALQUER SPAWN DISPONÍVEL
        if (spawnObj == null)
        {
            Debug.LogWarning($"[NetworkGameManager] Nenhum objeto com a tag '{spawnTag}' encontrado. Tentando encontrar qualquer spawn...");

            // Procura por spawns alternativos
            GameObject[] allSpawns = GameObject.FindGameObjectsWithTag("Spawn 1");
            if (allSpawns.Length == 0)
            {
                allSpawns = GameObject.FindGameObjectsWithTag("Spawn 2");
            }

            if (allSpawns.Length > 0)
            {
                spawnObj = allSpawns[0];
                Debug.Log($"[NetworkGameManager] Usando spawn alternativo: {spawnObj.tag}");
            }
            else
            {
                Debug.LogError($"[NetworkGameManager] Nenhum spawn encontrado na cena {SceneManager.GetActiveScene().name}!");
                return;
            }
        }

        Vector3 spawnPos = spawnObj.transform.position;
        Quaternion spawnRot = spawnObj.transform.rotation;

        GameObject player = PhotonNetwork.Instantiate(chosenPrefab.name, spawnPos, spawnRot);

        PlayerIdentifier identifier = player.GetComponent<PlayerIdentifier>();
        if (identifier != null)
            identifier.actorID = actorID;

        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
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
                Debug.Log("[NetworkGameManager] UI já existente — reutilizando o Canvas atual");
            }

            if (_localCamera == null)
            {
                _localCamera = Instantiate(cameraPrefab);
                DontDestroyOnLoad(_localCamera);
                Debug.Log("[NetworkGameManager] Câmera criada e persistente.");
            }
            else
            {
                Debug.Log("[NetworkGameManager] Câmera já existente — reutilizando a atual");
            }

            CameraIsometricaComRotacao camScript = _localCamera.GetComponent<CameraIsometricaComRotacao>();
            if (camScript != null)
            {
                camScript.player = player.transform;
            }

            FixedJoystick joystick = _localUI.GetComponentInChildren<FixedJoystick>();
            MovimentacaoIsometrica mov = player.GetComponent<MovimentacaoIsometrica>();

            if (mov != null && joystick != null && camScript != null)
            {
                mov.ConfigurarReferencias(joystick, camScript.transform);
                Debug.Log($"[NetworkGameManager] Joystick e câmera configurados com sucesso para Player {actorID}");
            }
            else
            {
                Debug.LogWarning("[NetworkGameManager] Referências não foram configuradas corretamente!");
            }
            PlayerUIReferences uiRefs = _localUI.GetComponent<PlayerUIReferences>();
            if (uiRefs != null && uiRefs.painelDialogo != null)
                uiRefs.painelDialogo.SetActive(false);
        }

        hasSpawned = true;
        Debug.Log($"[NetworkGameManager] Player {actorID} spawnado com sucesso na cena {SceneManager.GetActiveScene().name}");
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