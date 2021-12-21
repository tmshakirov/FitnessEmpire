using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemScript : MonoBehaviour
{
    public ToolType type;
    public float offset = 0.25f;
    [SerializeField] private Vector2 offPos;
    [SerializeField] private Vector3 angle;
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

    public void SetDestination (float _offset)
    {
        destPos = new Vector3(destPos.x, destPos.y - _offset, destPos.z);
    }

    public void SetTarget (Transform _place, int _offsetCount)
    {
        if (MR == null)
            MR = GetComponent<MeshRenderer>();
        transform.SetParent(_place);
        transform.localRotation = Quaternion.Euler(angle);
        destPos = new Vector3(offPos.x, _offsetCount * offset, offPos.y);
        var tmp = MR.material.color;
        MR.material.color = Color.white;
        MR.material.DOColor(tmp, 0.4f);
    }
}
