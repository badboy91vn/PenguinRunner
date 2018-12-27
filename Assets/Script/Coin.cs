using UnityEngine;

public class Coin : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.SetTrigger("Spawn");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.UpdateCoin(true);
            anim.SetTrigger("Collected");
        }
    }
}
