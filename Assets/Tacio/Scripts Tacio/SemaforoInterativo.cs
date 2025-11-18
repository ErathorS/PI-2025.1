using UnityEngine;
using Photon.Pun;

public class SemaforoInterativo : MonoBehaviourPun
{
    [Header("Configuração")]
    public Light luzSemaforo;
    public Color corVerde = Color.green;
    public Color corVermelho = Color.red;
    public float tempoVermelho = 5f;

    private bool semaforoAtivo = false;
    private float timer = 0f;

    private void Start()
    {
        if (luzSemaforo != null)
            luzSemaforo.color = corVerde;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            var ui = other.GetComponentInChildren<PlayerUIReferences>();
            if (ui != null && ui.botaoInteracao != null)   // ← NOME REAL DO SEU CAMPO
            {
                ui.botaoInteracao.gameObject.SetActive(true);
                ui.botaoInteracao.onClick.RemoveAllListeners();
                ui.botaoInteracao.onClick.AddListener(() => AtivarSemaforo(pv.OwnerActorNr));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            var ui = other.GetComponentInChildren<PlayerUIReferences>();
            if (ui != null && ui.botaoInteracao != null)
            {
                ui.botaoInteracao.gameObject.SetActive(false);
                ui.botaoInteracao.onClick.RemoveAllListeners();
            }
        }
    }

    private void AtivarSemaforo(int actorID)
    {
        if (!photonView.IsMine)
        {
            photonView.RPC("RPC_AtivarSemaforo", RpcTarget.MasterClient, actorID);
            return;
        }

        RPC_AtivarSemaforo(actorID);
    }

    [PunRPC]
    private void RPC_AtivarSemaforo(int actorID)
    {
        if (semaforoAtivo) return;

        semaforoAtivo = true;
        timer = tempoVermelho;

        if (luzSemaforo != null)
            luzSemaforo.color = corVermelho;
    }

    private void Update()
    {
        if (!semaforoAtivo) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (photonView.IsMine)
                photonView.RPC("RPC_DesativarSemaforo", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void RPC_DesativarSemaforo()
    {
        semaforoAtivo = false;

        if (luzSemaforo != null)
            luzSemaforo.color = corVerde;
    }
}
