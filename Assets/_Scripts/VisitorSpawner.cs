using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorSpawner : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private VisitorScript visitor;

    private void Update()
    {
        timer -= Time.deltaTime * 60;
        if (timer <= 0)
        {
            Instantiate(visitor, transform.position, transform.rotation);
            timer = Random.Range(300, 500);
        }
    }
}
