using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class NPCImportanteFase3 : MonoBehaviourPun
{
    [Header("Configuração Fase 3")]
    public int npcID = 1; // 1 ou 2 para identificar qual NPC é
    public bool requerSincronizacao = false;
    
    [Header("Dialogo")]
    [TextArea(2, 5)] public string[] falas;
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;
    public Button botaoAvancar;

    [Header("Referências")]
    public SincronizacaoManager sincronizacaoManager;
    public IndicadorNpc indicadorNPC;

    private int indiceFala = 0;
    private bool emDialogo = false;
    private bool tarefaConcluida = false;
    private bool missaoIniciada = false;

    void Start()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }

    public void IniciarDialogo()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (tarefaConcluida) return;

        emDialogo = true;
        indiceFala = 0;

        painelDialogo.SetActive(true);
        AtualizarFala();

        botaoAvancar.onClick.RemoveAllListeners();
        botaoAvancar.onClick.AddListener(ProximaFala);
    }

    void AtualizarFala()
    {
        if (textoDialogo != null && indiceFala < falas.Length)
            textoDialogo.text = falas[indiceFala];
    }

    public void ProximaFala()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        indiceFala++;
        if (indiceFala >= falas.Length)
        {
            EncerrarDialogo();
            return;
        }

        AtualizarFala();
    }

    void EncerrarDialogo()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        emDialogo = false;

        if (!tarefaConcluida && PhotonNetwork.IsMasterClient)
        {
            if (requerSincronizacao && !missaoIniciada)
            {
                // Inicia tarefa de sincronização
                if (sincronizacaoManager != null)
                {
                    sincronizacaoManager.IniciarSincronizacao();
                    missaoIniciada = true;
                }
            }
            else
            {
                // Tarefa simples - concluir diretamente
                ConcluirTarefa();
            }
        }
    }

    public void ConcluirTarefa()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        tarefaConcluida = true;
        
        // CORREÇÃO: Usando o método correto do MissaoFase3Manager
        if (MissaoFase3Manager.instancia != null)
        {
            if (npcID == 1)
            {
                MissaoFase3Manager.instancia.EntregarMissao1();
            }
            else if (npcID == 2)
            {
                MissaoFase3Manager.instancia.EntregarMissao2();
            }
        }

        // Atualizar indicador visual
        if (indicadorNPC != null)
        {
            indicadorNPC.MarcarComoConversado();
        }

        Debug.Log($"[NPCImportanteFase3] NPC {npcID} concluído!");
    }

    // Chamado pela sincronização quando concluída
    public void SincronizacaoConcluida()
    {
        if (!tarefaConcluida)
        {
            ConcluirTarefa();
        }
    }
}