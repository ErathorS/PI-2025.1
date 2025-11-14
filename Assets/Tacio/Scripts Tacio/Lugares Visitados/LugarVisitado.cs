using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
public class LugarVisitado : MonoBehaviourPun
{
    [Header("Identificação do Lugar")]
    public int lugarID = 1;

    [Header("Materiais")]
    public Material materialPadrao;
    public Material materialVisitado;   // verde

    private Renderer renderObj;
    private bool visitado = false;

    void Awake()
    {
        // Pega o renderer no próprio objeto ou em um filho
        renderObj = GetComponent<Renderer>();
        if (renderObj == null)
            renderObj = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        // Garante que o collider é trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // Define o material inicial
        if (renderObj != null && materialPadrao != null)
            renderObj.sharedMaterial = materialPadrao;
    }

    void OnTriggerEnter(Collider other)
    {
        // Só reage a jogadores
        if (!other.CompareTag("Player"))
            return;

        // Só o dono do player dispara o RPC
        PhotonView pvPlayer = other.GetComponent<PhotonView>();
        if (pvPlayer == null || !pvPlayer.IsMine)
            return;

        // Pede para TODOS (com buffer) marcarem este lugar como visitado
        photonView.RPC("RPC_VisitarLugar", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_VisitarLugar()
    {
        // Se já foi visitado antes, não faz nada
        if (visitado)
            return;

        visitado = true;

        // Troca o material para o verde
        if (renderObj != null && materialVisitado != null)
            renderObj.sharedMaterial = materialVisitado;

        Debug.Log($"[LugarVisitado] Lugar {lugarID} foi visitado.");

        // Só o Master aumenta o contador de lugares visitados no HUD
        if (PhotonNetwork.IsMasterClient)
        {
            ProgressaoFaseController progresso = FindObjectOfType<ProgressaoFaseController>();
            if (progresso != null && progresso.photonView != null)
            {
                progresso.photonView.RPC(
                    "RPC_AtualizarProgressoLugar",
                    RpcTarget.AllBuffered
                );
            }
        }
    }
}
