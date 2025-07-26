using Unity.Netcode;
using UnityEngine;

public class Fish : NetworkBehaviour
{
    public AudioClip somFisgada; 

    // avisar todos os clientes sobre a pesca
    public void AvisarTodos()
    {
        MostrarMensagemClientRpc();
    }

    // tocar o som da fisgada em todos os clientes
    [ClientRpc]
    private void MostrarMensagemClientRpc()
    {
        if (somFisgada != null)
            AudioSource.PlayClipAtPoint(somFisgada, transform.position);
    }
}