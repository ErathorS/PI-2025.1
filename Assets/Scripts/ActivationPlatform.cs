using UnityEngine;
using Photon.Pun;

public class ActivationPlatform : MonoBehaviourPun
{
    [Header("Configurações")]
    public GameObject objectToActivate; // Objeto a ser ativado/desativado
    public float activationDelay = 0.5f; // Tempo antes da ativação
    public bool isToggle = true; // Se true, alterna estado. Se false, só ativa

    [Header("Estado")]
    [SerializeField] private bool isActivated = false;

    private int playersOnPlatform = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersOnPlatform++;

            if (playersOnPlatform == 1) // Primeiro jogador a entrar
            {
                photonView.RPC("RequestActivation", RpcTarget.MasterClient, !isActivated);
               
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersOnPlatform--;

            if (playersOnPlatform == 0 && !isToggle) // Último jogador saiu e não é toggle
            {
                photonView.RPC("RequestActivation", RpcTarget.MasterClient, false);
            }
        }
         Debug.Log("Funcionou");
    }

    [PunRPC]
    private void RequestActivation(bool newState)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetActivationState", RpcTarget.AllBuffered, newState);
        }
    }

    [PunRPC]
    private void SetActivationState(bool newState)
    {
        isActivated = newState;

        if (objectToActivate != null)
        {
            // Ativa/desativa o objeto ou chama seu método de ativação
            if (objectToActivate.TryGetComponent<IActivatable>(out var activatable))
            {
                activatable.Activate(isActivated);
            }
            else
            {
                objectToActivate.SetActive(isActivated);
            }
        }

        // Aqui você pode adicionar efeitos visuais/sonoros
    }
    
}

// Interface para objetos ativáveis
public interface IActivatable
{

    void Activate(bool state);
}
