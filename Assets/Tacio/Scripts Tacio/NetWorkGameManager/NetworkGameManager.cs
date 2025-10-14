using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{
    [Header("Prefabs dos Jogadores")]
    public GameObject player1Prefab;   // Aparência do Player 1
    public GameObject player2Prefab;   // Aparência do Player 2

    [Header("Outros Prefabs")]
    public GameObject cameraPrefab;    // Prefab da câmera (CameraIsometricaComRotacao)
    public GameObject playerUiPrefab;  // Prefab da UI (Canvas + Joystick + painéis)
    public Transform[] spawnPoints;    // Pontos de spawn (2 posições)

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Não conectado ao Photon!");
            return;
        }

        SpawnPlayer();
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

        // Instancia o jogador
        GameObject player = PhotonNetwork.Instantiate(chosenPrefab.name, spawnPos, spawnRot);

        // Instancia HQ apenas uma vez em rede (todos recebem)
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject hqCanvas = GameObject.FindWithTag("HQCanvas");
            if (hqCanvas == null)
            {
                GameObject hqInstance = PhotonNetwork.Instantiate("HQCanvas", Vector3.zero, Quaternion.identity);
                hqInstance.tag = "HQCanvas";
                DontDestroyOnLoad(hqInstance);
                Debug.Log("HQ instanciada em rede pelo Player 1 (MasterClient).");
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
            // Instancia UI local
            GameObject ui = Instantiate(playerUiPrefab);

            // Define a tag do Canvas conforme o jogador
            Canvas canvas = ui.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                string tagName = actorID == 1 ? "CanvasP1" : "CanvasP2";
                canvas.gameObject.tag = tagName;
            }

            // Vincula joystick ao script de movimentação
            var mov = player.GetComponent<MovimentacaoIsometrica>();
            if (mov != null)
            {
                FixedJoystick joystick = ui.GetComponentInChildren<FixedJoystick>();
                if (joystick != null)
                    mov.joystick = joystick;
            }

            // Instancia a câmera e define o alvo
            GameObject cam = Instantiate(cameraPrefab);
            CameraIsometricaComRotacao camScript = cam.GetComponent<CameraIsometricaComRotacao>();

            if (camScript != null)
            {
                camScript.player = player.transform;
                // Vincula referência da câmera ao script de movimento
                if (mov != null)
                    mov.cameraTransform = camScript.transform;
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
