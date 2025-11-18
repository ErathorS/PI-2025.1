using UnityEngine;
using Photon.Pun;

public class PainelFinalFaseController : MonoBehaviourPun
{
    [Header("UI Final")]
    public GameObject painelFimDeFase;   // arraste o GameObject PainelFinalFase

    [Header("Cena da HQ final")]
    [Tooltip("Nome exato da cena da HQ final, igual no Build Settings.")]
    public string nomeCenaHQ = "PI_Fase1_HQ_Final";

    private bool jaMostrou = false;

    void Start()
    {
        if (painelFimDeFase != null)
            painelFimDeFase.SetActive(false);
    }

    /// <summary>
    /// Chamado pelo ProgressaoFaseController quando tudo chega a 100%.
    /// </summary>
    public void MostrarPainelFinal()
    {
        if (jaMostrou) return;
        jaMostrou = true;

        if (painelFimDeFase != null)
            painelFimDeFase.SetActive(true);

        Debug.Log("[PainelFinalFaseController] Painel final exibido.");
    }

    /// <summary>
    /// Chamado pelo botão 'Ir para próxima fase'.
    /// </summary>
    public void BotaoIrParaProximaFase()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[PainelFinalFaseController] Apenas o Master pode iniciar a próxima cena.");
            return;
        }

        if (string.IsNullOrEmpty(nomeCenaHQ))
        {
            Debug.LogError("[PainelFinalFaseController] Nome da cena da HQ não definido!");
            return;
        }

        Debug.Log($"[PainelFinalFaseController] Carregando cena final: {nomeCenaHQ}");
        PhotonNetwork.LoadLevel(nomeCenaHQ);
        // NetworkGameManager já está com AutomaticallySyncScene = true,
        // então todos os jogadores irão junto.
    }
}
