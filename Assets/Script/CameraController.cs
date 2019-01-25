using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool IsMoving { get; set; }

    public Transform lookAt; // Player
    public Vector3 offset = new Vector3(0, 2.0f, -5.5f);
    public Vector3 rotation = new Vector3(35.0f, 0.0f, 0.0f);

    private const float speedMove = 0.5f;
    private Vector3 move;

    private void LateUpdate()
    {
        if (!IsMoving) return;

        move = lookAt.position + offset;
        transform.position = Vector3.Lerp(transform.position, move, speedMove);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.1f);
    }
}
