using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemScript : MonoBehaviour
{
    public ToolType type;
    private Vector3 destPos;
    private bool set;
    [SerializeField] private MeshRenderer MR;

    private void Start()
    {
        MR = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!set)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destPos, 12 * Time.deltaTime);
            if (Vector3.Distance(transform.position, destPos) <= 0f)
            {
                set = true;
            }
        }
    }

    public void SetTarget (Transform _place, int _offsetCount)
    {
        if (MR == null)
            MR = GetComponent<MeshRenderer>();
        transform.SetParent(_place);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        destPos = new Vector3(0, _offsetCount * 0.25f, 0);
        var tmp = MR.material.color;
        MR.material.color = Color.white;
        MR.material.DOColor(tmp, 0.4f);
    }
}
