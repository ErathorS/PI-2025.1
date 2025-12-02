using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PainelFinalFaseController : MonoBehaviourPunCallbacks
{
    [Header("UI Final")]
    public GameObject painelFimDeFase;
    public GameObject canvasFinal; // ADICIONE ESTA REFERÊNCIA NO INSPECTOR

    [Header("Cenas")]
    public string nomeCenaMenu = "MenuJogo";
    public string nomeProximaCena = "ProximaFase";

    private bool jaMostrou = false;

    void Start()
    {
        if (painelFimDeFase != null)
            painelFimDeFase.SetActive(false);

        if (canvasFinal != null)
            canvasFinal.SetActive(false);
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
        Debug.Log($"[PainelFinalFaseController] Botão voltar ao menu pressionado.");

        if (PhotonNetwork.IsConnected)
        {
            // Encontrar o ProgressaoFaseController e chamar RPC
            ProgressaoFaseController progressController = FindObjectOfType<ProgressaoFaseController>();
            if (progressController != null && progressController.photonView != null)
            {
                progressController.photonView.RPC("RPC_MostrarCanvasFinal", RpcTarget.All);
            }
        }
        else
        {
            // Modo single player
            if (painelFimDeFase != null)
                painelFimDeFase.SetActive(false);

            if (canvasFinal != null)
            {
                canvasFinal.SetActive(true);
                Debug.Log("[PainelFinalFaseController] Canvas final ativado.");
            }
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

            // Garantir que ambos os jogadores vão para a próxima cena
            if (PhotonNetwork.IsMasterClient)
            {
                // O master carrega a cena e os outros sincronizam
                PhotonNetwork.LoadLevel(nomeProximaCena);
            }
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