using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LoadingTravel : MonoBehaviourPunCallbacks
{
    [Header("Referências")]
    public Image barra;
    public RectTransform aviao;
    public RectTransform pontoInicial;
    public RectTransform pontoFinal;
    public GameObject objetoUI; 

    [Header("Configuração")]
    public float duracao = 4f;
    public string cenaDestino = "MinhaCena";
    public bool apenasMasterCarrega = true;

    [Header("Cenas onde a UI será DESATIVADA")]
    [Tooltip("A UI ficará DESATIVADA apenas nestas cenas")]
    public string[] cenasDesativarUI = { "Viajando sao paulo", "Viajando rio", "Final" };

    private float tempo;
    private bool carregando = false;
    private bool cenaCarregada = false;
    private string cenaAtual;

    void Awake()
    {
        cenaAtual = SceneManager.GetActiveScene().name;
        Debug.Log($"[LoadingTravel] Cena atual: {cenaAtual}");
        
        ConfigurarUIBaseadoNaCena();
    }

    void Start()
    {
        tempo = 0f;
        if (barra != null)
            barra.fillAmount = 0f;
            
        carregando = false;
        cenaCarregada = false;

        if (objetoUI != null)
        {
            Debug.Log($"[LoadingTravel] Estado inicial da UI: {objetoUI.activeSelf} na cena {cenaAtual}");
        }

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("[LoadingTravel] Não conectado ao Photon - modo single player");
            carregando = true;
        }
        else
        {
            if (!apenasMasterCarrega || PhotonNetwork.IsMasterClient)
            {
                IniciarCarregamento();
            }
            else
            {
                Debug.Log("[LoadingTravel] Aguardando Master iniciar o carregamento...");
            }
        }
    }

    void Update()
    {
        if (!carregando) return;

        tempo += Time.deltaTime;
        
        float t = Mathf.Clamp01(tempo / duracao);

        if (barra != null)
            barra.fillAmount = t;

        if (aviao != null && pontoInicial != null && pontoFinal != null)
        {
            aviao.position = Vector3.Lerp(pontoInicial.position, pontoFinal.position, t);
        }

        if (t >= 1f && !cenaCarregada)
        {
            cenaCarregada = true;
            CarregarCena();
        }
    }

    private void ConfigurarUIBaseadoNaCena()
    {
        if (objetoUI == null)
        {
            Debug.LogWarning("[LoadingTravel] objetoUI não está atribuído!");
            return;
        }

        bool deveDesativar = false;
        
        foreach (string cena in cenasDesativarUI)
        {
            if (cenaAtual.Trim().Equals(cena.Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                deveDesativar = true;
                Debug.Log($"[LoadingTravel] ✓ Cena '{cenaAtual}' está na lista - UI será DESATIVADA");
                break;
            }
        }

        if (deveDesativar)
        {
            if (objetoUI.activeSelf)
            {
                objetoUI.SetActive(false);
                Debug.Log($"[LoadingTravel] UI desativada para cena: {cenaAtual}");
            }
        }
        else
        {
            if (!objetoUI.activeSelf)
            {
                objetoUI.SetActive(true);
                Debug.Log($"[LoadingTravel] UI ativada para cena: {cenaAtual}");
            }
        }
    }

    private bool CenaDeveTerUIDesativada(string nomeCena)
    {
        if (string.IsNullOrEmpty(nomeCena)) return false;
        
        foreach (string cena in cenasDesativarUI)
        {
            if (nomeCena.Trim().Equals(cena.Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private void IniciarCarregamento()
    {
        carregando = true;
        Debug.Log($"[LoadingTravel] Iniciando carregamento para cena: {cenaDestino}");

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_IniciarCarregamento", RpcTarget.Others);
        }
    }

    [PunRPC]
    private void RPC_IniciarCarregamento()
    {
        if (!carregando)
        {
            carregando = true;
            Debug.Log($"[LoadingTravel] Carregamento iniciado remotamente por Master");
        }
    }

    private void CarregarCena()
    {
        if (string.IsNullOrEmpty(cenaDestino))
        {
            Debug.LogError("[LoadingTravel] Nome da cena destino não definido!");
            return;
        }

        Debug.Log($"[LoadingTravel] Carregando cena: {cenaDestino}");

        if (CenaDeveTerUIDesativada(cenaDestino))
        {
            Debug.Log($"[LoadingTravel] Cena destino '{cenaDestino}' requer UI desativada");
        }

        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"[LoadingTravel] Master carregando cena para todos: {cenaDestino}");
                PhotonNetwork.LoadLevel(cenaDestino);
            }
        }
        else
        {
            SceneManager.LoadScene(cenaDestino);
        }
    }

    public void IniciarCarregamentoParaCena(string cena)
    {
        if (!string.IsNullOrEmpty(cena))
        {
            cenaDestino = cena;
            Debug.Log($"[LoadingTravel] Cena destino alterada para: {cenaDestino}");
        }

        IniciarCarregamento();
    }

    public void ConfigurarEDisparar(string proximaCena)
    {
        if (!string.IsNullOrEmpty(proximaCena))
        {
            cenaDestino = proximaCena;
            Debug.Log($"[LoadingTravel] Configurado para carregar: {cenaDestino}");
        }

        IniciarCarregamento();
    }

    public void ForcarEstadoUI(bool ativar)
    {
        if (objetoUI != null)
        {
            objetoUI.SetActive(ativar);
            Debug.Log($"[LoadingTravel] UI forçada para: {(ativar ? "ATIVADA" : "DESATIVADA")}");
        }
    }

    public void VerificarEstadoAtual()
    {
        Debug.Log($"[LoadingTravel] === VERIFICAÇÃO ===");
        Debug.Log($"Cena Atual: '{cenaAtual}'");
        Debug.Log($"UI Ativa: {(objetoUI != null ? objetoUI.activeSelf : "N/A")}");
        Debug.Log($"Cenas para desativar UI:");
        
        foreach (string cena in cenasDesativarUI)
        {
            bool corresponde = cenaAtual.Trim().Equals(cena.Trim(), System.StringComparison.OrdinalIgnoreCase);
            Debug.Log($"  - '{cena}' {(corresponde ? "✓ CORRESPONDE" : "")}");
        }
        
        Debug.Log($"Cena destino: {cenaDestino}");
        Debug.Log($"Deve desativar UI na cena atual: {CenaDeveTerUIDesativada(cenaAtual)}");
        Debug.Log($"==================================");
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        
        Debug.Log($"[LoadingTravel Debug] Cena: {cenaAtual}, UI Ativa: {objetoUI?.activeSelf}");
    }

    public void DebugInfo()
    {
        Debug.Log($"[LoadingTravel] === DEBUG ===");
        Debug.Log($"Cena Atual: '{cenaAtual}'");
        Debug.Log($"Cena Destino: '{cenaDestino}'");
        Debug.Log($"Carregando: {carregando}");
        Debug.Log($"Cena Carregada: {cenaCarregada}");
        Debug.Log($"Tempo: {tempo}/{duracao}");
        Debug.Log($"Progresso: {(barra != null ? barra.fillAmount * 100 : 0)}%");
        Debug.Log($"Photon Conectado: {PhotonNetwork.IsConnected}");
        Debug.Log($"Master Client: {PhotonNetwork.IsMasterClient}");
        Debug.Log($"ObjetoUI Ativo: {(objetoUI != null ? objetoUI.activeSelf : false)}");
        
        Debug.Log($"Cenas para desativar UI:");
        foreach (string cena in cenasDesativarUI)
        {
            Debug.Log($"  - '{cena}'");
        }
        Debug.Log($"==================================");
    }
}