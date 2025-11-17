using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
public class CarTrafficController : MonoBehaviourPun
{
    [Header("Pontos de percurso")]
    public Transform startPoint;        // Ponto A
    public Transform endPoint;          // Ponto B
    public float reachDistance = 1f;    // Distância para considerar que chegou em B

    [Header("Velocidade")]
    public float maxSpeed = 8f;
    public float acceleration = 10f;

    [Header("Detecção à frente")]
    public float rayDistance = 5f;
    public float rayRadius = 1f;
    public LayerMask detectionMask;     // Layer para Player + Carros

    [Header("Semáforo (opcional)")]
    public SemaforoController semaforo;
    public Transform semaforoStopPoint;
    public float semaforoStopDistance = 4f;

    [Header("Animação de motor")]
    public Transform visualRoot;        // Parte visual do carro (mesh)
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 10f;

    private NavMeshAgent agent;
    private Vector3 visualBaseLocalPos;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (visualRoot != null)
            visualBaseLocalPos = visualRoot.localPosition;

        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        agent.autoBraking = false;

        if (PhotonNetwork.IsMasterClient)
        {
            if (startPoint != null)
            {
                transform.position = startPoint.position;
                agent.Warp(startPoint.position);
            }

            if (endPoint != null)
                agent.SetDestination(endPoint.position);
        }
        else
        {
            // Só o Master simula o NavMeshAgent
            agent.enabled = false;
        }
    }

    void Update()
    {
        AnimateMotor();

        // Movimento e lógica só no Master
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (endPoint == null || startPoint == null)
            return;

        bool blocked = CheckBlocked();

        if (blocked)
        {
            agent.isStopped = true;
        }
        else
        {
            if (agent.isStopped)
            {
                agent.isStopped = false;
                agent.SetDestination(endPoint.position);
            }
        }

        // Chegou em B → teleporta de volta para A
        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            agent.Warp(startPoint.position);
            transform.position = startPoint.position;
            agent.ResetPath();
            agent.SetDestination(endPoint.position);
        }
    }

    bool CheckBlocked()
    {
        // 1) Semáforo vermelho
        if (semaforo != null && semaforo.IsRed)
        {
            if (semaforoStopPoint != null)
            {
                float dist = Vector3.Distance(transform.position, semaforoStopPoint.position);
                if (dist <= semaforoStopDistance)
                    return true;
            }
        }

        // 2) Raycast à frente (Player / Carro)
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = transform.forward;

        if (Physics.SphereCast(origin, rayRadius, dir, out RaycastHit hit,
                               rayDistance, detectionMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return false;
    }

    void AnimateMotor()
    {
        if (visualRoot == null) return;

        float offset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        visualRoot.localPosition = visualBaseLocalPos + Vector3.up * offset;
    }

    // Se atropelar jogador -> respawn no spawn inicial
    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

        int actor = pv.Owner.ActorNumber;
        photonView.RPC("RPC_RespawnPlayer", RpcTarget.All, actor);
    }

    [PunRPC]
    void RPC_RespawnPlayer(int actorNumber)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            PhotonView pv = p.GetComponent<PhotonView>();
            if (pv != null && pv.Owner.ActorNumber == actorNumber)
            {
                string spawnTag = actorNumber == 1 ? "Spawn 1" : "Spawn 2";
                GameObject spawn = GameObject.FindGameObjectWithTag(spawnTag);

                if (spawn != null)
                    p.transform.position = spawn.transform.position;

                break;
            }
        }
    }
}
