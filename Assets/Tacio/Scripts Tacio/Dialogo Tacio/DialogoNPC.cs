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

    private int linhaAtual = 0;
    private bool jogadorPerto = false;
    private bool dialogoAtivo = false;

    private GameObject jogadorAtual;
    private TMP_Text textoDialogo;
    private GameObject painelDialogo;
    private Button botaoDialogo;
    private PhotonView photonViewDoJogador;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            jogadorPerto = true;
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
            jogadorPerto = false;
            jogadorAtual = null;
            MostrarBotaoDialogo(other.gameObject, false);
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
        if (dialogoAtivo || linhasDialogo.Length == 0) return;

        dialogoAtivo = true;
        linhaAtual = 0;

        var ui = jogador.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null) return;

        painelDialogo = ui.painelDialogo;
        textoDialogo = ui.textoDialogo;
        botaoDialogo = ui.botaoDialogo;

        painelDialogo.SetActive(true);
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

        if (linhaAtual < linhasDialogo.Length)
        {
            textoDialogo.text = linhasDialogo[linhaAtual];
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

        if (indicadorNPC != null)
        {
            indicadorNPC.MarcarComoConversado();
            photonView.RPC("DesativarIndicadorGlobal", RpcTarget.AllBuffered);
        }

        // 🔹 Se for NPC importante, avisa o sistema de introdução
        if (indicadorNPC != null && indicadorNPC.tipoExclamacao == 2)
        {
            var dialogoIntroducao = FindObjectOfType<FuroDeNoticia.DialogoNPCIntroducao>();
            if (dialogoIntroducao != null)
            {
                int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                dialogoIntroducao.MarcarDialogoConcluido(playerID);
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
