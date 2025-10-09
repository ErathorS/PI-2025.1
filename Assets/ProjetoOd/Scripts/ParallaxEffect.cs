using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f;
    
    private Transform cam;
    private Vector3 lastCamPos;

    void Start() {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate() {
        Vector3 delta = cam.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxEffect, delta.y * parallaxEffect, 0);
        lastCamPos = cam.position;
    }
}