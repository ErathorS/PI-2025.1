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
    public GameObject objetoUI; // GameObject da UI para controlar

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
        // Pega o nome da cena atual
        cenaAtual = SceneManager.GetActiveScene().name;
        Debug.Log($"[LoadingTravel] Cena atual: {cenaAtual}");
        
        // Configura a UI baseado na cena atual
        ConfigurarUIBaseadoNaCena();
    }

    void Start()
    {
        tempo = 0f;
        if (barra != null)
            barra.fillAmount = 0f;
            
        carregando = false;
        cenaCarregada = false;

        // DEBUG: Verificar estado da UI
        if (objetoUI != null)
        {
            Debug.Log($"[LoadingTravel] Estado inicial da UI: {objetoUI.activeSelf} na cena {cenaAtual}");
        }

        // Se não estiver conectado ao Photon
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("[LoadingTravel] Não conectado ao Photon - modo single player");
            carregando = true;
        }
        else
        {
            // Verifica se pode iniciar o carregamento
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

        // Atualiza a barra
        if (barra != null)
            barra.fillAmount = t;

        // Move o avião
        if (aviao != null && pontoInicial != null && pontoFinal != null)
        {
            aviao.position = Vector3.Lerp(pontoInicial.position, pontoFinal.position, t);
        }

        // Quando terminar
        if (t >= 1f && !cenaCarregada)
        {
            cenaCarregada = true;
            CarregarCena();
        }
    }

    // CORREÇÃO: Método corrigido para configurar a UI
    private void ConfigurarUIBaseadoNaCena()
    {
        if (objetoUI == null)
        {
            Debug.LogWarning("[LoadingTravel] objetoUI não está atribuído!");
            return;
        }

        bool deveDesativar = false;
        
        // Verifica se a cena atual está na lista de cenas para DESATIVAR
        foreach (string cena in cenasDesativarUI)
        {
            if (cenaAtual.Trim().Equals(cena.Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                deveDesativar = true;
                Debug.Log($"[LoadingTravel] ✓ Cena '{cenaAtual}' está na lista - UI será DESATIVADA");
                break;
            }
        }

        // Aplica o estado correto
        if (deveDesativar)
        {
            // Se está na lista: DESATIVA a UI
            if (objetoUI.activeSelf)
            {
                objetoUI.SetActive(false);
                Debug.Log($"[LoadingTravel] UI desativada para cena: {cenaAtual}");
            }
        }
        else
        {
            // Se NÃO está na lista: ATIVA a UI
            if (!objetoUI.activeSelf)
            {
                objetoUI.SetActive(true);
                Debug.Log($"[LoadingTravel] UI ativada para cena: {cenaAtual}");
            }
        }
    }

    // Método para verificar se uma cena deve ter UI desativada
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

        // Sincroniza com outros jogadores
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

        // Verifica se a cena destino precisa de UI desativada
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
            // Modo single player
            SceneManager.LoadScene(cenaDestino);
        }
    }

    // Método público para ser chamado por outros scripts
    public void IniciarCarregamentoParaCena(string cena)
    {
        if (!string.IsNullOrEmpty(cena))
        {
            cenaDestino = cena;
            Debug.Log($"[LoadingTravel] Cena destino alterada para: {cenaDestino}");
        }

        IniciarCarregamento();
    }

    // Método para ser chamado pelo PainelFinalFaseController
    public void ConfigurarEDisparar(string proximaCena)
    {
        if (!string.IsNullOrEmpty(proximaCena))
        {
            cenaDestino = proximaCena;
            Debug.Log($"[LoadingTravel] Configurado para carregar: {cenaDestino}");
        }

        IniciarCarregamento();
    }

    // NOVO: Método para forçar ativação/desativação da UI
    public void ForcarEstadoUI(bool ativar)
    {
        if (objetoUI != null)
        {
            objetoUI.SetActive(ativar);
            Debug.Log($"[LoadingTravel] UI forçada para: {(ativar ? "ATIVADA" : "DESATIVADA")}");
        }
    }

    // NOVO: Método para verificar o estado atual
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

    // NOVO: Método para adicionar debug visual no Editor
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        
        Debug.Log($"[LoadingTravel Debug] Cena: {cenaAtual}, UI Ativa: {objetoUI?.activeSelf}");
    }

    // Método para debug
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