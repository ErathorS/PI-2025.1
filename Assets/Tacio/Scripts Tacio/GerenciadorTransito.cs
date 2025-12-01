using UnityEngine;
using System.Collections.Generic;

public class GerenciadorTransito : MonoBehaviour
{
    [System.Serializable]
    public class SemaforoConfig
    {
        public string nome;
        public bool semaforoAtivo = false;
        public Collider triggerSemaforo;
        public List<Collider> paredesBarreira = new List<Collider>();
    }

    [Header("Configurações dos Semáforos")]
    public List<SemaforoConfig> semaforos = new List<SemaforoConfig>();
    
    [Header("Controle Automático")]
    public bool controleAutomatico = true;
    public float tempoTrocaSemaforo = 10f;

    private float tempoDecorrido = 0f;

    void Start()
    {
        AtualizarEstadosSemaforos();
    }

    void Update()
    {
        if (controleAutomatico)
        {
            tempoDecorrido += Time.deltaTime;
            if (tempoDecorrido >= tempoTrocaSemaforo)
            {
                TrocarSemaforos();
                tempoDecorrido = 0f;
            }
        }

        AtualizarEstadosSemaforos();
    }

    void AtualizarEstadosSemaforos()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            foreach (Collider parede in semaforo.paredesBarreira)
            {
                if (parede != null)
                {
                    parede.enabled = !semaforo.semaforoAtivo;
                }
            }

            if (semaforo.triggerSemaforo != null)
            {
                semaforo.triggerSemaforo.enabled = semaforo.semaforoAtivo;
            }
        }
    }

    void TrocarSemaforos()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            semaforo.semaforoAtivo = !semaforo.semaforoAtivo;
        }
        Debug.Log("[GerenciadorTransito] Estados dos semáforos trocados automaticamente");
    }

    // ✅ MÉTODOS QUE O MISSÃOFASE3MANAGER PRECISA:

    public void LiberarTransito()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            semaforo.semaforoAtivo = true;
        }
        AtualizarEstadosSemaforos();
        Debug.Log("[GerenciadorTransito] Trânsito liberado!");
    }

    public void BloquearTransito()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            semaforo.semaforoAtivo = false;
        }
        AtualizarEstadosSemaforos();
        Debug.Log("[GerenciadorTransito] Trânsito bloqueado!");
    }

    public void LiberarPassagemJogadores()
    {
        // Mesma lógica do LiberarTransito para compatibilidade
        LiberarTransito();
    }

    public void BloquearPassagemJogadores()
    {
        // Mesma lógica do BloquearTransito para compatibilidade
        BloquearTransito();
    }

    public bool IsPassagemLiberada()
    {
        if (semaforos.Count > 0)
        {
            return semaforos[0].semaforoAtivo;
        }
        return false;
    }

    public bool DevePararNoSemaforo(Collider triggerDetectado)
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            if (semaforo.triggerSemaforo == triggerDetectado)
            {
                return semaforo.semaforoAtivo;
            }
        }
        return false;
    }

    public void DebugEstadoAtual()
    {
        Debug.Log($"[GerenciadorTransito] === DEBUG ===");
        foreach (SemaforoConfig semaforo in semaforos)
        {
            Debug.Log($"Semáforo {semaforo.nome}: {(semaforo.semaforoAtivo ? "ATIVO" : "INATIVO")}");
        }
        Debug.Log($"=====================");
    }
}