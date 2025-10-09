using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Holder : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Input.GetAxisRaw("Horizontal") * Time.deltaTime * 10, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime * 10);
    }
}
