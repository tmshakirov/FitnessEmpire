using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ItemScript : SerializedMonoBehaviour
{
    public ToolType type;
    public float offset = 0.25f;
    [SerializeField] private Vector2 offPos;
    [SerializeField] private Vector3 angle;
    [SerializeField] private Dictionary<ToolType, float> offsets;
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

    public void SetTarget (Transform _place, ItemScript _prevItem)
    {
        if (MR == null)
            MR = GetComponent<MeshRenderer>();
        transform.SetParent(_place);
        transform.localRotation = Quaternion.Euler(angle);
        if (_prevItem != null)
        {
            if (offsets.TryGetValue(_prevItem.type, out offset))
                destPos = new Vector3(offPos.x, _prevItem.transform.localPosition.y + offset, offPos.y);
        }
        else
        {
            destPos = new Vector3(offPos.x, offset, offPos.y);
        }
        var tmp = MR.material.color;
        MR.material.color = Color.white;
        MR.material.DOColor(tmp, 0.4f);
    }
}
