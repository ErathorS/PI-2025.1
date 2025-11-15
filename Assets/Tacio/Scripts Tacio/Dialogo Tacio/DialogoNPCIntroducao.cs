using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class DialogoNPCIntroducao : MonoBehaviourPun
    {
        public bool player1Terminou = false;
        public bool player2Terminou = false;

        [Header("Manager de placas")]
        public PlatePressureManager plateManager; // arraste na cena

        // Chamado pelo sistema de diálogo quando o jogador termina a conversa
        public void MarcarDialogoConcluido(int playerID)
        {
            photonView.RPC("RPC_MarcarDialogoConcluido", RpcTarget.All, playerID);
        }

        [PunRPC]
        private void RPC_MarcarDialogoConcluido(int playerID)
        {
            if (playerID == 1) player1Terminou = true;
            else if (playerID == 2) player2Terminou = true;

            //Debug.Log($"[DialogoNPCIntroducao] player1Terminou={player1Terminou} player2Terminou={player2Terminou}");

            if (player1Terminou && player2Terminhou()) // evita typo: use helper
            {
                // ativa o sistema de placas para aguardar os pressionamentos sincronizados
                if (plateManager != null)
                {
                    plateManager.EnableListening();
                    //Debug.Log("[DialogoNPCIntroducao] Ambos terminaram diálogo - placas ativadas.");
                }
                else
                {
                    //Debug.LogWarning("[DialogoNPCIntroducao] plateManager não atribuído!");
                }
            }
        }

        private bool player2Terminhou()
        {
            // método auxiliar para evitar conflito com nomes
            return player2Terminou;
        }
    }
}
