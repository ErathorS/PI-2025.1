using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CarMovemetn : MonoBehaviour
{
    public float DeSlow = 0;
    public Vector3 SpawnPlayer = new Vector3(-2.5f, -0.001f, -7.4f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(transform.right * Time.deltaTime * DeSlow);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right, out hit, 15))
        {
            if (Vector3.Distance(transform.position, hit.transform.position) <= 5f && hit.transform.CompareTag("Stop Car"))
            {
                DeSlow = 0;
            }
            else if (hit.transform.CompareTag("Stop Car"))
            {
                DeSlow = Vector3.Distance(transform.position, hit.transform.position) * 0.5f;
            }
            if (hit.transform.CompareTag("Respawn"))
            {
                transform.position = new Vector3(transform.position.x - Random.Range(120, 150), transform.position.y, transform.position.z);
            }
            if (hit.transform.CompareTag("Player") && DeSlow > 5 && (Vector3.Distance(transform.position, hit.transform.position) < 5))
            {
                hit.collider.gameObject.transform.position = SpawnPlayer;
            }
            if (DeSlow < 15 && !hit.transform.CompareTag("Player") && !hit.transform.CompareTag("Respawn") && !hit.transform.CompareTag("Stop Car"))
            {
                DeSlow += 0.1f;
            }

        }
        else 
        {
            if (DeSlow < 15)
            {
                DeSlow += 0.1f;
            }
        }
    }
}
