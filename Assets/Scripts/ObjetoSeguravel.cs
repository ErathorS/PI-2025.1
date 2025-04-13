using UnityEngine;

public class ObjetoSeguravel : MonoBehaviour
{
    Rigidbody objetoRigidbody;
    Transform objtGrabPointTransform;

    void Awake()
    {
        objetoRigidbody = GetComponent<Rigidbody>();
    }
    public void pegar(Transform objtGrabPointTransform)
    {
        this.objtGrabPointTransform = objtGrabPointTransform;
        objetoRigidbody.useGravity = false;
    }
    public void soltar()
    {
        this.objtGrabPointTransform = null;
        objetoRigidbody.useGravity = true;
    }
    void FixedUpdate()
    {
        if(objtGrabPointTransform != null)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, objtGrabPointTransform.position, Time.deltaTime * 25f);
            objetoRigidbody.MovePosition(newPosition);
        }
    }


}
