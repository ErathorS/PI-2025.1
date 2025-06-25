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
    public bool dialogoGlobal = false; // Define se é local ou global

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

    private PhotonView photonViewDoIniciador; // Quem iniciou o diálogo

    void Start()
    {
        // Procurar todos os jogadores na cena
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject jogador in jogadores)
        {
            PhotonView pv = jogador.GetComponent<PhotonView>();

            if (pv != null && pv.IsMine)
            {
                // Encontrou o jogador local, agora pegamos o canvas dele
                Transform canvas = jogador.transform.Find("Canvas");

                if (canvas != null)
                {
                    // Atribui dinamicamente os elementos do canvas do jogador local
                    painelDialogo1 = canvas.Find("PainelDialogo1")?.gameObject;
                    painelDialogo2 = canvas.Find("PainelDialogo2")?.gameObject;
                    textoDialogo1 = canvas.Find("PainelDialogo1/TextoDialogo1")?.GetComponent<TMP_Text>();
                    textoDialogo2 = canvas.Find("PainelDialogo2/TextoDialogo2")?.GetComponent<TMP_Text>();
                    botaoDialogo1 = canvas.Find("BotaoInteragir1")?.GetComponent<Button>();
                    botaoDialogo2 = canvas.Find("BotaoInteragir2")?.GetComponent<Button>();

                    if (botaoDialogo1 != null)
                        botaoDialogo1.onClick.AddListener(AoClicarNoBotao);

                    break; // já encontrou o jogador local, não precisa continuar
                }
            }
        }

        // Se necessário, desativar o botão até o jogador estar perto
        if (botaoDialogo1 != null) botaoDialogo1.gameObject.SetActive(false);
        if (painelDialogo1 != null) painelDialogo1.SetActive(false);
    }

    void OnDestroy()
    {
        if (botaoDialogo1 != null)
            botaoDialogo1.onClick.RemoveListener(AoClicarNoBotao);
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
            if (!falando)
                photonView.RPC("IniciarDialogoGlobal", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
            else if (photonViewDoIniciador != null && photonViewDoIniciador.IsMine)
                photonView.RPC("AvancarDialogoGlobal", RpcTarget.AllBuffered);
        }
    }

    private void IniciarDialogoLocal()
    {
        linhaAtual = 0;
        if (painelDialogo1 != null) painelDialogo1.SetActive(true);
        if (textoDialogo1 != null) textoDialogo1.text = linhasDialogo[linhaAtual];
        falando = true;
        indicadorNPC?.MarcarComoConversado();
    }

    private void AvancarDialogoLocal()
    {
        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            if (textoDialogo1 != null) textoDialogo1.text = linhasDialogo[linhaAtual];
        }
        else
        {
            FinalizarDialogoLocal();
        }
    }

    private void FinalizarDialogoLocal()
    {
        if (painelDialogo1 != null) painelDialogo1.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }

    [PunRPC]
    private void IniciarDialogoGlobal(int actorID)
    {
        linhaAtual = 0;
        if (painelDialogo1 != null) painelDialogo1.SetActive(true);
        if (painelDialogo2 != null) painelDialogo2.SetActive(true);
        if (textoDialogo1 != null) textoDialogo1.text = linhasDialogo[linhaAtual];
        if (textoDialogo2 != null) textoDialogo2.text = linhasDialogo[linhaAtual];
        falando = true;

        photonViewDoIniciador = photonView; // Atribuir o photonView local como iniciador
    }

    [PunRPC]
    private void AvancarDialogoGlobal()
    {
        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            if (textoDialogo1 != null) textoDialogo1.text = linhasDialogo[linhaAtual];
            if (textoDialogo2 != null) textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            if (painelDialogo1 != null) painelDialogo1.SetActive(false);
            if (painelDialogo2 != null) painelDialogo2.SetActive(false);
            falando = false;
            linhaAtual = 0;
        }
    }

    public void MostrarBotao()
    {
        jogadorPerto = true;
        if (botaoDialogo1 != null)
            botaoDialogo1.gameObject.SetActive(true);
    }

    public void EsconderTudo()
    {
        jogadorPerto = false;
        if (botaoDialogo1 != null)
            botaoDialogo1.gameObject.SetActive(false);
        if (painelDialogo1 != null)
            painelDialogo1.SetActive(false);
        if (painelDialogo2 != null)
            painelDialogo2.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }
}
