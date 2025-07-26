using Unity.Netcode;
using UnityEngine;

public class Fish : NetworkBehaviour
{
    public AudioClip somFisgada; // Som que será tocado quando o peixe for pescado

    // Método para avisar todos os clientes sobre a pesca
    public void AvisarTodos()
    {
        MostrarMensagemClientRpc();
    }

    // RPC para tocar o som da fisgada em todos os clientes
    [ClientRpc]
    private void MostrarMensagemClientRpc()
    {
        if (somFisgada != null)
            AudioSource.PlayClipAtPoint(somFisgada, transform.position);
    }
}