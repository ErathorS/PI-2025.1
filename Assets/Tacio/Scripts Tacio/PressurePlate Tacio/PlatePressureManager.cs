using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlatePressureManager : MonoBehaviourPun
{
    [Header("Configurações das Placas")]
    public string plateTag = "Plate";          // Tag das placas
    public int expectedPlateCount = 2;         // Quantas placas devem ser pressionadas

    [Header("Bloqueio da área")]
    public GameObject bloqueio;                // Objeto que bloqueia a passagem

    [Header("Próxima cena")]
    [Tooltip("Nome exato da cena que deve ser carregada quando as placas forem ativadas corretamente.")]
    public string proximaCena = "PI Fase 1";   // <-- AGORA É CONFIGURÁVEL NO INSPECTOR

    private bool listening = false;

    // estado interno das placas
    private Dictionary<int, PlateState> plateStates = new Dictionary<int, PlateState>();

    void Start()
    {
        // Registra as placas existentes na cena
        PlatePressure[] plates = FindObjectsOfType<PlatePressure>();
        foreach (var p in plates)
        {
            plateStates[p.plateID] = new PlateState();
        }

        if (bloqueio != null)
            bloqueio.SetActive(true); // começa bloqueado
    }

    /// <summary>
    /// Chamado (por exemplo) quando os dois jogadores terminam o diálogo.
    /// </summary>
    public void EnableListening()
    {
        listening = true;

        if (bloqueio != null)
            bloqueio.SetActive(false); // libera a área das placas

        // reseta o estado das placas
        foreach (var id in new List<int>(plateStates.Keys))
            plateStates[id] = new PlateState();

        //Debug.Log("[PlatePressureManager] Começou a escutar as placas.");
    }

    /// <summary>
    /// Chamado pelas placas quando um jogador pisa.
    /// </summary>
    public void ReportPlatePressed(int plateID, int playerActorID)
    {
        if (!listening) return;

        if (!plateStates.ContainsKey(plateID))
            plateStates[plateID] = new PlateState();

        plateStates[plateID].pressed = true;
        plateStates[plateID].pressedByActorID = playerActorID;

        //Debug.Log($"[PlatePressureManager] Plate {plateID} pressionada por player {playerActorID}");

        if (AllPlatesPressed() && PlatesPressedByDifferentPlayers())
        {
            listening = false;

            if (!PhotonNetwork.IsMasterClient)
                return;

            if (string.IsNullOrEmpty(proximaCena))
            {
                Debug.LogError("[PlatePressureManager] proximaCena não definida no Inspector!");
                return;
            }

            Debug.Log($"[PlatePressureManager] Condição satisfeita — carregando cena '{proximaCena}'");
            PhotonNetwork.LoadLevel(proximaCena);
        }
    }

    bool AllPlatesPressed()
    {
        int count = 0;
        foreach (var kv in plateStates)
            if (kv.Value.pressed)
                count++;

        return count >= expectedPlateCount;
    }

    bool PlatesPressedByDifferentPlayers()
    {
        HashSet<int> players = new HashSet<int>();
        foreach (var kv in plateStates)
            if (kv.Value.pressed)
                players.Add(kv.Value.pressedByActorID);

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
