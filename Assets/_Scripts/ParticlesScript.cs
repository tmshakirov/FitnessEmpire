using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesScript : MonoBehaviour
{
    [SerializeField] private float delay;

    void Start()
    {
        Invoke("Destruction", delay);
    }

    void Destruction()
    {
        Destroy(gameObject);
    }
}
