using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    private bool picked;
    [SerializeField] private float moveSpeed;
    private Transform player;

    private Vector3 rotationVector;
    private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (Random.Range(1, 101) <= 50)
            rotationVector = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), 0);
        else
            rotationVector = new Vector3(0, Random.Range(0.5f, 1f), 0);
        rotationSpeed = Random.Range(50, 75);
    }

    // Update is called once per frame
    void Update()
    {
        if (picked)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            moveSpeed += Time.deltaTime;
            if (Vector3.Distance (transform.position, player.position) <= 0)
            {
                player.GetComponent<StickmanController>().AddDollars(10);
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Rotate(rotationVector * Time.deltaTime * rotationSpeed);
            if (Vector3.Distance(transform.position, player.position) <= 2.5f)
            {
                picked = true;
            }
        }
    }
}
