using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class PushableObjectSingle : MonoBehaviourPun
{
    [Header("Configurações")]
    public float pushForce = 5f;
    public float stoppingDrag = 5f;
    
    private Rigidbody rb;
    private bool isBeingPushed = false;
    private Vector3 pushDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    [PunRPC]
    public void StartPushing(Vector3 direction)
    {
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
        }

        isBeingPushed = true;
        pushDirection = new Vector3(direction.x, 0, direction.z).normalized;
        rb.drag = 0.5f; // Baixo arrasto enquanto empurra
    }

    [PunRPC]
    public void StopPushing()
    {
        isBeingPushed = false;
        rb.drag = stoppingDrag; // Alto arrasto para parar rápido
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (isBeingPushed)
        {
            // Aplica força apenas na direção do empurrão
            rb.AddForce(pushDirection * pushForce, ForceMode.Force);
        }
        else if (rb.velocity.magnitude > 0.1f)
        {
            // Desacelera naturalmente quando não está sendo empurrado
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 2f);
        }
    }

    public bool IsBeingPushed()
    {
        return isBeingPushed;
    }
}