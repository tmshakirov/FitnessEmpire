using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorScript : MonoBehaviour
{
    private bool open;
    [SerializeField] private float openingDistance = 2.5f;
    private Animator anim;
    private List<VisitorScript> visitors = new List<VisitorScript>();

    private void Start()
    {
        anim = GetComponent<Animator>();
        open = false;
        InvokeRepeating("CheckOpen", 0, 0.5f);
    }

    void CheckOpen ()
    {
        visitors.Clear();
        visitors = FindObjectsOfType<VisitorScript>().ToList();
        open = false;
        if (visitors.Count > 0)
        {
            foreach (var v in visitors)
            {
                if (Vector3.Distance(transform.position, v.transform.position) <= openingDistance)
                    open = true;
            }
        }
        if (open)
            anim.Play("DoorOpen");
        else
            anim.Play("DoorClose");
    }
}
