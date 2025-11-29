using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class ColetarJornal : MonoBehaviourPun
{
    [Header("Configuração")]
    public string tagJogador = "Player";
    public int jornalID = 0;

    private bool coletado = false;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (coletado) return;
        if (!other.CompareTag(tagJogador)) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            coletado = true;

            Debug.Log($"[ColetarJornal] Jornal {jornalID} coletado por jogador {pv.OwnerActorNr}");

            // 🔴 CORREÇÃO: Processar imediatamente e sincronizar
            if (PhotonNetwork.IsMasterClient)
            {
                // Master processa diretamente
                ProcessarColeta();
            }
            else
            {
                // Cliente envia para master
                photonView.RPC("RPC_ColetarJornal", RpcTarget.MasterClient, jornalID);
            }

            // 🔴 CORREÇÃO: Destruir visualmente para todos imediatamente
            photonView.RPC("RPC_DestruirJornal", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RPC_ColetarJornal(int idJornal)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ProcessarColeta();
        }
    }

    private void ProcessarColeta()
    {
        ProgressaoFaseController progresso = ProgressaoFaseController.instancia;
        if (progresso != null)
        {
            progresso.JornalColetado();
            Debug.Log($"[ColetarJornal] ✅ Jornal {jornalID} processado pelo Master");
        }
    }

    [PunRPC]
    void RPC_DestruirJornal()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
            Debug.Log($"[ColetarJornal] 🗑️ Jornal {jornalID} destruído para todos");
        }
    }
}