using UnityEngine;
using TMPro;
using Photon.Pun;

public class ColetarCaixasManager : MonoBehaviourPun
{
    public static ColetarCaixasManager instancia;

    [Header("Caixas na cena")]
    public CaixaDeIngrediente[] caixasOriginais;
    public GameObject prefabCaixa;

    private Vector3[] posicoesIniciais;
    private Quaternion[] rotacoesIniciais;

    [Header("UI")]
    public TMP_Text textoCaixas;
    public TMP_Text textoTempo;
    public GameObject botaoReset;

    [Header("Config")]
    public float tempoLimite = 60f;
    private float tempoAtual;
    private int caixasColetadas = 0;
    private int totalCaixas;
    private bool faseAtiva = false;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        totalCaixas = caixasOriginais.Length;

        posicoesIniciais = new Vector3[totalCaixas];
        rotacoesIniciais = new Quaternion[totalCaixas];

        for (int i = 0; i < totalCaixas; i++)
        {
            posicoesIniciais[i] = caixasOriginais[i].transform.position;
            rotacoesIniciais[i] = caixasOriginais[i].transform.rotation;

            caixasOriginais[i].gameObject.SetActive(false);
        }
    }

    public void AtivarCaixasParaMissao()
    {
        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        for (int i = 0; i < totalCaixas; i++)
        {
            caixasOriginais[i].transform.position = posicoesIniciais[i];
            caixasOriginais[i].transform.rotation = rotacoesIniciais[i];
            caixasOriginais[i].gameObject.SetActive(true);
        }

        AtualizarUI();
        botaoReset.SetActive(false);
    }

    public void RegistrarColeta()
    {
        caixasColetadas++;
        AtualizarUI();

        if (caixasColetadas >= totalCaixas)
        {
            faseAtiva = false;
            MissaoFase1Manager.instancia.MissaoFinalizada();
        }
    }

    private void Update()
    {
        if (!faseAtiva) return;

        tempoAtual -= Time.deltaTime;
        if (tempoAtual <= 0)
        {
            tempoAtual = 0;
            faseAtiva = false;

            botaoReset.SetActive(true);
        }

        AtualizarUI();
    }

    public void ResetarFase()
    {
        foreach (var caixa in GameObject.FindGameObjectsWithTag("Caixa"))
            Destroy(caixa);

        for (int i = 0; i < totalCaixas; i++)
            Instantiate(prefabCaixa, posicoesIniciais[i], rotacoesIniciais[i]);

        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        botaoReset.SetActive(false);
        AtualizarUI();
    }

    private void AtualizarUI()
    {
        textoCaixas.text = $"Caixas: {caixasColetadas}/{totalCaixas}";
        textoTempo.text = $"{Mathf.RoundToInt(tempoAtual)}s";
    }
}
