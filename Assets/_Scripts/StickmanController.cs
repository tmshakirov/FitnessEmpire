using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using System;

public class StickmanController : MonoBehaviour
{
    [SerializeField] private int dollars;
    private AudioSource thisSource;
    private Animator anim;
    private Rigidbody RB;
    [SerializeField] private List<ItemScript> items;
    [SerializeField] private Transform itemPlace;
    [SerializeField] private Joystick joystick;
    [SerializeField] private float stickmanSpeed;

    void Start()
    {
        thisSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (joystick.Direction != Vector2.zero)
        {
            float inputZ = joystick.Direction.y;
            float inputX = joystick.Direction.x;

            Vector3 lookDirection = new Vector3(inputX, 0, inputZ);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            float step = 10 * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, step);
            transform.Translate(Vector3.forward * Time.deltaTime * stickmanSpeed);
            anim.Play(items.Count > 0 ? "Carrying": "Run");
        }
        else
        {
            anim.Play(items.Count > 0 ? "CarryingIdle" : "Idle");
        }
    }

    private void LateUpdate()
    {
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
    }

    public void AddItem (ItemScript _item)
    {
        items.Add(_item);
        _item.SetTarget(itemPlace, items.Count);
    }

    public void RemoveItem (ToolScript _tool, VisitorScript _visitor)
    {
        if (HasItem (_tool.type))
        {
            var item = items.Find(x => x.type == _tool.type);
            items.Remove(item);
            var middlePoint = new Vector3((_tool.currentVisitor.transform.position.x + item.transform.position.x) / 2,
                item.transform.position.y + 1f, (_tool.currentVisitor.transform.position.z + item.transform.position.z) / 2);
            item.transform.DOMove(middlePoint, 0.25f).OnComplete(() =>
            {
                item.GetComponent<MeshRenderer>().material.DOColor(Color.white, 0.25f);
                item.transform.DOScale(0, 0.5f);
                item.transform.DOMove(_tool.currentVisitor.transform.position, 0.5f).OnComplete(() =>
                {
                    _visitor.StartTraining();
                    Destroy(item.gameObject);
                });
            });
        }
    }

    public bool HasItem (ToolType _type)
    {
        return items.Find (x => x.type == _type) != null;
    }

    public int GetDollars()
    {
        return dollars;
    }

    public void AddDollars (int _amount)
    {
        dollars += _amount;
        if (dollars < 0)
            dollars = 0;
        UIHandler.Instance.SetCount(dollars);
    }
}