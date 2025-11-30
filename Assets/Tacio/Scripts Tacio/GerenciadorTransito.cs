using UnityEngine;
using System.Collections.Generic;

public class GerenciadorTransito : MonoBehaviour
{
    [System.Serializable]
    public class SemaforoConfig
    {
        public string nome;
        public bool semaforoAtivo = false;
        public Collider triggerSemaforo; // Trigger que os carros detectam
        public List<Collider> paredesBarreira = new List<Collider>(); // Barreiras invisíveis
    }

    [Header("Configurações dos Semáforos")]
    public List<SemaforoConfig> semaforos = new List<SemaforoConfig>();

    [Header("Controle Automático")]
    public bool controleAutomatico = true;
    public float tempoTrocaSemaforo = 10f;

    private float tempoDecorrido = 0f;

    void Start()
    {
        // Inicializa todos os estados baseado nos semáforos
        AtualizarEstadosSemaforos();
    }

    void Update()
    {
        if (controleAutomatico)
        {
            // Controle automático do semáforo
            tempoDecorrido += Time.deltaTime;
            if (tempoDecorrido >= tempoTrocaSemaforo)
            {
                TrocarSemaforos();
                tempoDecorrido = 0f;
            }
        }

        // Atualiza os estados continuamente
        AtualizarEstadosSemaforos();
    }

    void AtualizarEstadosSemaforos()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            // Lógica: Semáforo ativo = barreiras DESATIVADAS (players podem passar)
            //          Semáforo inativo = barreiras ATIVADAS (players NÃO podem passar)
            
            foreach (Collider parede in semaforo.paredesBarreira)
            {
                if (parede != null)
                {
                    parede.enabled = !semaforo.semaforoAtivo;
                }
            }

            // Ativa/desativa o trigger do semáforo para os carros detectarem
            if (semaforo.triggerSemaforo != null)
            {
                semaforo.triggerSemaforo.enabled = semaforo.semaforoAtivo;
            }
        }
    }

    void TrocarSemaforos()
    {
        // Alterna o estado de todos os semáforos
        foreach (SemaforoConfig semaforo in semaforos)
        {
            semaforo.semaforoAtivo = !semaforo.semaforoAtivo;
        }

        Debug.Log("Estados dos semáforos trocados automaticamente");
    }

    // Método para verificar se um carro deve parar baseado no semáforo
    public bool DevePararNoSemaforo(Collider triggerDetectado)
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            if (semaforo.triggerSemaforo == triggerDetectado && semaforo.semaforoAtivo)
            {
                return true; // Carro deve parar
            }
        }
        return false; // Carro pode continuar
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

    // Método para controle alternativo externo
    public void SetarEstadoSemaforo(int index, bool estado)
    {
        if (index >= 0 && index < semaforos.Count)
        {
            semaforos[index].semaforoAtivo = estado;
            Debug.Log($"Semáforo {semaforos[index].nome} setado para: {estado}");
        }
    }

    // Gizmos para visualização no Editor
    void OnDrawGizmosSelected()
    {
        foreach (SemaforoConfig semaforo in semaforos)
        {
            // Gizmo para o trigger do semáforo
            if (semaforo.triggerSemaforo != null)
            {
                Gizmos.color = semaforo.semaforoAtivo ? Color.red : Color.green;
                
                if (semaforo.triggerSemaforo is BoxCollider boxCollider)
                {
                    Gizmos.matrix = semaforo.triggerSemaforo.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                }
            }

            // Gizmo para as paredes barreira
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