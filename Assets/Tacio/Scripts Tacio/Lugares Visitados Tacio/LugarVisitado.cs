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
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        // 🔴 CORREÇÃO: Enviar para todos processarem (Master decide a lógica)
        photonView.RPC("RPC_MarcarVisitado", RpcTarget.AllBuffered, pv.OwnerActorNr);
    }

    [PunRPC]
    private void RPC_MarcarVisitado(int playerID)
    {
        // Evitar duplicação
        if (donoDaAtivacao != -1) return;

        // 🔴 CORREÇÃO: Atualizar visual primeiro
        donoDaAtivacao = playerID;

        if (renderObj != null && materialVisitado != null)
            renderObj.sharedMaterial = materialVisitado;

        // 🔴 CORREÇÃO: Só notificar manager se for Master
        if (PhotonNetwork.IsMasterClient)
        {
            if (manager != null && !manager.JogadorJaAtivouLugarNoGrupo(playerID, grupoID))
            {
                manager.MarcarLugar(grupoID, lugarID, playerID);
            }
        }

        Debug.Log($"[LugarVisitado] Lugar {lugarID}-{grupoID} visitado por {playerID} | Master: {PhotonNetwork.IsMasterClient}");
    }

    public bool FoiVisitado()
    {
        return donoDaAtivacao != -1;
    }
}