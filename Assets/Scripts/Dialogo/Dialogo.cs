using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Dialogo : MonoBehaviourPun
{
    [Header("Referências")]
    public Button botaoDialogo1;
    public Button botaoDialogo2;
    public GameObject painelDialogo1;
    public GameObject painelDialogo2;
    public TMP_Text textoDialogo1;
    public TMP_Text textoDialogo2;
    public IndicadorNpc indicadorNPC;

    [Header("Configuração")]
    public bool dialogoGlobal = false;

    [Header("Linhas do Diálogo")]
    [TextArea(2, 4)]
    [SerializeField]
    private string[] linhasDialogo = {
        "",
        "Olá, viajante!",
        "Você está pronto para uma nova aventura?",
        "Boa sorte no seu caminho!"
    };

    private int linhaAtual = 0;
    private bool falando = false;
    private bool jogadorPerto = false;

    private GameObject jogadorAtual;
    private PhotonView photonViewDoIniciador;

    void Start()
    {
        botaoDialogo1.gameObject.SetActive(false);
        botaoDialogo2.gameObject.SetActive(false);
        painelDialogo1.SetActive(false);
        painelDialogo2.SetActive(false);

        botaoDialogo1.onClick.AddListener(() => AoClicarNoBotao("PI_MC_1"));
        botaoDialogo2.onClick.AddListener(() => AoClicarNoBotao("PI_MC_2"));
    }

    void OnDestroy()
    {
        botaoDialogo1.onClick.RemoveAllListeners();
        botaoDialogo2.onClick.RemoveAllListeners();
    }

    private void AoClicarNoBotao(string nomeJogador)
    {
        if (!dialogoGlobal)
        {
            if (!falando) IniciarDialogoLocal(nomeJogador);
            else AvancarDialogoLocal(nomeJogador);
        }
        else
        {
            if (!falando)
            {
                photonView.RPC("IniciarDialogoGlobal", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (photonViewDoIniciador != null && photonViewDoIniciador.IsMine)
            {
                photonView.RPC("AvancarDialogoGlobal", RpcTarget.AllBuffered);
            }
        }
    }

    private void IniciarDialogoLocal(string nomeJogador)
    {
        linhaAtual = 0;
        falando = true;

        if (nomeJogador.Contains("PI_MC_1"))
        {
            painelDialogo1.SetActive(true);
            textoDialogo1.text = linhasDialogo[linhaAtual];
        }
        else if (nomeJogador.Contains("PI_MC_2"))
        {
            painelDialogo2.SetActive(true);
            textoDialogo2.text = linhasDialogo[linhaAtual];
        }

        indicadorNPC?.MarcarComoConversado();
    }

    private void AvancarDialogoLocal(string nomeJogador)
    {
        linhaAtual++;

        if (linhaAtual < linhasDialogo.Length)
        {
            if (nomeJogador.Contains("PI_MC_1"))
                textoDialogo1.text = linhasDialogo[linhaAtual];
            else if (nomeJogador.Contains("PI_MC_2"))
                textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            FinalizarDialogoLocal();
        }
    }

    private void FinalizarDialogoLocal()
    {
        painelDialogo1.SetActive(false);
        painelDialogo2.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }

    [PunRPC]
    private void IniciarDialogoGlobal(int actorID)
    {
        linhaAtual = 0;
        falando = true;
        painelDialogo1.SetActive(true);
        painelDialogo2.SetActive(true);
        textoDialogo1.text = linhasDialogo[linhaAtual];
        textoDialogo2.text = linhasDialogo[linhaAtual];

        photonViewDoIniciador = PhotonView.Find(actorID);
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

    public void MostrarBotao(GameObject jogador)
    {
        jogadorPerto = true;
        jogadorAtual = jogador;

        string nomeJogador = jogador.name;

        if (nomeJogador.Contains("PI_MC_1"))
        {
            botaoDialogo1.gameObject.SetActive(true);
        }
        else if (nomeJogador.Contains("PI_MC_2"))
        {
            botaoDialogo2.gameObject.SetActive(true);
        }
    }

    public void EsconderTudo()
    {
        jogadorPerto = false;
        botaoDialogo1.gameObject.SetActive(false);
        botaoDialogo2.gameObject.SetActive(false);
        painelDialogo1.SetActive(false);
        painelDialogo2.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }
}
