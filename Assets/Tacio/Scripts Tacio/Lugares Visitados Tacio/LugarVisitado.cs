using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
public class LugarVisitado : MonoBehaviourPun
{
    [Header("IDs")]
    public int grupoID = 0;  // 1 ou 2
    public int lugarID = 0;  // 1 ou 2

    [Header("Materiais")]
    public Material materialPadrao;
    public Material materialVisitado;

    private Renderer renderObj;

    // Quem ativou este lugar
    private int donoDaAtivacao = -1;

    private LugarVisitadoManager manager;

    private void Awake()
    {
        renderObj = GetComponent<Renderer>();
        if (renderObj == null)
            renderObj = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (materialPadrao != null)
            renderObj.sharedMaterial = materialPadrao;

        manager = FindObjectOfType<LugarVisitadoManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine)
            return;

        // Envia quem pisou
        photonView.RPC("RPC_MarcarVisitado", RpcTarget.AllBuffered, pv.OwnerActorNr);
    }

    [PunRPC]
    private void RPC_MarcarVisitado(int playerID)
    {
        // ❌ Se este lugar já foi ativado por alguém → não muda nada
        if (donoDaAtivacao != -1)
            return;

        // ❌ Se o jogador já ativou OUTRO lugar do MESMO grupo → ele não ativa este
        if (manager.JogadorJaAtivouLugarNoGrupo(playerID, grupoID))
            return;

        // 🔥 Marca dono
        donoDaAtivacao = playerID;

        // 🔥 Troca material
        if (renderObj != null && materialVisitado != null)
            renderObj.sharedMaterial = materialVisitado;

        // 🔥 Notifica manager
        manager.MarcarLugar(grupoID, lugarID, playerID);
    }

    public bool FoiVisitado()
    {
        return donoDaAtivacao != -1;
    }
}
