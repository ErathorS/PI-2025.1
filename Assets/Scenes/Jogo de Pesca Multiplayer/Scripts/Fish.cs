using Unity.Netcode;
using UnityEngine;

public class Fish : NetworkBehaviour
{
    public AudioClip somFisgada;

    public void AvisarTodos()
    {
        MostrarMensagemClientRpc();
    }

    [ClientRpc]
    private void MostrarMensagemClientRpc()
    {
        if (somFisgada != null)
            AudioSource.PlayClipAtPoint(somFisgada, transform.position);
    }
}
