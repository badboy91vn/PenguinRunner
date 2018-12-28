using UnityEngine;

public class GlacierSpawner : MonoBehaviour
{
    //public static GlacierSpawner Instance { get; set; }

    public bool IsScrolling { get; set; }

    // Const
    private const float DISTANCE_TO_DESPAWN = 10.0f;

    // Game
    public float scrollSpeed = -2.0f;
    public float totalLength;

    private float scrollLocaltion;
    private Transform playerTransform;

    void Awake()
    {
        //Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsScrolling) return;

        scrollLocaltion += scrollSpeed * Time.deltaTime;
        Vector3 newLocaltion = (playerTransform.position.z * scrollLocaltion) * Vector3.forward;
        transform.position = newLocaltion;

        if (transform.GetChild(0).transform.position.z < (playerTransform.position.z - DISTANCE_TO_DESPAWN))
        {
            transform.GetChild(0).localPosition += Vector3.forward * totalLength;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);
        }
    }
}
