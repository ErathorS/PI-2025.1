using Unity.Netcode;
using UnityEngine;

public class PlayerFishing : NetworkBehaviour
{
    public AudioSource somFisgada;

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TentarPescarServerRpc();
        }
    }

    [ServerRpc]
    void TentarPescarServerRpc(ServerRpcParams rpcParams = default)
    {
        bool sucesso = Random.value > 0.5f;

        if (sucesso)
        {
            GameManager.Instance.AdicionarPontuacao(10);
            TocarFisgadaClientRpc();
        }
    }

    [ClientRpc]
    void TocarFisgadaClientRpc()
    {
        if (somFisgada != null)
            somFisgada.Play();
    }
}
