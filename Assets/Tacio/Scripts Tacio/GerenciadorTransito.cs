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
        Debug.Log("Estados dos semáforos trocados automaticamente");
    }

    // NOVO: Método para liberar o trânsito
    public void LiberarTransito()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            semaforo.semaforoAtivo = true;
        }
        
        AtualizarEstadosSemaforos();
        Debug.Log("[GerenciadorTransito] Trânsito liberado!");
    }

    // NOVO: Método para bloquear o trânsito
    public void BloquearTransito()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            semaforo.semaforoAtivo = false;
        }
        
        AtualizarEstadosSemaforos();
        Debug.Log("[GerenciadorTransito] Trânsito bloqueado!");
    }

    public bool DevePararNoSemaforo(Collider triggerDetectado)
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            if (semaforo.triggerSemaforo == triggerDetectado && semaforo.semaforoAtivo)
            {
                return true;
            }
        }
        return false;
    }

    // Métodos para controle manual dos semáforos
    public void AtivarSemaforo(int index)
    {
        if (index >= 0 && index < semaforos.Count)
        {
            semaforos[index].semaforoAtivo = true;
            Debug.Log($"Semáforo {semaforos[index].nome} ativado");
        }
    }

    public void DesativarSemaforo(int index)
    {
        if (index >= 0 && index < semaforos.Count)
        {
            semaforos[index].semaforoAtivo = false;
            Debug.Log($"Semáforo {semaforos[index].nome} desativado");
        }
    }

    public void AlternarSemaforo(int index)
    {
        if (index >= 0 && index < semaforos.Count)
        {
            semaforos[index].semaforoAtivo = !semaforos[index].semaforoAtivo;
            Debug.Log($"Semáforo {semaforos[index].nome} alternado para: {semaforos[index].semaforoAtivo}");
        }
    }

    public void SetarEstadoSemaforo(int index, bool estado)
    {
        if (index >= 0 && index < semaforos.Count)
        {
            semaforos[index].semaforoAtivo = estado;
            Debug.Log($"Semáforo {semaforos[index].nome} estado para: {estado}");
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            if (semaforo.triggerSemaforo != null)
            {
                Gizmos.color = semaforo.semaforoAtivo ? Color.red : Color.green;

                if (semaforo.triggerSemaforo is BoxCollider boxCollider)
                {
                    Gizmos.matrix = semaforo.triggerSemaforo.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                }
            }

            Gizmos.color = semaforo.semaforoAtivo ? Color.blue : Color.yellow;
            foreach (Collider parede in semaforo.paredesBarreira)
            {
                if (parede != null)
                {
                    if (parede is BoxCollider boxParede)
                    {
                        Gizmos.matrix = parede.transform.localToWorldMatrix;
                        Gizmos.DrawWireCube(boxParede.center, boxParede.size);
                    }
                }
            }
        }
    }
}