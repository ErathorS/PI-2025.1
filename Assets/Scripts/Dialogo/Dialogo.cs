using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Dialogo : MonoBehaviourPun
{
    [Header("Referências")]
    public Button botaoInteragir1;
    public Button botaoInteragir2;
    public GameObject painelDialogo1;
    public GameObject painelDialogo2;
    public TMP_Text textoDialogo1;
    public TMP_Text textoDialogo2;
    public IndicadorNpc indicadorNPC;

    [Header("Configuração")]
    public bool dialogoGlobal = false; // <- NOVO: define se é local ou global

    [Header("Linhas do Diálogo")]
    [TextArea(2, 4)]
    [SerializeField] private string[] linhasDialogo = {
        "",
        "Olá, viajante!",
        "Você está pronto para uma nova aventura?",
        "Boa sorte no seu caminho!"
    };

    private int linhaAtual = 0;
    private bool falando = false;
    private bool jogadorPerto = false;

    private PhotonView photonViewDoIniciador; // <- Quem iniciou o diálogo

    void Start()
    {
        botaoInteragir1.gameObject.SetActive(false);
        painelDialogo1.SetActive(false);

        botaoInteragir1.onClick.AddListener(AoClicarNoBotao);
    }

    void OnDestroy()
    {
        botaoInteragir1.onClick.RemoveListener(AoClicarNoBotao);
    }

    private void AoClicarNoBotao()
    {
        if (!dialogoGlobal)
        {
            if (!falando) IniciarDialogoLocal();
            else AvancarDialogoLocal();
        }
        else
        {
            if (!falando) photonView.RPC("IniciarDialogoGlobal", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
            else if (photonViewDoIniciador != null && photonViewDoIniciador.IsMine)
                photonView.RPC("AvancarDialogoGlobal", RpcTarget.AllBuffered);
        }
    }

    private void IniciarDialogoLocal()
    {
        linhaAtual = 0;
        painelDialogo1.SetActive(true);
        textoDialogo1.text = linhasDialogo[linhaAtual];
        falando = true;
        indicadorNPC?.MarcarComoConversado();
    }

    private void AvancarDialogoLocal()
    {
        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            textoDialogo1.text = linhasDialogo[linhaAtual];
        }
        else
        {
            FinalizarDialogoLocal();
        }
    }

    private void FinalizarDialogoLocal()
    {
        painelDialogo1.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }

    [PunRPC]
    private void IniciarDialogoGlobal(int actorID)
    {
        linhaAtual = 0;
        painelDialogo1.SetActive(true);
        painelDialogo2.SetActive(true);
        textoDialogo1.text = linhasDialogo[linhaAtual];
        textoDialogo2.text = linhasDialogo[linhaAtual];
        falando = true;

        photonViewDoIniciador = PhotonView.Find(PhotonNetwork.CurrentRoom.GetPlayer(actorID).ActorNumber);
    }

    [PunRPC]
    private void AvancarDialogoGlobal()
    {
        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            textoDialogo1.text = linhasDialogo[linhaAtual];
            textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            painelDialogo1.SetActive(false);
            painelDialogo2.SetActive(false);
            falando = false;
            linhaAtual = 0;
        }
    }

    public void MostrarBotao()
    {
        jogadorPerto = true;
        botaoInteragir1.gameObject.SetActive(true);
    }

    public void EsconderTudo()
    {
        jogadorPerto = false;
        botaoInteragir1.gameObject.SetActive(false);
        painelDialogo1.SetActive(false);
        painelDialogo2.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }
}
