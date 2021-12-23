using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StickmanController : Singleton<StickmanController>
{
    [SerializeField] private int dollars;
    private AudioSource thisSource;
    private Animator anim;
    private Rigidbody RB;
    [SerializeField] private List<ItemScript> items;
    [SerializeField] private Transform itemPlace;
    [SerializeField] private GameObject money, gold;
    [SerializeField] private RectTransform moneyRect, canvasRect;
    [SerializeField] private Joystick joystick;
    [SerializeField] private float stickmanSpeed;

    private float defaultPosY;

    void Start()
    {
        defaultPosY = transform.position.y;
        thisSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
    }

    public bool IsMoving()
    {
        return joystick.Direction != Vector2.zero;
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
        transform.position = new Vector3(transform.position.x, defaultPosY, transform.position.z);
    }

    public void AddItem (ItemScript _item)
    {
        ItemScript prevItem = null;
        if (items.Count > 0)
            prevItem = items[items.Count - 1];
        items.Add(_item);
        _item.SetTarget(itemPlace, prevItem);
    }

    public void RemoveItem (ToolScript _tool, VisitorScript _visitor)
    {
        if (HasItem (_tool.type))
        {
            var item = items.Find(x => x.type == _tool.type);
            items.Remove(item);
            var middlePoint = new Vector3((_tool.currentVisitor.transform.position.x + item.transform.position.x) / 2,
                item.transform.position.y + 1f, (_tool.currentVisitor.transform.position.z + item.transform.position.z) / 2);
            foreach (var i in items)
            {
                if (i.transform.position.y > item.transform.position.y)
                {
                    i.SetDestination(item.offset);
                }
            }
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

    public bool EnoughMoney (int _amount)
    {
        return dollars >= _amount;
    }

    public void AddDollars (int _amount)
    {
        if (_amount > 0)
        {
            bool gold = _amount >= 50;
            if (gold)
                StartCoroutine(AddingMoney(_amount / 50, gold));
            else
                StartCoroutine(AddingMoney(_amount, gold));
        }
        else
        {
            dollars += _amount;
            if (dollars < 0)
                dollars = 0;
        }
        UIHandler.Instance.SetCount(dollars);
    }

    private IEnumerator AddingMoney (int _amount, bool _gold)
    {
        if (_amount > 0)
        {
            var m = Instantiate(_gold ? gold : money, canvasRect.transform);
            m.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            m.GetComponent<Image>().DOFade(0.5f, 0.3f);
            m.transform.DOMoveY(m.transform.position.y + 100, 0.3f).OnComplete(() =>
            {
                m.GetComponent<Image>().DOFade(0.1f, 0.4f);
                m.transform.DOScale(0.1f, 0.4f);
                m.transform.DOMove(moneyRect.transform.position, 0.4f).OnComplete(() =>
                {
                    Destroy(m.gameObject);
                });
            });
            yield return new WaitForSeconds(0.05f);
            if (_gold)
                dollars += 50;
            else
                dollars += 10;
            UIHandler.Instance.SetCount(dollars);
            StartCoroutine(AddingMoney(_amount-1, _gold));
        }
    }
}