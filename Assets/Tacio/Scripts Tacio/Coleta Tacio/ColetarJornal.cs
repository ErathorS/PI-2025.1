using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class ColetarJornal : MonoBehaviourPun
{
    [Header("Configuração")]
    public string tagJogador = "Player";  // Tag usada pelos jogadores

    private bool coletado = false;

    void Start()
    {
        // Garante que o collider é um trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Evita múltiplas ativações
        if (coletado) return;

        // Verifica se quem encostou é um jogador
        if (!other.CompareTag(tagJogador)) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            coletado = true;

            // 🔍 Busca segura do controlador de progresso
            ProgressaoFaseController progresso = FindObjectOfType<ProgressaoFaseController>();

            if (progresso != null)
            {
                progresso.JornalColetado();
            }
            else
            {
                Debug.LogError("[ColetarJornal] Nenhum ProgressaoFaseController encontrado na cena!");
            }

            // 🔥 Chama RPC para todos destruírem o jornal
            photonView.RPC("RPC_DestruirJornal", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RPC_DestruirJornal()
    {
        // 🔥 Destrói o objeto para todos os jogadores
        Destroy(gameObject);
    }
}
