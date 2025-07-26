using Unity.Netcode;
using UnityEngine;

public class JogadorPescador : NetworkBehaviour
{
    public GameObject peixePrefab;
    
    public void PescarViaBotao()
    {
        if (IsClient)
        {
            PedirPescarServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PedirPescarServerRpc(ServerRpcParams rpcParams = default)
    {
        // Verifica se o NetworkManager existe
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager não encontrado!");
            return;
        }

        // Verifica se o cliente existe
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(rpcParams.Receive.SenderClientId, out var client))
        {
            Debug.LogError($"Cliente {rpcParams.Receive.SenderClientId} não encontrado!");
            return;
        }

        // Verifica se o PlayerObject existe
        if (client.PlayerObject == null)
        {
            Debug.LogError($"PlayerObject do cliente {rpcParams.Receive.SenderClientId} não encontrado!");
            return;
        }

        bool sucesso = Random.value < 0.3f; 

        if (sucesso)
        {
            Vector3 pos = client.PlayerObject.transform.position + Vector3.forward;
            
            // Verifica se o prefab existe
            if (peixePrefab == null)
            {
                return;
            }

            GameObject peixe = Instantiate(peixePrefab, pos, Quaternion.identity);
            var networkObject = peixe.GetComponent<NetworkObject>();

            networkObject.Spawn();

            Fish fishScript = peixe.GetComponent<Fish>();
            if (fishScript != null)
            {
                fishScript.AvisarTodos();
            }

            AtualizarClientesClientRpc(true, rpcParams.Receive.SenderClientId);
            GameManager.Instance?.AdicionarPontuacao(10);
        }
        else
        {
            AtualizarClientesClientRpc(false, rpcParams.Receive.SenderClientId); 
        }
    }

    [ClientRpc]
    private void AtualizarClientesClientRpc(bool sucesso, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.Log(sucesso ? "Peixe fisgado com sucesso!" : "Não conseguiu pescar.");
        }
    }
}