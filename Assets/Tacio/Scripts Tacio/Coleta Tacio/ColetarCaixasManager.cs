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
    
    public int caixasColetadas = 0;
    public int totalCaixas;
    
    private bool faseAtiva = false;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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
        Debug.Log($"[ColetarCaixasManager] Caixa coletada! Total: {caixasColetadas}/{totalCaixas}");

        // CORREÇÃO: Sincronizar com todos os jogadores
        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);

        if (caixasColetadas >= totalCaixas)
        {
            faseAtiva = false;
            Debug.Log("[ColetarCaixasManager] TODAS as caixas coletadas! Finalizando missão...");
            
            // CORREÇÃO: Notificar MissaoFase1Manager para finalizar a missão
            if (MissaoFase1Manager.instancia != null)
            {
                MissaoFase1Manager.instancia.MissaoFinalizada();
            }
            else
            {
                Debug.LogError("[ColetarCaixasManager] MissaoFase1Manager não encontrado!");
            }
        }
    }

    [PunRPC]
    private void RPC_SincronizarUI(int novoValor)
    {
        caixasColetadas = novoValor;
        AtualizarUI();
        Debug.Log($"[ColetarCaixasManager] UI sincronizada: {caixasColetadas}/{totalCaixas}");
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

        // CORREÇÃO: Sincronizar estado inicial
        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);
        
        if (botaoReset != null)
            botaoReset.SetActive(false);
            
        Debug.Log("[ColetarCaixasManager] Caixas ativadas para missão");
    }

    private void Update()
    {
        if (!faseAtiva) return;

        // CORREÇÃO: Apenas Master controla o timer
        if (PhotonNetwork.IsMasterClient)
        {
            tempoAtual -= Time.deltaTime;

            if (tempoAtual <= 0)
            {
                tempoAtual = 0;
                faseAtiva = false;

                if (botaoReset != null)
                    botaoReset.SetActive(true);
            }
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

        // CORREÇÃO: Sincronizar reset
        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);

        if (botaoReset != null)
            botaoReset.SetActive(false);
            
        Debug.Log("[ColetarCaixasManager] Fase resetada");
    }

    // Método para verificar se a missão está ativa
    public bool IsMissaoAtiva()
    {
        return faseAtiva;
    }
}