using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform lookAt; // Player
    public Vector3 offset = new Vector3(0, 2.0f, -5.5f);
    private const float speedMove = 0.5f;
    private Vector3 move;

    private void Start()
    {
        transform.position = lookAt.position + offset;
    }

    private void LateUpdate()
    {
        move = lookAt.position + offset;
        //move.x = 0f;
        transform.position = Vector3.Lerp(transform.position, move, speedMove);
    }
}
