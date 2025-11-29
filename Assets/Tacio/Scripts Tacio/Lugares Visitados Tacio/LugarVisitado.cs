using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
public class LugarVisitado : MonoBehaviourPun
{
    [Header("IDs")]
    public int grupoID = 0;
    public int lugarID = 0;

    [Header("Materiais")]
    public Material materialPadrao;
    public Material materialVisitado;

    private Renderer renderObj;
    private int donoDaAtivacao = -1;

    private void Awake()
    {
        renderObj = GetComponent<Renderer>();
        if (renderObj == null)
            renderObj = GetComponentInChildren<Renderer>();

        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void Start()
    {
        if (materialPadrao != null && renderObj != null)
        {
            renderObj.sharedMaterial = materialPadrao;
        }

        Debug.Log($"[LugarVisitado] Lugar {grupoID}-{lugarID} inicializado");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        Debug.Log($"[LugarVisitado] Jogador {pv.OwnerActorNr} entrou no lugar {grupoID}-{lugarID}");

        // 🔴 CORREÇÃO: Sempre enviar para o Master decidir se pode ativar
        // A verificação será feita no Master, não localmente
        if (photonView != null)
        {
            photonView.RPC("RPC_TentarMarcarVisitado", RpcTarget.MasterClient, pv.OwnerActorNr);
        }
        else
        {
            Debug.LogError("[LugarVisitado] photonView é nulo!");
        }
    }

    [PunRPC]
    private void RPC_TentarMarcarVisitado(int playerID)
    {
        Debug.Log($"[LugarVisitado] RPC_TentarMarcarVisitado recebido - Lugar {grupoID}-{lugarID}, Jogador: {playerID}");

        // 🔴 CORREÇÃO: Só o Master verifica se o jogador pode ativar
        LugarVisitadoManager manager = FindObjectOfType<LugarVisitadoManager>();
        if (manager != null)
        {
            // Verificar se jogador já ativou outro lugar no mesmo grupo
            if (manager.JogadorJaAtivouLugarNoGrupo(playerID, grupoID))
            {
                Debug.Log($"[LugarVisitado] ❌ Master: Jogador {playerID} JÁ ativou outro lugar no grupo {grupoID}. Bloqueando...");
                
                // 🔴 CORREÇÃO: Notificar o cliente que foi bloqueado
                photonView.RPC("RPC_AtivacaoBloqueada", RpcTarget.All, playerID);
                return;
            }

            // Se chegou aqui, jogador pode ativar este lugar
            Debug.Log($"[LugarVisitado] ✅ Master: Jogador {playerID} pode ativar lugar {grupoID}-{lugarID}");
            
            // Chamar o RPC para marcar visualmente para todos
            photonView.RPC("RPC_MarcarVisitado", RpcTarget.AllBuffered, playerID);
        }
        else
        {
            Debug.LogError($"[LugarVisitado] Manager não encontrado no Master!");
        }
    }

    [PunRPC]
    private void RPC_AtivacaoBloqueada(int playerID)
    {
        Debug.Log($"[LugarVisitado] ❌ Ativação BLOQUEADA para jogador {playerID} no lugar {grupoID}-{lugarID}");
        
        // 🔴 CORREÇÃO: Feedback visual ou sonoro pode ser adicionado aqui
        // Por exemplo: tocar som de erro, mostrar mensagem, etc.
    }

    [PunRPC]
    private void RPC_MarcarVisitado(int playerID)
    {
        // Evitar duplicação
        if (donoDaAtivacao != -1) 
        {
            Debug.Log($"[LugarVisitado] Lugar {grupoID}-{lugarID} já foi visitado por {donoDaAtivacao}");
            return;
        }

        // Atualização visual
        donoDaAtivacao = playerID;

        if (renderObj != null && materialVisitado != null)
        {
            renderObj.sharedMaterial = materialVisitado;
            Debug.Log($"[LugarVisitado] ✅ Material alterado para lugar {grupoID}-{lugarID}");
        }

        Debug.Log($"[LugarVisitado] ✅ Lugar {grupoID}-{lugarID} marcado como visitado por {playerID}");

        // 🔴 CORREÇÃO: Notificar o manager APENAS no Master
        if (PhotonNetwork.IsMasterClient)
        {
            NotificarManager(playerID);
        }
    }

    private void NotificarManager(int playerID)
    {
        LugarVisitadoManager manager = FindObjectOfType<LugarVisitadoManager>();
        if (manager != null)
        {
            manager.MarcarLugar(grupoID, lugarID, playerID);
            Debug.Log($"[LugarVisitado] ✅ Manager notificado sobre lugar {grupoID}-{lugarID}");
        }
        else
        {
            Debug.LogError($"[LugarVisitado] ❌ Manager não encontrado!");
        }
    }

    public bool FoiVisitado()
    {
        return donoDaAtivacao != -1;
    }

    public void DebugEstado()
    {
        Debug.Log($"[LugarVisitado] Lugar {grupoID}-{lugarID} - Visitado: {FoiVisitado()}, Dono: {donoDaAtivacao}");
    }
}