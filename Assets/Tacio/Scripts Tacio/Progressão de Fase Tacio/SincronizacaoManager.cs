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
        if (!sincronizacaoAtiva || sincronizacaoConcluida) return;

        photonView.RPC("RPC_RegistrarToque", RpcTarget.All, actorID);
    }

    [PunRPC]
    private void RPC_RegistrarToque(int actorID)
    {
        jogadoresQueTocaram.Add(actorID);
        tempoUltimoToque = Time.time;

        Debug.Log($"[SincronizacaoManager] Jogador {actorID} tocou. Total: {jogadoresQueTocaram.Count}/2");

        if (jogadoresQueTocaram.Count >= 2)
        {
            ConcluirSincronizacao();
        }
    }

    private void Update()
    {
        if (sincronizacaoAtiva && !sincronizacaoConcluida)
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

        photonView.RPC("RPC_ConcluirSincronizacao", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ConcluirSincronizacao()
    {
        Debug.Log("[SincronizacaoManager] Sincronização concluída!");

        // Para as luzes de piscar
        foreach (var poste in postesPiscando)
        {
            if (poste != null)
            {
                poste.PararDePiscar();
            }
        }

        EsconderBotoesSincronizacao();

        // Notifica o MissaoFase2Manager
        MissaoFase2Manager.instancia.MissaoConcluida();
    }

    private void ResetarSincronizacao()
    {
        jogadoresQueTocaram.Clear();
        sincronizacaoAtiva = false;
        Debug.Log("[SincronizacaoManager] Sincronização resetada - tempo esgotado");
        
        // Opcional: feedback visual/sonoro de falha
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
                // 🔴 CORREÇÃO: Usar método controlado em vez de SetActive direto
                var botaoScript = botao.GetComponent<BotaoSincronizacaoFase2>();
                if (botaoScript != null)
                {
                    botaoScript.AtivarParaMissao();
                    Debug.Log($"[SincronizacaoManager] Botão {botao.name} ativado via script");
                }
                else
                {
                    Debug.LogError($"[SincronizacaoManager] Botão {botao.name} não tem script BotaoSincronizacaoFase2!");
                    botao.SetActive(true); // Fallback
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