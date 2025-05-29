using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CarMovemetn : MonoBehaviour
{
    public float DeSlow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.right, out hit, 15) && hit.transform.CompareTag("Stop Car"))
        {
            if(Vector3.Distance(transform.position, hit.transform.position) <= 5f) 
            {
                DeSlow = 0;
            }
            else 
            {
                DeSlow = Vector3.Distance(transform.position, hit.transform.position) * 0.5f;
            }
            
        }
        else 
        {
            DeSlow = 10;
        }
        transform.Translate(transform.right * Time.deltaTime * DeSlow);

    }
}
