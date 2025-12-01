using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class DialogoNPCIntroducao : MonoBehaviourPun
{
    public bool player1Terminou = false;
    public bool player2Terminou = false;

    [Header("Manager de placas")]
    public PlatePressureManager plateManager;

    [Header("Configuração por Cena")]
    public string nomeCenaDestino = "PI Fase 1";

    private void Start()
    {
        ConfigurarDestinoPorCena();
    }

    private void ConfigurarDestinoPorCena()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;
        
        if (cenaAtual == "Cena de Introducao 1")
            nomeCenaDestino = "PI Fase 1";
        else if (cenaAtual == "Cena de Introducao 2")
            nomeCenaDestino = "PI Fase 2";
        else if (cenaAtual == "Cena de Introducao 3")
            nomeCenaDestino = "PI Fase 3";
        
        Debug.Log($"[DialogoNPCIntroducao] {cenaAtual} → {nomeCenaDestino}");
    }

    public void MarcarDialogoConcluido(int playerID)
    {
        photonView.RPC("RPC_MarcarDialogoConcluido", RpcTarget.All, playerID);
    }

    [PunRPC]
    private void RPC_MarcarDialogoConcluido(int playerID)
    {
        if (playerID == 1) 
            player1Terminou = true;
        else if (playerID == 2) 
            player2Terminou = true;

        Debug.Log($"[DialogoNPCIntroducao] Player{playerID} conversou. P1={player1Terminou} P2={player2Terminou}");

        if (player1Terminou && player2Terminou)
        {
            Debug.Log("[DialogoNPCIntroducao] Ambos jogadores prontos!");

            if (plateManager != null)
            {
                if (plateManager.proximaCena == "PI Fase 1" && !string.IsNullOrEmpty(nomeCenaDestino))
                {
                    plateManager.proximaCena = nomeCenaDestino;
                }
                
                plateManager.EnableListening();
                Debug.Log($"[DialogoNPCIntroducao] Placas ativadas para {plateManager.proximaCena}");
            }
            else
            {
                Debug.LogError("[DialogoNPCIntroducao] PlateManager não configurado!");
            }
        }
    }

    public void ResetarEstado()
    {
        player1Terminou = false;
        player2Terminou = false;
    }

    public void DebugEstado()
    {
        Debug.Log($"[DialogoNPCIntroducao] P1={player1Terminou}, P2={player2Terminou}, Destino={nomeCenaDestino}");
    }
}