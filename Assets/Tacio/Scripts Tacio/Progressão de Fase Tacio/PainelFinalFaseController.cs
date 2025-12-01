using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PainelFinalFaseController : MonoBehaviourPunCallbacks
{
    [Header("UI Final")]
    public GameObject painelFimDeFase;

    [Header("Cenas")]
    public string nomeCenaMenu = "MenuJogo";
    public string nomeProximaCena = "ProximaFase"; 

    private bool jaMostrou = false;

    void Start()
    {
        if (painelFimDeFase != null)
            painelFimDeFase.SetActive(false);
    }

    public void MostrarPainelFinal()
    {
        if (jaMostrou) return;
        jaMostrou = true;

        if (painelFimDeFase != null)
            painelFimDeFase.SetActive(true);

        Debug.Log("[PainelFinalFaseController] PainelFinal exibido.");
    }

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
            Debug.Log($"[PainelFinalFaseController] Carregando próxima cena: {nomeProximaCena}");
            SceneManager.LoadScene(nomeProximaCena);
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("[PainelFinalFaseController] Saiu da sala Photon, voltando ao menu");
        SceneManager.LoadScene(nomeCenaMenu);
    }
}