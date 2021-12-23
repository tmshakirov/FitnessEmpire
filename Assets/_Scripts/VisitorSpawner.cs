using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorSpawner : MonoBehaviour
{
    [SerializeField] private int maxVisitors = 5;
    [SerializeField] private float timer;
    [SerializeField] private VisitorScript visitor, femaleVisitor, vip;
    [SerializeField] private List<VisitorScript> visitors = new List<VisitorScript>();

    private void Update()
    {
        timer -= Time.deltaTime * 60;
        if (timer <= 0 && visitors.Count < maxVisitors)
        {
            VisitorScript v = null;
            if (Random.Range (1,101) <= 10)
                v = Instantiate(vip, transform.position, transform.rotation);
            else
                v = Instantiate((Random.Range(1, 101) <= 50) ? visitor : femaleVisitor, transform.position, transform.rotation);
            v.SetSpawner(this);
            visitors.Add(v);
            timer = Random.Range(150, 250);
        }
    }

    public void ResetSpawn()
    {
        timer = Random.Range(300, 450);
    }

    public void ChangeLimit()
    {
        maxVisitors = ToolsHandler.Instance.tools.Count;
    }

    public void Remove (VisitorScript _visitor)
    {
        visitors.Remove(_visitor);
    }
}
