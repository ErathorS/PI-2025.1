using UnityEngine;
using TMPro;
using Photon.Pun;

public class ColetarCaixasManager : MonoBehaviourPun
{
    public static ColetarCaixasManager instancia;

    [Header("Caixas na cena (originais)")]
    public CaixaDeIngrediente[] caixasOriginais;

    private Vector3[] posicoesIniciais;
    private Quaternion[] rotacoesIniciais;

    [Header("UI")]
    public TMP_Text textoCaixas;
    public TMP_Text textoTempo;
    public GameObject botaoReset;

    [Header("Config")]
    public float tempoLimite = 60f;
    private float tempoAtual;
    
    // NOVO: Tornar público para acesso do MissaoFase3Manager
    public int caixasColetadas = 0;
    public int totalCaixas;
    
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
            if (caixasOriginais[i] != null)
            {
                posicoesIniciais[i] = caixasOriginais[i].transform.position;
                rotacoesIniciais[i] = caixasOriginais[i].transform.rotation;
                caixasOriginais[i].gameObject.SetActive(false);
            }
        }
    }

    // Chamado apenas pelo MasterClient
    public void AdicionarColetaMaster()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        caixasColetadas++;
        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);

        if (caixasColetadas >= totalCaixas)
        {
            faseAtiva = false;
            // NOVO: Notificar NPC que tarefa está concluída
            NotificarTarefaConcluida();
        }
    }

    [PunRPC]
    private void RPC_SincronizarUI(int novoValor)
    {
        caixasColetadas = novoValor;
        AtualizarUI();
    }

    // NOVO: Método para notificar que a tarefa foi concluída
    private void NotificarTarefaConcluida()
    {
        // Encontra o NPC da Zona 1 e notifica que a tarefa está concluída
        DialogoNPC[] npcs = FindObjectsOfType<DialogoNPC>();
        foreach (DialogoNPC npc in npcs)
        {
            if (npc.ehNPCZona1)
            {
                npc.TarefaConcluida();
                Debug.Log("[ColetarCaixasManager] NPC Zona 1 notificado sobre conclusão da tarefa!");
                break;
            }
        }
    }

    // Ativar caixas no inicio
    public void AtivarCaixasParaMissao()
    {
        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        for (int i = 0; i < totalCaixas; i++)
        {
            if (caixasOriginais[i] != null)
            {
                caixasOriginais[i].transform.position = posicoesIniciais[i];
                caixasOriginais[i].transform.rotation = rotacoesIniciais[i];
                caixasOriginais[i].gameObject.SetActive(true);
                caixasOriginais[i].ResetarCaixa();
            }
        }

        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);
        
        if (botaoReset != null)
            botaoReset.SetActive(false);
    }

    private void Update()
    {
        if (!faseAtiva) return;

        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0)
        {
            tempoAtual = 0;
            faseAtiva = false;

            if (PhotonNetwork.IsMasterClient && botaoReset != null)
                botaoReset.SetActive(true);
        }

        AtualizarUI();
    }

    private void AtualizarUI()
    {
        if (textoCaixas != null)
            textoCaixas.text = $"Caixas: {caixasColetadas}/{totalCaixas}";

        if (textoTempo != null)
            textoTempo.text = $"{Mathf.RoundToInt(tempoAtual)}s";
    }

    public void ResetarFase()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        for (int i = 0; i < totalCaixas; i++)
        {
            if (caixasOriginais[i] != null)
            {
                caixasOriginais[i].transform.position = posicoesIniciais[i];
                caixasOriginais[i].transform.rotation = rotacoesIniciais[i];
                caixasOriginais[i].gameObject.SetActive(true);
                caixasOriginais[i].ResetarCaixa();
            }
        }

        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);

        if (botaoReset != null)
            botaoReset.SetActive(false);
    }

    // NOVO: Método para verificar se a missão está ativa
    public bool IsMissaoAtiva()
    {
        return faseAtiva;
    }
}