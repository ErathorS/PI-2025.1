using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance; 

    // armazena a pontuação da equipe
    public NetworkVariable<int> pontuacaoEquipe = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // adicionar pontuação que so funciona no Host/server
    public void AdicionarPontuacao(int valor)
    {
        if (IsServer)
            pontuacaoEquipe.Value += valor;
    }
}