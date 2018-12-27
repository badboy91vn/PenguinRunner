using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public int maxCoin = 5;
    public float changeToSpawn = 0.5f;
    public bool forceSpawnAll = false;

    private GameObject[] coins;

    private void Awake()
    {
        coins = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            coins[i] = transform.GetChild(i).gameObject;
        }

        OnDisable();
    }

    private void OnEnable()
    {
        if (Random.Range(0.0f, 1.0f) > changeToSpawn) return;

        if (forceSpawnAll)
        {
            for (int i = 0; i < maxCoin; i++)
            {
                coins[i].SetActive(true);
            }
        }
        else
        {
            int ran = Random.Range(0, maxCoin);
            for (int i = 0; i < ran; i++)
            {
                coins[i].SetActive(true);
            }
        }
    }
    private void OnDisable()
    {
        foreach (GameObject go in coins)
        {
            go.SetActive(false);
        }
    }
}
