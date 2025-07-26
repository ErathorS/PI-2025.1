using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance; // Singleton do GameManager

    // Variável de rede para armazenar a pontuação da equipe
    public NetworkVariable<int> pontuacaoEquipe = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        // Configura o singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Método para adicionar pontuação (só funciona no servidor)
    public void AdicionarPontuacao(int valor)
    {
        if (IsServer)
            pontuacaoEquipe.Value += valor;
    }
}