using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PainelFinalFaseController : MonoBehaviourPunCallbacks
{
    [Header("UI Final")]
    public GameObject painelFimDeFase;

    [Header("Cenas")]
    public string nomeCenaMenu = "MenuJogo";
    public string nomeProximaCena = "ProximaFase"; // ADICIONE: Nome da próxima cena

    private bool jaMostrou = false;

    void Start()
    {
        if (painelFimDeFase != null)
            painelFimDeFase.SetActive(false);
    }

    /// <summary>
    /// Chamado quando a fase é concluída
    /// </summary>
    public void MostrarPainelFinal()
    {
        if (jaMostrou) return;
        jaMostrou = true;

        if (painelFimDeFase != null)
            painelFimDeFase.SetActive(true);

        Debug.Log("[PainelFinalFaseController] PainelFinal exibido.");
    }

    /// <summary>
    /// Chamado pelo botão 'Voltar ao Menu' 
    /// </summary>
    public void BotaoVoltarAoMenu()
    {
        Debug.Log($"[PainelFinalFaseController] Botão voltar ao menu pressionado. Master: {PhotonNetwork.IsMasterClient}");

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("[PainelFinalFaseController] Saindo da sala Photon...");
        }
        else
        {
            SceneManager.LoadScene(nomeCenaMenu);
            Debug.Log($"[PainelFinalFaseController] Carregando cena do menu: {nomeCenaMenu}");
        }
    }

    /// <summary>
    /// CORREÇÃO: Botão 'Próxima Fase' agora carrega a próxima cena
    /// </summary>
    public void BotaoIrParaProximaFase()
    {
        Debug.Log($"[PainelFinalFaseController] Botão próxima fase pressionado. Master: {PhotonNetwork.IsMasterClient}");

        if (PhotonNetwork.IsConnected)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("[PainelFinalFaseController] Apenas o Master pode iniciar a próxima fase.");
                return;
            }

            Debug.Log($"[PainelFinalFaseController] Carregando próxima cena: {nomeProximaCena}");
            PhotonNetwork.LoadLevel(nomeProximaCena); // USA PhotonNetwork.LoadLevel para multiplayer
        }
        else
        {
            // Modo single player
            Debug.Log($"[PainelFinalFaseController] Carregando próxima cena: {nomeProximaCena}");
            SceneManager.LoadScene(nomeProximaCena);
        }
    }

    /// <summary>
    /// Chamado quando sai da sala Photon
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("[PainelFinalFaseController] Saiu da sala Photon, voltando ao menu");
        SceneManager.LoadScene(nomeCenaMenu);
    }
}