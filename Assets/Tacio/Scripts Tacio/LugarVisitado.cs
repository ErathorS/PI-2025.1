using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class LugarVisitado : MonoBehaviourPun
{
    [Header("Identificação do Lugar")]
    public int lugarID = 1; // ID único do lugar

    [Header("Materiais")]
    public Material materialPadrao;
    public Material materialAtivo; // cor verde, por exemplo

    private LugarVisitadoManager manager;
    private Renderer renderObj;
    private bool ativado = false;

    void Start()
    {
        manager = FindObjectOfType<LugarVisitadoManager>();

        // Garante que o collider seja trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        // Localiza automaticamente o Renderer (no próprio objeto ou em um filho)
        renderObj = GetComponent<Renderer>();
        if (renderObj == null)
            renderObj = GetComponentInChildren<Renderer>();

        // Define o material inicial
        if (renderObj != null && materialPadrao != null)
            renderObj.material = materialPadrao;
    }

    void OnTriggerEnter(Collider other)
    {
        if (ativado) return; // já ativado, ignora
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && manager != null && manager.photonView != null)
        {
            // Notifica o manager em rede
            manager.photonView.RPC("RPC_ReportLugarVisitado", RpcTarget.AllBuffered, lugarID, pv.Owner.ActorNumber);
        }
    }

    [PunRPC]
    public void RPC_AtivarLugar()
    {
        ativado = true;

        // Troca o material sincronizado entre todos os jogadores
        if (renderObj != null && materialAtivo != null)
            renderObj.material = materialAtivo;

        Debug.Log($"[LugarVisitado] Lugar {lugarID} completado — material alterado!");
    }
}
