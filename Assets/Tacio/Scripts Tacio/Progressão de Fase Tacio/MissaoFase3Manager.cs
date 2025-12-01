using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class MissaoFase3Manager : MonoBehaviourPunCallbacks
{
    public static MissaoFase3Manager instancia;

    [Header("UI Missão 1 - Zona 1")]
    public GameObject painelMissao1;
    public TMP_Text textoMissao1;
    public TMP_Text textoTempoMissao1;

    [Header("UI Missão 2 - Zona 2")]
    public GameObject painelMissao2;
    public TMP_Text textoMissao2;

    [Header("Referências NPCs")]
    public DialogoNPC npcZona1;
    public DialogoNPC npcZona2;

    [Header("Sistemas de Trânsito")]
    public GerenciadorTransito transitoZonaA;
    public GerenciadorTransito transitoZonaB;

    [Header("Sistema de Coleta (Missão 1)")]
    public ColetarCaixasManager coletorMissao1;

    [Header("Sistema de Sincronização (Missão 2)")]
    public SincronizacaoManager sincronizadorMissao2;

    [Header("Painel Final")]
    public PainelFinalFaseController painelFinalFase;

    [Header("Configurações")]
    public float tempoLimiteMissao1 = 180f;

    // Estados
    private bool missao1Ativa = false;
    private bool missao1Concluida = false;
    private bool missao2Ativa = false;
    private bool missao2Concluida = false;
    private float tempoRestanteMissao1;

    private PhotonView photonView;

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

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
        }
    }

    private void Start()
    {
        if (painelMissao1 != null) painelMissao1.SetActive(false);
        if (painelMissao2 != null) painelMissao2.SetActive(false);

        ConfigurarNPCs();
        
        if (transitoZonaA != null) 
        {
            transitoZonaA.BloquearTransito(); 
            transitoZonaA.controleAutomatico = false;
        }
        if (transitoZonaB != null) 
        {
            transitoZonaB.BloquearTransito(); 
            transitoZonaB.controleAutomatico = false;
        }

        Debug.Log("[MissaoFase3Manager] Fase 3 iniciada - Zonas bloqueadas");
    }

    private void ConfigurarNPCs()
    {
        if (npcZona1 != null)
        {
            npcZona1.OnDialogoInicialConcluido += IniciarMissao1;
            npcZona1.OnDialogoEntregaConcluido += EntregarMissao1;
            npcZona1.ConfigurarComoNPCZona1();
        }

        if (npcZona2 != null)
        {
            npcZona2.OnDialogoInicialConcluido += IniciarMissao2;
            npcZona2.OnDialogoEntregaConcluido += EntregarMissao2;
            npcZona2.ConfigurarComoNPCZona2();
        }
    }

    private void Update()
    {
        if (missao1Ativa && !missao1Concluida)
        {
            tempoRestanteMissao1 -= Time.deltaTime;
            
            if (tempoRestanteMissao1 <= 0)
            {
                tempoRestanteMissao1 = 0;
                Missao1Falhou();
            }

            AtualizarUIMissao1();
        }
    }

    private void AtualizarUIMissao1()
    {
        if (textoTempoMissao1 != null)
        {
            int minutos = Mathf.FloorToInt(tempoRestanteMissao1 / 60);
            int segundos = Mathf.FloorToInt(tempoRestanteMissao1 % 60);
            textoTempoMissao1.text = $"{minutos:00}:{segundos:00}";
        }

        if (textoMissao1 != null && coletorMissao1 != null)
        {
            textoMissao1.text = $"Caixas coletadas: {coletorMissao1.caixasColetadas}/{coletorMissao1.totalCaixas}\nTempo: {textoTempoMissao1.text}";
        }
    }


    public void IniciarMissao1()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("[MissaoFase3Manager] Iniciando Missão 1 via RPC");
        photonView.RPC("RPC_IniciarMissao1", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IniciarMissao1()
    {
        missao1Ativa = true;
        missao1Concluida = false;
        tempoRestanteMissao1 = tempoLimiteMissao1;

        if (painelMissao1 != null)
        {
            painelMissao1.SetActive(true);
            textoMissao1.text = "Coletem as caixas espalhadas pela zona!\nDepois voltem para falar comigo.";
        }

        if (coletorMissao1 != null)
        {
            coletorMissao1.AtivarCaixasParaMissao();
        }

        Debug.Log("[MissaoFase3Manager] Missão 1 (Zona 1) iniciada!");
    }

    public void EntregarMissao1()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("[MissaoFase3Manager] Entregando Missão 1 via RPC");
        photonView.RPC("RPC_ConcluirMissao1", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ConcluirMissao1()
    {
        missao1Ativa = false;
        missao1Concluida = true;

        if (painelMissao1 != null)
        {
            painelMissao1.SetActive(false);
        }

        if (transitoZonaA != null)
        {
            transitoZonaA.LiberarTransito(); 
            transitoZonaA.controleAutomatico = true;
            Debug.Log("[MissaoFase3Manager] ✅ Zona A liberada! Trânsito automático ativado.");
        }

        Debug.Log("[MissaoFase3Manager] Missão 1 concluída! Zona 2 liberada.");
    }

    private void Missao1Falhou()
    {
        missao1Ativa = false;

        if (painelMissao1 != null)
        {
            textoMissao1.text = "Tempo esgotado! A missão falhou.\nVoltem ao NPC para tentar novamente.";
        }

        if (coletorMissao1 != null)
        {
            coletorMissao1.ResetarFase();
        }

        Debug.Log("[MissaoFase3Manager] Missão 1 falhou - tempo esgotado");
    }


    public void IniciarMissao2()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("[MissaoFase3Manager] Iniciando Missão 2 via RPC");
        photonView.RPC("RPC_IniciarMissao2", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IniciarMissao2()
    {
        missao2Ativa = true;
        missao2Concluida = false;

        if (painelMissao2 != null)
        {
            painelMissao2.SetActive(true);
            textoMissao2.text = "Sincronizem os botões para restaurar a energia!\nDepois voltem para falar comigo.";
        }

        if (sincronizadorMissao2 != null)
        {
            sincronizadorMissao2.IniciarSincronizacao();
        }

        Debug.Log("[MissaoFase3Manager] Missão 2 (Zona 2) iniciada!");
    }

    public void EntregarMissao2()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("[MissaoFase3Manager] Entregando Missão 2 via RPC");
        photonView.RPC("RPC_ConcluirMissao2", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ConcluirMissao2()
    {
        missao2Ativa = false;
        missao2Concluida = true;

        if (painelMissao2 != null)
        {
            painelMissao2.SetActive(false);
        }

        if (transitoZonaB != null)
        {
            transitoZonaB.LiberarTransito(); 
            transitoZonaB.controleAutomatico = true;
            Debug.Log("[MissaoFase3Manager] Zona B liberada! Trânsito automático ativado.");
        }

        if (painelFinalFase != null)
        {
            painelFinalFase.MostrarPainelFinal();
            Debug.Log("[MissaoFase3Manager] Painel final da fase exibido!");
        }

        Debug.Log("[MissaoFase3Manager] Missão 2 concluída! Zona 3 liberada.");
    }


    public bool IsMissao1Ativa()
    {
        return missao1Ativa;
    }

    public bool IsMissao1Concluida()
    {
        return missao1Concluida;
    }

    public bool IsMissao2Ativa()
    {
        return missao2Ativa;
    }

    public bool IsMissao2Concluida()
    {
        return missao2Concluida;
    }

    private void OnDestroy()
    {
        if (npcZona1 != null)
        {
            npcZona1.OnDialogoInicialConcluido -= IniciarMissao1;
            npcZona1.OnDialogoEntregaConcluido -= EntregarMissao1;
        }
        
        if (npcZona2 != null)
        {
            npcZona2.OnDialogoInicialConcluido -= IniciarMissao2;
            npcZona2.OnDialogoEntregaConcluido -= EntregarMissao2;
        }
    }

    // Método para debug
    public void DebugEstado()
    {
        Debug.Log($"[MissaoFase3Manager] === DEBUG FASE 3 ===");
        Debug.Log($"M1_Ativa: {missao1Ativa}, M1_Concluida: {missao1Concluida}");
        Debug.Log($"M2_Ativa: {missao2Ativa}, M2_Concluida: {missao2Concluida}");
        Debug.Log($"PainelFinal: {painelFinalFase != null}");
        Debug.Log($"===================================");
    }
}