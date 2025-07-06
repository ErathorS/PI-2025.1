using UnityEngine;
using Photon.Pun;

public class TriggerPhotonDetector : MonoBehaviourPunCallbacks
{
    [Header("Referência ao Objeto Y (este objeto com Trigger)")]
    [SerializeField] private Collider areaTrigger;

    private void Reset()
    {
        // Preenche automaticamente se o script estiver no mesmo GameObject que o Collider
        areaTrigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Apenas o MasterClient processa a colisão (evita múltiplas ativações)
        if (!PhotonNetwork.IsMasterClient) return;

        // Verifica se o objeto que entrou tem a tag "Box"
        if (other.CompareTag("Box"))
        {
            photonView.RPC("ExecutarAlgoZ", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ExecutarAlgoZ()
    {
        Debug.Log("Um objeto com a tag 'Box' entrou na área Trigger! Executando algo Z...");
        // Aqui você pode ativar algo, mudar cor, tocar som, etc.
    }
}
