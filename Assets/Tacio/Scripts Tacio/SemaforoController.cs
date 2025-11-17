using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class SemaforoController : MonoBehaviourPun
{
    public enum Estado { Verde, Vermelho }

    [Header("Visual")]
    public Renderer luzRenderer;
    public Material matVerde;
    public Material matVermelho;

    [Header("Configuração")]
    public float duracaoVermelho = 5f;

    public bool IsRed => estadoAtual == Estado.Vermelho;

    private Estado estadoAtual = Estado.Verde;
    private float timer = 0f;

    void Start()
    {
        AtualizarVisual();
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (estadoAtual == Estado.Vermelho)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                MudarParaVerde();
            }
        }
    }

    void MudarParaVermelho()
    {
        estadoAtual = Estado.Vermelho;
        timer = duracaoVermelho;
        photonView.RPC("RPC_SincronizarEstado", RpcTarget.All, (int)estadoAtual);
    }

    void MudarParaVerde()
    {
        estadoAtual = Estado.Verde;
        photonView.RPC("RPC_SincronizarEstado", RpcTarget.All, (int)estadoAtual);
    }

    [PunRPC]
    void RPC_SincronizarEstado(int estado)
    {
        estadoAtual = (Estado)estado;
        AtualizarVisual();
    }

    void AtualizarVisual()
    {
        if (luzRenderer == null) return;

        if (estadoAtual == Estado.Vermelho && matVermelho != null)
            luzRenderer.material = matVermelho;
        else if (estadoAtual == Estado.Verde && matVerde != null)
            luzRenderer.material = matVerde;
    }

    // Chamado pelo script de interação do jogador
    public void PedirAcionarSemaforo()
    {
        photonView.RPC("RPC_PedirAcionarSemaforo", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PedirAcionarSemaforo()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (estadoAtual == Estado.Verde)
            MudarParaVermelho();
    }
}
