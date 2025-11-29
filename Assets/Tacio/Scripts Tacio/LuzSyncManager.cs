using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LuzSyncManager : MonoBehaviourPun
{
    public static LuzSyncManager instancia;
    
    [Header("Configurações")]
    public int toquesNecessarios = 2; // Um de cada jogador
    public float tempoMaximoSincronizacao = 3f; // Tempo máximo entre os toques
    
    [Header("Referências das Luzes")]
    public List<PostePiscando> postesPiscando;
    public Light[] outrasLuzes; // Outras luzes que também devem parar de piscar
    
    // Estado
    private HashSet<int> jogadoresQueTocaram = new HashSet<int>();
    private float tempoUltimoToque;
    private bool sincronizacaoConcluida = false;
    private bool aguardandoSincronizacao = false;

    private void Awake()
    {
        instancia = this;
    }

    // Chamado quando um jogador toca no botão de interação
    public void RegistrarToque(int actorID)
    {
        if (sincronizacaoConcluida) return;
        
        if (!aguardandoSincronizacao)
        {
            // Inicia o processo de sincronização
            aguardandoSincronizacao = true;
            tempoUltimoToque = Time.time;
            photonView.RPC("RPC_IniciarSincronizacao", RpcTarget.All);
        }
        
        jogadoresQueTocaram.Add(actorID);
        tempoUltimoToque = Time.time;
        
        // Verifica se conseguiu a sincronização
        VerificarSincronizacao();
    }

    private void VerificarSincronizacao()
    {
        if (jogadoresQueTocaram.Count >= toquesNecessarios)
        {
            SincronizacaoConcluida();
        }
    }

    private void Update()
    {
        if (aguardandoSincronizacao && !sincronizacaoConcluida)
        {
            // Verifica timeout
            if (Time.time - tempoUltimoToque > tempoMaximoSincronizacao)
            {
                ResetarSincronizacao();
            }
        }
    }

    private void SincronizacaoConcluida()
    {
        sincronizacaoConcluida = true;
        aguardandoSincronizacao = false;
        
        photonView.RPC("RPC_SincronizacaoConcluida", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_IniciarSincronizacao()
    {
        Debug.Log("[LuzSyncManager] Sincronização iniciada!");
        // Poderia adicionar feedback visual/sonoro aqui
    }

    [PunRPC]
    private void RPC_SincronizacaoConcluida()
    {
        Debug.Log("[LuzSyncManager] Sincronização concluída! Normalizando luzes...");
        
        // Para todas as luzes de piscar
        foreach (var poste in postesPiscando)
        {
            if (poste != null)
            {
                poste.PararDePiscar();
            }
        }
        
        // Ativa outras luzes se necessário
        foreach (var luz in outrasLuzes)
        {
            if (luz != null)
            {
                luz.enabled = true;
            }
        }
        
        // Notifica o progresso da fase
        var progresso = FindObjectOfType<ProgressaoFaseController>();
        progresso?.NPCImportanteConcluido();
    }

    private void ResetarSincronizacao()
    {
        jogadoresQueTocaram.Clear();
        aguardandoSincronizacao = false;
        photonView.RPC("RPC_ResetarSincronizacao", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ResetarSincronizacao()
    {
        Debug.Log("[LuzSyncManager] Sincronização resetada - tempo esgotado");
        // Feedback visual/sonoro de falha
    }

}