using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlatePressureManager : MonoBehaviourPun
{
    [Header("Configurações")]
    public string proximaCena = "Fase1";
    public string plateTag = "Plate"; // Tag das placas
    public int expectedPlateCount = 2; // Quantas placas devem ser pressionadas
    public GameObject bloqueio; // Arraste o objeto de bloqueio da cena aqui

    private bool listening = false;
    private Dictionary<int, PlateState> plateStates = new Dictionary<int, PlateState>();

    void Start()
    {
        // Procura todas as placas na cena e as registra
        PlatePressure[] plates = FindObjectsOfType<PlatePressure>();
        foreach (var p in plates)
        {
            plateStates[p.plateID] = new PlateState();
        }

        if (bloqueio != null)
            bloqueio.SetActive(true); // começa bloqueado
    }

    public void EnableListening()
    {
        listening = true;
        Debug.Log("[PlatePressureManager] Escutando as placas.");

        if (bloqueio != null)
            bloqueio.SetActive(false); // libera a passagem quando começa a escutar

        foreach (var id in new List<int>(plateStates.Keys))
            plateStates[id] = new PlateState();
    }

    public void ReportPlatePressed(int plateID, int playerActorID)
    {
        if (!listening) return;

        if (!plateStates.ContainsKey(plateID))
            plateStates[plateID] = new PlateState();

        plateStates[plateID].pressed = true;
        plateStates[plateID].pressedByActorID = playerActorID;

        Debug.Log($"[PlatePressureManager] Plate {plateID} pressionada por player {playerActorID}");

        if (AllPlatesPressed() && PlatesPressedByDifferentPlayers())
        {
            Debug.Log("[PlatePressureManager] Condição satisfeita — carregando próxima cena!");
            listening = false;

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(proximaCena);
            else
                photonView.RPC(nameof(RequestMasterLoadScene), RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void RequestMasterLoadScene()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(proximaCena);
    }

    bool AllPlatesPressed()
    {
        int count = 0;
        foreach (var kv in plateStates)
            if (kv.Value.pressed) count++;

        return count >= expectedPlateCount;
    }

    bool PlatesPressedByDifferentPlayers()
    {
        HashSet<int> players = new HashSet<int>();
        foreach (var kv in plateStates)
            if (kv.Value.pressed) players.Add(kv.Value.pressedByActorID);

        return players.Count >= expectedPlateCount;
    }

    private class PlateState
    {
        public bool pressed = false;
        public int pressedByActorID = -1;
    }

    [PunRPC]
    public void RPC_ReportPlatePressed(int plateID, int playerActorID)
    {
        ReportPlatePressed(plateID, playerActorID);
    }
}
