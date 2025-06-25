using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Dialogo : MonoBehaviourPun
{
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

    // UI automática
    private Button botaoInteragir1;
    private Button botaoInteragir2;
    private GameObject painelDialogo1;
    private GameObject painelDialogo2;
    private TMP_Text textoDialogo1;
    private TMP_Text textoDialogo2;

    public IndicadorNpc indicadorNPC;

    void Awake()
    {
        EncontrarElementosUI();
    }

    void Start()
    {
        if (botaoInteragir1) botaoInteragir1.onClick.AddListener(() => AoClicarNoBotao("PI_MC_1"));
        if (botaoInteragir2) botaoInteragir2.onClick.AddListener(() => AoClicarNoBotao("PI_MC_2"));

        if (botaoInteragir1) botaoInteragir1.gameObject.SetActive(false);
        if (botaoInteragir2) botaoInteragir2.gameObject.SetActive(false);
        if (painelDialogo1) painelDialogo1.SetActive(false);
        if (painelDialogo2) painelDialogo2.SetActive(false);
    }

    void OnDestroy()
    {
        if (botaoInteragir1) botaoInteragir1.onClick.RemoveAllListeners();
        if (botaoInteragir2) botaoInteragir2.onClick.RemoveAllListeners();
    }

    private void EncontrarElementosUI()
    {
        GameObject canvas1 = GameObject.FindWithTag("CanvasP1");
        GameObject canvas2 = GameObject.FindWithTag("CanvasP2");

        if (canvas1)
        {
            botaoInteragir1 = canvas1.transform.Find("BotaoDialogo")?.GetComponent<Button>();
            painelDialogo1 = canvas1.transform.Find("PainelDialogo")?.gameObject;
            textoDialogo1 = painelDialogo1?.transform.Find("TMP_Text")?.GetComponent<TMP_Text>();
        }

        if (canvas2)
        {
            botaoInteragir2 = canvas2.transform.Find("BotaoDialogo")?.GetComponent<Button>();
            painelDialogo2 = canvas2.transform.Find("PainelDialogo")?.gameObject;
            textoDialogo2 = painelDialogo2?.transform.Find("TMP_Text")?.GetComponent<TMP_Text>();
        }
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
                photonView.RPC("IniciarDialogoGlobal", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
            else if (photonViewDoIniciador != null && photonViewDoIniciador.IsMine)
                photonView.RPC("AvancarDialogoGlobal", RpcTarget.AllBuffered);
        }
    }

    private void IniciarDialogoLocal(string nomeJogador)
    {
        linhaAtual = 0;
        falando = true;

        if (nomeJogador.Contains("PI_MC_1"))
        {
            painelDialogo1?.SetActive(true);
            if (textoDialogo1) textoDialogo1.text = linhasDialogo[linhaAtual];
        }
        else if (nomeJogador.Contains("PI_MC_2"))
        {
            painelDialogo2?.SetActive(true);
            if (textoDialogo2) textoDialogo2.text = linhasDialogo[linhaAtual];
        }

        indicadorNPC?.MarcarComoConversado();
    }

    private void AvancarDialogoLocal(string nomeJogador)
    {
        linhaAtual++;

        if (linhaAtual < linhasDialogo.Length)
        {
            if (nomeJogador.Contains("PI_MC_1") && textoDialogo1)
                textoDialogo1.text = linhasDialogo[linhaAtual];
            else if (nomeJogador.Contains("PI_MC_2") && textoDialogo2)
                textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            FinalizarDialogoLocal();
        }
    }

    private void FinalizarDialogoLocal()
    {
        painelDialogo1?.SetActive(false);
        painelDialogo2?.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }

    [PunRPC]
    private void IniciarDialogoGlobal(int actorID)
    {
        linhaAtual = 0;
        falando = true;

        painelDialogo1?.SetActive(true);
        painelDialogo2?.SetActive(true);

        if (textoDialogo1) textoDialogo1.text = linhasDialogo[linhaAtual];
        if (textoDialogo2) textoDialogo2.text = linhasDialogo[linhaAtual];

        photonViewDoIniciador = PhotonView.Find(actorID);
    }

    [PunRPC]
    private void AvancarDialogoGlobal()
    {
        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            if (textoDialogo1) textoDialogo1.text = linhasDialogo[linhaAtual];
            if (textoDialogo2) textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            painelDialogo1?.SetActive(false);
            painelDialogo2?.SetActive(false);
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
            if (botaoInteragir1) botaoInteragir1.gameObject.SetActive(true);
        }
        else if (nomeJogador.Contains("PI_MC_2"))
        {
            if (botaoInteragir2) botaoInteragir2.gameObject.SetActive(true);
        }
    }

    public void EsconderTudo()
    {
        jogadorPerto = false;

        if (botaoInteragir1) botaoInteragir1.gameObject.SetActive(false);
        if (botaoInteragir2) botaoInteragir2.gameObject.SetActive(false);
        if (painelDialogo1) painelDialogo1.SetActive(false);
        if (painelDialogo2) painelDialogo2.SetActive(false);

        falando = false;
        linhaAtual = 0;
    }
}
