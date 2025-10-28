using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LugarVisitadoManager : MonoBehaviourPun
{
    [Header("Configurações")]
    public int expectedPlayers = 2; // Quantos jogadores precisam visitar

    // Quem já pisou em cada lugar
    private Dictionary<int, HashSet<int>> lugaresVisitados = new Dictionary<int, HashSet<int>>();

    // Lugares já completados (estado local por cliente)
    private HashSet<int> lugaresCompletos = new HashSet<int>();

    private ProgressaoFaseController progresso;

    void Start()
    {
        progresso = FindObjectOfType<ProgressaoFaseController>();

        // Inicializa todos os lugares encontrados na cena
        LugarVisitado[] lugares = FindObjectsOfType<LugarVisitado>();
        foreach (var lugar in lugares)
        {
            if (!lugaresVisitados.ContainsKey(lugar.lugarID))
                lugaresVisitados[lugar.lugarID] = new HashSet<int>();
        }
    }

    [PunRPC]
    public void RPC_ReportLugarVisitado(int lugarID, int playerActorID)
    {
        // Recria referências se necessário (caso cena tenha carregado depois)
        if (progresso == null)
            progresso = FindObjectOfType<ProgressaoFaseController>();

        if (!lugaresVisitados.ContainsKey(lugarID))
            lugaresVisitados[lugarID] = new HashSet<int>();

        // Se já tínhamos marcado localmente como completo, ignora
        if (lugaresCompletos.Contains(lugarID))
            return;

        // Registra o jogador que pisou
        lugaresVisitados[lugarID].Add(playerActorID);
        Debug.Log($"[LugarVisitadoManager] Jogador {playerActorID} visitou o lugar {lugarID}");

        // Só o MASTER decide concluir e propagar
        if (!PhotonNetwork.IsMasterClient)
            return;

        // No Master: se atingiu a quantidade necessária, conclui UMA vez
        if (lugaresVisitados[lugarID].Count >= expectedPlayers)
        {
            // Dispara para todos um RPC que marca o lugar como completo localmente
            photonView.RPC("RPC_MarcarLugarCompleto_Global", RpcTarget.AllBuffered, lugarID);

            // E só o Master dispara o incremento de progresso (uma vez para todos)
            if (progresso != null && progresso.photonView != null)
            {
                progresso.photonView.RPC("RPC_AtualizarProgressoLugar", RpcTarget.AllBuffered);
            }

            Debug.Log($"[LugarVisitadoManager] Lugar {lugarID} COMPLETO (decidido pelo Master).");
        }
    }

    // Este RPC roda em TODOS: marca localmente como completo e troca o material
    [PunRPC]
    void RPC_MarcarLugarCompleto_Global(int lugarID)
    {
        // Evita rodar duas vezes se o buffer do Photon reexecutar
        if (!lugaresCompletos.Add(lugarID))
            return;

        // Ativa o visual (verde) para todos
        LugarVisitado lugar = GetLugarByID(lugarID);
        if (lugar != null && lugar.photonView != null)
        {
            lugar.photonView.RPC("RPC_AtivarLugar", RpcTarget.AllBuffered);
        }
    }

    private LugarVisitado GetLugarByID(int id)
    {
        foreach (var l in FindObjectsOfType<LugarVisitado>())
            if (l.lugarID == id)
                return l;
        return null;
    }
}
