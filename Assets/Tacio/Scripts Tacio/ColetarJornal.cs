using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class ColetarJornal : MonoBehaviourPun
{
    [Header("Configuração")]
    public string tagJogador = "Player";  // Tag dos jogadores

    private bool coletado = false;

    void Start()
    {
        // Garante que o collider está configurado como trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Evita múltiplas ativações
        if (coletado) return;

        // Verifica se quem encostou foi um jogador
        if (other.CompareTag(tagJogador))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                coletado = true;

                // Atualiza o progresso para todos os jogadores
                ProgressaoFaseController progresso = FindObjectOfType<ProgressaoFaseController>();
                if (progresso != null)
                {
                    progresso.JornalColetado();
                }

                // Desativa o jornal em rede
                photonView.RPC("RPC_ColetarJornal", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void RPC_ColetarJornal()
    {
        // Desativa o objeto na cena para todos
        gameObject.SetActive(false);
    }
}
