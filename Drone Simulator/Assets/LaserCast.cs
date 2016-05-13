using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class LaserCast : MonoBehaviour {
    public Camera cam;
    private Transform lockPoint;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void LaserLock()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            lockPoint = hit.transform;

            // Do something with object that was hit by raycast
        }
    }
}