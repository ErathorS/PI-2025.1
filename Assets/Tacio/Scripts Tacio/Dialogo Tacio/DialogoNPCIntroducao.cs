using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class DialogoNPCIntroducao : MonoBehaviourPun
    {
        public bool player1Terminou = false;
        public bool player2Terminou = false;

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

            // Checa se os dois terminaram
            if (player1Terminou && player2Terminou)
            {
                FindObjectOfType<ControladorIntroducaoFase1>().LiberarAcesso();
            }
        }
    }
}
