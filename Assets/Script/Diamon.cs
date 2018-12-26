using UnityEngine;

public class Diamon : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.UpdateDiamon(true);
            anim.SetTrigger("Collected");
        }
    }
}
