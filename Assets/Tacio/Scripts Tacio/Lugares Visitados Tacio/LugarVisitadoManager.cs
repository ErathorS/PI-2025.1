using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LugarVisitadoManager : MonoBehaviourPun
{
    [Header("Grupos de Lugares")]
    public List<LugarVisitado> grupo1;
    public List<LugarVisitado> grupo2;

    private bool grupo1Concluido = false;
    private bool grupo2Concluido = false;

    private Dictionary<(int grupoID, int lugarID), int> ativacoes = new Dictionary<(int, int), int>();

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            VerificarEAtualizarProgresso();
        }
    }

    public bool JogadorJaAtivouLugarNoGrupo(int playerID, int grupoID)
    {
        foreach (var entry in ativacoes)
        {
            if (entry.Key.grupoID == grupoID && entry.Value == playerID)
                return true;
        }
        return false;
    }

    public void MarcarLugar(int grupoID, int lugarID, int playerID)
    {
        // 🔴 CORREÇÃO: Só o Master atualiza o dicionário principal
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[LugarVisitadoManager] Cliente tentou marcar lugar - enviando para Master");
            photonView.RPC("RPC_MarcarLugarNoMaster", RpcTarget.MasterClient, grupoID, lugarID, playerID);
            return;
        }

        // 🔴 Só Master processa a lógica principal
        ativacoes[(grupoID, lugarID)] = playerID;
        
        Debug.Log($"[LugarVisitadoManager] Master marcou lugar - Grupo: {grupoID}, Lugar: {lugarID}, Jogador: {playerID}");

        // Sincronizar com outros jogadores
        photonView.RPC("RPC_SincronizarLugar", RpcTarget.Others, grupoID, lugarID, playerID);

        // Verificar progresso
        switch (grupoID)
        {
            case 1:
                VerificarGrupo(grupo1, ref grupo1Concluido);
                break;
            case 2:
                VerificarGrupo(grupo2, ref grupo2Concluido);
                break;
        }
    }

    [PunRPC]
    private void RPC_MarcarLugarNoMaster(int grupoID, int lugarID, int playerID)
    {
        // 🔴 Só o Master processa
        if (PhotonNetwork.IsMasterClient)
        {
            MarcarLugar(grupoID, lugarID, playerID);
        }
    }

    [PunRPC]
    private void RPC_SincronizarLugar(int grupoID, int lugarID, int playerID)
    {
        // 🔴 CORREÇÃO: Clientes só atualizam visualmente, não a lógica
        Debug.Log($"[LugarVisitadoManager] Cliente sincronizou lugar visual - Grupo: {grupoID}, Lugar: {lugarID}");

        // Atualizar visualmente (material) mas NÃO contar progresso
        AtualizarLugarVisual(grupoID, lugarID);
    }

    private void AtualizarLugarVisual(int grupoID, int lugarID)
    {
        List<LugarVisitado> grupo = grupoID == 1 ? grupo1 : grupo2;
        
        if (lugarID >= 0 && lugarID < grupo.Count && grupo[lugarID] != null)
        {
            // O LugarVisitado já tem seu próprio RPC para material
            Debug.Log($"[LugarVisitadoManager] Lugar visual atualizado no cliente - Grupo: {grupoID}, Lugar: {lugarID}");
        }
    }

    private void VerificarGrupo(List<LugarVisitado> grupo, ref bool grupoConcluido)
    {
        if (grupoConcluido) return;

        bool todosVisitados = true;
        foreach (var lugar in grupo)
        {
            if (!lugar.FoiVisitado())
            {
                todosVisitados = false;
                break;
            }
        }

        if (todosVisitados)
        {
            grupoConcluido = true;

            // 🔴 CORREÇÃO: Só Master conta progresso
            if (PhotonNetwork.IsMasterClient)
            {
                ProgressaoFaseController prog = FindObjectOfType<ProgressaoFaseController>();
                if (prog != null)
                {
                    prog.LugarVisitadoConcluido();
                    Debug.Log($"[LugarVisitadoManager] ✅ Progresso de lugar notificado pelo Master!");
                }
            }
        }
    }

    public void VerificarEAtualizarProgresso()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        VerificarGrupo(grupo1, ref grupo1Concluido);
        VerificarGrupo(grupo2, ref grupo2Concluido);
    }
}