using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<int> pontuacaoEquipe = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        Instance = this;
    }

    public void AdicionarPontuacao(int valor)
    {
        if (IsServer)
        {
            pontuacaoEquipe.Value += valor;
        }
    }
}
