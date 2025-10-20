using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{
    private bool hasSpawned = false;

    [Header("Prefabs dos Jogadores")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    [Header("Outros Prefabs")]
    public GameObject cameraPrefab;    // Prefab da câmera (CameraIsometricaComRotacao)
    public GameObject playerUiPrefab;  // Prefab da UI (Canvas + Joystick + Painéis)
    public Transform[] spawnPoints;    // Pontos de spawn (2 posições)

    void Awake()
    {
        // Evita múltiplas instâncias do GameManager
        if (FindObjectsOfType<NetworkGameManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Não conectado ao Photon!");
            return;
        }

        if (!hasSpawned)
        {
            SpawnPlayer();
            hasSpawned = true;
        }
    }

    void SpawnPlayer()
    {
        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;

        // Escolhe o prefab de acordo com o jogador
        GameObject chosenPrefab = actorID == 1 ? player1Prefab : player2Prefab;

        // Define spawn point
        int index = (actorID - 1) % spawnPoints.Length;
        Vector3 spawnPos = spawnPoints[index].position;
        Quaternion spawnRot = spawnPoints[index].rotation;

        // Instancia o jogador em rede
        GameObject player = PhotonNetwork.Instantiate(chosenPrefab.name, spawnPos, spawnRot);

        // Instancia HQ (apenas uma vez para todos)
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameObject.FindWithTag("HQCanvas") == null)
            {
                GameObject hqInstance = PhotonNetwork.Instantiate("HQCanvas", Vector3.zero, Quaternion.identity);
                hqInstance.tag = "HQCanvas";
                DontDestroyOnLoad(hqInstance);
            }
        }

        // Configura o identificador
        PlayerIdentifier identifier = player.GetComponent<PlayerIdentifier>();
        if (identifier != null)
            identifier.actorID = actorID;

        // Apenas o dono local instancia sua UI e câmera
        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            // 1) Instancia UI local
            GameObject uiInstance = Instantiate(playerUiPrefab);
            DontDestroyOnLoad(uiInstance);

            // 2) Ajusta a tag do Canvas para cada jogador
            Canvas canvas = uiInstance.GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvas.gameObject.tag = actorID == 1 ? "CanvasP1" : "CanvasP2";

            // 3) Vincula joystick ao script de movimentação
            MovimentacaoIsometrica mov = player.GetComponent<MovimentacaoIsometrica>();
            if (mov != null)
            {
                FixedJoystick joystick = uiInstance.GetComponentInChildren<FixedJoystick>();
                if (joystick != null)
                    mov.joystick = joystick;
            }

            // 4) Instancia a câmera e define o alvo
            GameObject cam = Instantiate(cameraPrefab);
            CameraIsometricaComRotacao camScript = cam.GetComponent<CameraIsometricaComRotacao>();
            if (camScript != null)
            {
                camScript.player = player.transform;
                if (mov != null)
                    mov.cameraTransform = camScript.transform;
            }

            // 5) Vincula referências de diálogo
            PlayerUIReferences uiRefs = uiInstance.GetComponent<PlayerUIReferences>();
            if (uiRefs != null)
            {
                // Aqui você pode inicializar ou conectar os botões, texto e painel
                uiRefs.painelDialogo.SetActive(false); // Começa fechado
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entrou. Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("Dois jogadores conectados - partida pode começar.");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player saiu. Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }
}
