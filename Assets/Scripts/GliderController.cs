using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderController : MonoBehaviour
{
    [SerializeField]
    private GameObject BodyPlayer;

    [SerializeField]
    float rotateSpeed = 5f;

    private void Update()
    {
        /*//transform.LookAt(transform.position + scaledMovement, Vector3.up);
        transform.Rotate(0, 0, rotateSpeed);
        BodyPlayer.transform.Rotate(0, 0, rotateSpeed);*/
    }
}
