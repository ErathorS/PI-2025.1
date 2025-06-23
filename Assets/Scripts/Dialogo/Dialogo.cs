using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogo : MonoBehaviour
{
    [Header("Referências")]
    public Button botaoInteragir;         // Botão que o jogador toca para falar/avançar
    public GameObject painelDialogo;      // Painel de fundo do texto
    public TMP_Text textoDialogo;         // Componente de texto (TextMeshPro)
    public IndicadorNpc indicadorNPC;     // (Opcional) controle do NPC

    [Header("Linhas do Diálogo")]
    [TextArea(2, 4)]
    [SerializeField] private string[] linhasDialogo = {
        "",
        "Olá, viajante!",
        "Você está pronto para uma nova aventura?",
        "Boa sorte no seu caminho!"
    };

    private int linhaAtual = 0;
    private bool jogadorPerto = false;
    private bool falando = false;

    void Start()
    {
        botaoInteragir.gameObject.SetActive(false);
        painelDialogo.SetActive(false);

        botaoInteragir.onClick.AddListener(AoClicarNoBotao);
    }

    void OnDestroy()
    {
        botaoInteragir.onClick.RemoveListener(AoClicarNoBotao);
    }

    private void AoClicarNoBotao()
    {
        if (!falando)
        {
            IniciarDialogo(); // primeiro clique
        }
        else
        {
            AvancarDialogo(); // próximos cliques
        }
    }

    private void IniciarDialogo()
    {
        linhaAtual = 0;
        painelDialogo.SetActive(true);
        textoDialogo.text = linhasDialogo[linhaAtual];
        falando = true;

        indicadorNPC?.MarcarComoConversado();
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
        falando = false;
        linhaAtual = 0;
        // Botão continua ativo, pode recomeçar a conversa se quiser
    }

    public void MostrarBotao()
    {
        jogadorPerto = true;
        botaoInteragir.gameObject.SetActive(true);
    }

    public void EsconderTudo()
    {
        jogadorPerto = false;
        botaoInteragir.gameObject.SetActive(false);
        painelDialogo.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }
}
