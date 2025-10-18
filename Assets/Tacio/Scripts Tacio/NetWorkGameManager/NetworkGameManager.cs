using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;      // prefab do player (sem camera)
    public GameObject cameraPrefab;      // prefab da câmera (CameraIsometricaComRotacao)
    public GameObject playerUiPrefab;    // prefab do Canvas com joystick e painéis de diálogo
    public Transform[] spawnPoints;      // 2 posições (assign no editor)

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

        // Escolhe spawn point baseado no actorNumber (1 ou 2)
        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
        Vector3 spawnPos = spawnPoints[index].position;
        Quaternion spawnRot = spawnPoints[index].rotation;

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);

        // só o dono local instancia sua UI e câmera
        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            // Instancia UI local (Canvas + joystick etc)
            GameObject ui = Instantiate(playerUiPrefab);
            // set tag do canvas conforme actorNumber (CanvasP1 ou CanvasP2)
            int actorID = PhotonNetwork.LocalPlayer.ActorNumber;
            Canvas canvas = ui.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                string tagName = actorID == 1 ? "CanvasP1" : "CanvasP2";
                canvas.gameObject.tag = tagName;
            }

            // Link joystick ao script de movimentação (se precisar)
            var mov = player.GetComponent<MovimentacaoIsometrica>();
            if (mov != null)
            {
                // O PlayerUI deve ter um componente FixedJoystick na cena
                FixedJoystick joystick = ui.GetComponentInChildren<FixedJoystick>();
                if (joystick != null)
                    mov.joystick = joystick;
            }

            // Instancia a câmera local e seta o player como alvo
            GameObject cam = Instantiate(cameraPrefab);
            CameraIsometricaComRotacao camScript = cam.GetComponent<CameraIsometricaComRotacao>();
            if (camScript != null)
            {
                camScript.player = player.transform;
            }

            // Linka a câmera ao script de movimentação (se precisar)
            if (camScript != null)
            {
                camScript.player = player.transform;
                // linka a câmera ao script de movimento
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
            // Aqui você pode liberar UI de "Start" ou ativar outros managers
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player saiu. Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        // Desabilitar/pausar se necessário
    }
}
