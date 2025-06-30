using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GerenciadorDeJogadores : MonoBehaviourPunCallbacks
{
    void Start()
    {
        VerificarNumeroJogadores();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Jogador entrou na sala: {newPlayer.NickName}");
        VerificarNumeroJogadores();
    }

    void VerificarNumeroJogadores()
    {
        int quantidade = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("Quantidade de jogadores na sala: " + quantidade);

        if (quantidade < 2)
        {
            BloquearJogo();
        }
        else
        {
            DesbloquearJogo();
        }
    }

    void BloquearJogo()
    {
        Debug.Log("Aguardando outro jogador para iniciar o jogo...");
        Time.timeScale = 0f; // Pausa o jogo
        // Alternativamente, exiba uma tela de espera aqui
    }

    void DesbloquearJogo()
    {
        Debug.Log("Dois jogadores conectados. Jogo liberado!");
        Time.timeScale = 1f;
        // Feche a tela de espera se estiver usando uma
    }
}
