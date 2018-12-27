using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTranform;

    // Start is called before the first frame update
    void Start()
    {
        playerTranform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.forward * playerTranform.position.z;
    }
}
