using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class DialogoNPC : MonoBehaviourPun
{
    [Header("Referências")]
    public IndicadorNpc indicadorNPC;
    [TextArea(2, 4)] public string[] linhasDialogo;

    [Header("Configuração de Acesso")]
    public bool apenasMasterPodeDialogar = false;

    private int linhaAtual = 0;
    private bool dialogoAtivo = false;

    private GameObject jogadorAtual;
    private TMP_Text textoDialogo;
    private GameObject painelDialogo;
    private Button botaoDialogo;

    private PhotonView photonViewDoJogador;

    [Header("Diálogos Alternativos (Entrega)")]
    public string[] dialogoAposEntrega;
    private bool dialogoDeEntregaAtivo = false;

    // 🔥 Novo: garante que a missão só inicia 1 vez
    private bool missaoJaEntregue = false;

    public void AtivarDialogoDeEntrega()
    {
        dialogoDeEntregaAtivo = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient)
                return;

            jogadorAtual = other.gameObject;
            photonViewDoJogador = pv;

            MostrarBotaoDialogo(jogadorAtual, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            MostrarBotaoDialogo(other.gameObject, false);
            jogadorAtual = null;
        }
    }

    private void MostrarBotaoDialogo(GameObject jogador, bool mostrar)
    {
        var ui = jogador.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null) return;

        ui.botaoDialogo.gameObject.SetActive(mostrar);

        if (mostrar)
        {
            ui.botaoDialogo.onClick.RemoveAllListeners();
            ui.botaoDialogo.onClick.AddListener(() => IniciarDialogo(jogador));
        }
        else
        {
            ui.botaoDialogo.onClick.RemoveAllListeners();
        }
    }

    private void IniciarDialogo(GameObject jogador)
    {
        if (dialogoAtivo) return;

        if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient)
            return;

        linhaAtual = 0;
        dialogoAtivo = true;

        var ui = jogador.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null) return;

        painelDialogo = ui.painelDialogo;
        textoDialogo = ui.textoDialogo;
        botaoDialogo = ui.botaoDialogo;

        painelDialogo.SetActive(true);

        if (dialogoDeEntregaAtivo)
            textoDialogo.text = dialogoAposEntrega[linhaAtual];
        else
            textoDialogo.text = linhasDialogo[linhaAtual];

        StartCoroutine(EsperarToqueParaAvancar());
    }

    private IEnumerator EsperarToqueParaAvancar()
    {
        while (dialogoAtivo)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                AvancarDialogo();
                yield return new WaitForSeconds(0.2f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                AvancarDialogo();
                yield return new WaitForSeconds(0.2f);
            }

            yield return null;
        }
    }

    private void AvancarDialogo()
    {
        linhaAtual++;

        string[] dialogoAtual = dialogoDeEntregaAtivo ? dialogoAposEntrega : linhasDialogo;

        if (linhaAtual < dialogoAtual.Length)
        {
            textoDialogo.text = dialogoAtual[linhaAtual];
        }
        else
        {
            FinalizarDialogo();
        }
    }

    private void FinalizarDialogo()
    {
        painelDialogo.SetActive(false);
        dialogoAtivo = false;

        // Marca como já conversado
        if (indicadorNPC != null)
        {
            indicadorNPC.MarcarComoConversado();
            photonView.RPC("DesativarIndicadorGlobal", RpcTarget.AllBuffered);
        }

        // ------------------------------------------
        // 🔥 SE FOR DIÁLOGO DE ENTREGA
        // ------------------------------------------
        if (dialogoDeEntregaAtivo)
        {
            missaoJaEntregue = true;

            // Esconde UI da missão
            if (MissaoFase1Manager.instancia != null)
                MissaoFase1Manager.instancia.FinalizarEntrega();

            return; // 🔥 Para aqui! Nada mais depois disso roda!
        }

        // ----------------------------------------------
        // 🔥 SE ESTE NPC INICIA A MISSÃO
        // ----------------------------------------------
        if (indicadorNPC != null && indicadorNPC.tipoExclamacao == 2)
        {
            // 🚫 Se missão já foi entregue, NÃO inicia de novo
            if (missaoJaEntregue)
                return;

            var dialogoIntroducao = FindObjectOfType<FuroDeNoticia.DialogoNPCIntroducao>();
            if (dialogoIntroducao != null)
            {
                int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                dialogoIntroducao.MarcarDialogoConcluido(playerID);
            }

            // Apenas Master pode iniciar a missão
            if (PhotonNetwork.IsMasterClient && MissaoFase1Manager.instancia != null)
            {
                MissaoFase1Manager.instancia.IniciarMissao();
            }
        }
    }

    [PunRPC]
    private void DesativarIndicadorGlobal()
    {
        if (indicadorNPC != null && indicadorNPC.iconeExclamacao != null)
            indicadorNPC.iconeExclamacao.SetActive(false);
    }
}
