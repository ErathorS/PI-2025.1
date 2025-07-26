using Unity.Netcode;
using UnityEngine;

public class JogadorPescador : NetworkBehaviour
{
    public GameObject peixePrefab; 
    
    // botão de pescar
    public void PescarViaBotao()
    {
        // Verifica se é cliente 
        if (IsClient)
        {
            PedirPescarServerRpc();
        }
    }

    // solicitação da ação de pesca 
    [ServerRpc(RequireOwnership = false)]
    private void PedirPescarServerRpc(ServerRpcParams rpcParams = default)
    {
        // Verifica se o cliente existe
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(rpcParams.Receive.SenderClientId, out var client))
        {
            Debug.LogError($"Cliente {rpcParams.Receive.SenderClientId} não encontrado!");
            return;
        }

        // chance de sucesso na pesca
        bool sucesso = Random.value < 0.3f; 

        // Indentificador de jogadores
        string nomeJogador = $"Jogador {rpcParams.Receive.SenderClientId + 1}";

        if (sucesso)
        {
            // Posição à frente do jogador para spawnar o peixe
            Vector3 pos = client.PlayerObject.transform.position + Vector3.forward;

            // Instancia e spawna o peixe na rede
            GameObject peixe = Instantiate(peixePrefab, pos, Quaternion.identity);
            var networkObject = peixe.GetComponent<NetworkObject>();
            networkObject.Spawn();

            // Chama o método de aviso no peixe
            Fish fishScript = peixe.GetComponent<Fish>();
            if (fishScript != null)
            {
                fishScript.AvisarTodos();
            }

            // Notifica todos os clientes sobre o sucesso
            AtualizarClientesClientRpc(true, nomeJogador);
            
            // Adiciona pontuação
            GameManager.Instance?.AdicionarPontuacao(10);
        }
        else
        {
            // Notifica todos os clientes sobre a falha
            AtualizarClientesClientRpc(false, nomeJogador);
            
            // Registra a falha no FishSpawnManager (nova linha adicionada)
            FishSpawnManager.Instancia.RegistrarFalha();
        }
    }

    // RPC enviado do servidor para todos os clientes com o resultado da pesca
    [ClientRpc]
    private void AtualizarClientesClientRpc(bool sucesso, string nomeJogador)
    {
        // Mostra a mensagem para todos os clientes
        string mensagem = sucesso ? 
            $"{nomeJogador} pescou um peixe com sucesso! +10 pontos" : 
            $"{nomeJogador} tentou pescar mas não conseguiu...";
        
        Debug.Log(mensagem);
    }
}