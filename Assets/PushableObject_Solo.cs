using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class PushableObject_Solo : MonoBehaviourPun
{
    public float pushForce = 5f;
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isBeingPushed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    [PunRPC]
    public void StartPushing(Vector3 direction)
    {
        isBeingPushed = true;
        pushDirection = new Vector3(direction.x, 0, direction.z).normalized;
    }

    [PunRPC]
    public void StopPushing()
    {
        isBeingPushed = false;
        pushDirection = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (isBeingPushed)
        {
            rb.velocity = pushDirection * pushForce;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Garante que a caixa não fique presa no jogador
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), false);
        }
    }
}