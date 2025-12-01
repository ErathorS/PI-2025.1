using UnityEngine;
using Photon.Pun;

public class ColetarCaixasManager : MonoBehaviourPun
{
    public static ColetarCaixasManager instancia;

    [Header("Caixas na cena (originais)")]
    public CaixaDeIngrediente[] caixasOriginais;

    private Vector3[] posicoesIniciais;
    private Quaternion[] rotacoesIniciais;

    [Header("Config")]
    public float tempoLimite = 60f;
    private float tempoAtual;
    
    public int caixasColetadas = 0;
    public int totalCaixas;
    
    private bool faseAtiva = false;
    
    [Header("Referências")]
    public GameObject botaoReset;

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

    public void AdicionarColetaMaster()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        caixasColetadas++;
        Debug.Log($"[ColetarCaixasManager] Caixa coletada! Total: {caixasColetadas}/{totalCaixas}");

        photonView.RPC("RPC_SincronizarContador", RpcTarget.AllBuffered, caixasColetadas);

        if (MissaoFase1Manager.instancia != null)
        {
            MissaoFase1Manager.instancia.CaixaColetada();
        }

        if (caixasColetadas >= totalCaixas)
        {
            faseAtiva = false;
            Debug.Log("[ColetarCaixasManager] TODAS as caixas coletadas! Finalizando missão...");
            
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
    private void RPC_SincronizarContador(int novoValor)
    {
        caixasColetadas = novoValor;
        Debug.Log($"[ColetarCaixasManager] Contador sincronizado: {caixasColetadas}/{totalCaixas}");
    }

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

        photonView.RPC("RPC_SincronizarContador", RpcTarget.AllBuffered, caixasColetadas);
        
        if (botaoReset != null)
            botaoReset.SetActive(false);
            
        Debug.Log("[ColetarCaixasManager] Caixas ativadas para missão");
    }

    private void Update()
    {
        if (!faseAtiva) return;

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

        photonView.RPC("RPC_SincronizarContador", RpcTarget.AllBuffered, caixasColetadas);

        if (botaoReset != null)
            botaoReset.SetActive(false);
            
        Debug.Log("[ColetarCaixasManager] Fase resetada");
    }

    public bool IsMissaoAtiva()
    {
        return faseAtiva;
    }
    
    public void DebugInfo()
    {
        Debug.Log($"[ColetarCaixasManager] === DEBUG ===");
        Debug.Log($"Caixas: {caixasColetadas}/{totalCaixas}");
        Debug.Log($"Fase Ativa: {faseAtiva}");
        Debug.Log($"Tempo Atual: {tempoAtual}");
        Debug.Log($"MasterClient: {PhotonNetwork.IsMasterClient}");
        Debug.Log($"Caixas Originais: {caixasOriginais.Length}");
        Debug.Log($"=================================");
    }
}