using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SincronizacaoManager : MonoBehaviourPun
{
    public static SincronizacaoManager instancia;

    [Header("Configurações")]
    public float tempoMaximoSincronizacao = 5f;

    [Header("Referências")]
    public GameObject[] botoesSincronizacao;
    public PostePiscando[] postesPiscando;

    // Estado
    private HashSet<int> jogadoresQueTocaram = new HashSet<int>();
    private bool sincronizacaoAtiva = false;
    private bool sincronizacaoConcluida = false;
    private float tempoUltimoToque;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        EsconderBotoesSincronizacao();
    }

    public void IniciarSincronizacao()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (photonView == null)
        {
            Debug.LogError("[SincronizacaoManager] PhotonView não encontrado!");
            return;
        }

        photonView.RPC("RPC_IniciarSincronizacao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IniciarSincronizacao()
    {
        sincronizacaoAtiva = true;
        sincronizacaoConcluida = false;
        jogadoresQueTocaram.Clear();
        tempoUltimoToque = Time.time;

        MostrarBotoesSincronizacao();
        Debug.Log("[SincronizacaoManager] Sincronização iniciada!");
    }

    public void RegistrarToque(int actorID)
    {
        if (!sincronizacaoAtiva || sincronizacaoConcluida)
        {
            Debug.Log($"[SincronizacaoManager] Sincronização não está ativa, ignorando toque do jogador {actorID}");
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("[SincronizacaoManager] PhotonView é null no RegistrarToque!");
            return;
        }

        photonView.RPC("RPC_RegistrarToque", RpcTarget.All, actorID);
    }

    [PunRPC]
    private void RPC_RegistrarToque(int actorID)
    {
        if (!sincronizacaoAtiva || sincronizacaoConcluida)
        {
            Debug.Log($"[SincronizacaoManager] Sincronização não ativa no RPC, ignorando jogador {actorID}");
            return;
        }

        jogadoresQueTocaram.Add(actorID);
        tempoUltimoToque = Time.time;

        Debug.Log($"[SincronizacaoManager] Jogador {actorID} tocou. Total: {jogadoresQueTocaram.Count}/2");

        if (PhotonNetwork.IsMasterClient && jogadoresQueTocaram.Count >= 2)
        {
            Debug.Log($"[SincronizacaoManager] ✔️ Dois jogadores tocaram! Concluindo sincronização...");
            ConcluirSincronizacao();
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[SincronizacaoManager] Aguardando mais jogadores... ({jogadoresQueTocaram.Count}/2)");
        }
    }

    private void Update()
    {
        if (sincronizacaoAtiva && !sincronizacaoConcluida && PhotonNetwork.IsMasterClient)
        {
            if (Time.time - tempoUltimoToque > tempoMaximoSincronizacao)
            {
                ResetarSincronizacao();
            }
        }
    }

    private void ConcluirSincronizacao()
    {
        sincronizacaoConcluida = true;
        sincronizacaoAtiva = false;

        if (photonView == null)
        {
            Debug.LogError("[SincronizacaoManager] PhotonView é null no ConcluirSincronizacao!");
            return;
        }

        photonView.RPC("RPC_ConcluirSincronizacao", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ConcluirSincronizacao()
    {
        Debug.Log("[SincronizacaoManager] ✅✅✅ Sincronização concluída para todos os jogadores!");

        // Parar as luzes de piscar
        if (postesPiscando != null)
        {
            foreach (var poste in postesPiscando)
            {
                if (poste != null) poste.PararDePiscar();
            }
        }

        EsconderBotoesSincronizacao();

        // NOVO: Notificar NPC da Zona 2 que a tarefa está concluída
        NotificarNPCZona2();

        Debug.Log("[SincronizacaoManager] Tarefa de sincronização concluída!");
    }

    // NOVO: Método para notificar NPC da Zona 2
    private void NotificarNPCZona2()
    {
        DialogoNPC[] npcs = FindObjectsOfType<DialogoNPC>();
        foreach (DialogoNPC npc in npcs)
        {
            if (npc.ehNPCZona2)
            {
                npc.TarefaConcluida();
                Debug.Log("[SincronizacaoManager] NPC Zona 2 notificado sobre conclusão da tarefa!");
                break;
            }
        }
    }

    private void ResetarSincronizacao()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        jogadoresQueTocaram.Clear();
        sincronizacaoAtiva = false;

        Debug.Log("[SincronizacaoManager] Sincronização resetada - tempo esgotado");

        // Preparar para nova tentativa
        Debug.Log("[SincronizacaoManager] Pronto para nova tentativa...");

        // Reiniciar automaticamente após 2 segundos
        Invoke("ReiniciarSincronizacao", 2f);
    }

    private void ReiniciarSincronizacao()
    {
        if (!sincronizacaoConcluida && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[SincronizacaoManager] Reiniciando sincronização automaticamente...");
            IniciarSincronizacao();
        }
    }

    private void MostrarBotoesSincronizacao()
    {
        Debug.Log("[SincronizacaoManager] MostrarBotoesSincronizacao chamado");

        if (botoesSincronizacao == null || botoesSincronizacao.Length == 0)
        {
            Debug.LogError("[SincronizacaoManager] Array botoesSincronizacao está vazio ou null!");
            return;
        }

        foreach (var botao in botoesSincronizacao)
        {
            if (botao != null)
            {
                var botaoScript = botao.GetComponent<BotaoSincronizacaoFase2>();
                if (botaoScript != null)
                {
                    botaoScript.AtivarParaMissao();
                    Debug.Log($"[SincronizacaoManager] Botão {botao.name} ativado via script");
                }
                else
                {
                    Debug.LogError($"[SincronizacaoManager] Botão {botao.name} não tem script BotaoSincronizacaoFase2!");
                    botao.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("[SincronizacaoManager] Botão null no array!");
            }
        }
    }

    private void EsconderBotoesSincronizacao()
    {
        Debug.Log("[SincronizacaoManager] EsconderBotoesSincronizacao chamado");

        if (botoesSincronizacao != null)
        {
            foreach (var botao in botoesSincronizacao)
            {
                if (botao != null)
                {
                    botao.SetActive(false);
                    Debug.Log($"[SincronizacaoManager] Botão {botao.name} desativado");
                }
            }
        }
    }

    // NOVO: Método para verificar se a sincronização está ativa
    public bool IsSincronizacaoAtiva()
    {
        return sincronizacaoAtiva;
    }

    // NOVO: Método para verificar se a sincronização foi concluída
    public bool IsSincronizacaoConcluida()
    {
        return sincronizacaoConcluida;
    }
}