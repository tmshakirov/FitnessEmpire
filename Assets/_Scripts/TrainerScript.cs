using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum TrainerType { WAITING, GOING, CARRYING }

public class TrainerScript : MonoBehaviour
{
    public TrainerType task;
    public Vector3 target;
    private Animator anim;
    public float moveSpeed;
    private NavMeshAgent agent;
    private float itemTimer;
    [SerializeField] private Transform itemPlace;

    [SerializeField] private VisitorScript visitor;
    [SerializeField] private ItemScript item;
    private ItemSpawner spawner;

    private void Start()
    {
        itemTimer = 60;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("FindVisitors", 0.1f, 0.5f);
    }

    private bool DestinationReached()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                //if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Update()
    {
        switch (task)
        {
            case TrainerType.GOING:
                //Debug.Log(agent.remainingDistance + " - " + transform.position + " - " + target + " - " + agent.destination);
                if (DestinationReached())
                {
                    anim.Play("Idle");
                    agent.SetDestination(transform.position);
                    transform.LookAt(target);
                    itemTimer -= Time.deltaTime * 60;
                    if (itemTimer <= 0)
                    {
                        SetCarrying();
                    }
                }
                else
                {
                    anim.Play("Run");
                }
                break;
            case TrainerType.WAITING:
                anim.Play("Idle");
                break;
            case TrainerType.CARRYING:
                if (visitor != null && visitor.task == TaskType.WAITING)
                {
                    if (DestinationReached() && !visitor.gotItem)
                    {
                        anim.Play("Idle");
                        agent.SetDestination(transform.position);
                        transform.LookAt(target);
                        GiveItem();
                    }
                    else
                    {
                        anim.Play("Carrying");
                    }
                }
                else
                {
                    Destroy(item.gameObject);
                    visitor = null;
                    spawner = null;
                    task = TrainerType.WAITING;
                }
                break;
        }
    }

    private void FindVisitors()
    {
        if (visitor == null)
        {
            var visitors = FindObjectsOfType<VisitorScript>().ToList();
            foreach (var v in visitors)
            {
                if (v.task == TaskType.WAITING)
                {
                    if (visitor == null ||
                        Vector3.Distance(transform.position, visitor.transform.position) > Vector3.Distance(transform.position, v.transform.position))
                        visitor = v;
                }
            }
            if (visitor != null)
            {
                var t = visitor.currentTool;
                var items = FindObjectsOfType<ItemSpawner>();
                spawner = items.First(x => x.ItemType() == t.type);
                if (spawner != null)
                {
                    target = new Vector3(spawner.transform.position.x, transform.position.y, spawner.transform.position.z);
                    agent.SetDestination(target);
                    task = TrainerType.GOING;
                }
                else
                {
                    task = TrainerType.WAITING;
                    agent.SetDestination(transform.position);
                }
            }
            else
            {
                task = TrainerType.WAITING;
                agent.SetDestination(transform.position);
            }
        }
    }

    private void SetCarrying()
    {
        if (visitor != null && visitor.task == TaskType.WAITING)
        {
            AddItem();
            target = new Vector3(visitor.transform.position.x, transform.position.y, visitor.transform.position.z);
            agent.SetDestination(target);
            task = TrainerType.CARRYING;
        }
        else
        {
            spawner = null;
            item = null;
            task = TrainerType.WAITING;
        }
        itemTimer = 60;
    }

    public void AddItem()
    {
        var _item = spawner.SpawnItem();
        item = _item;
        _item.SetTarget(itemPlace, 0);
    }

    private void GiveItem()
    {
        RemoveItem(visitor.currentTool, visitor);
        visitor.gotItem = true;
        spawner = null;
        visitor = null;
        task = TrainerType.WAITING;
    }

    public void RemoveItem(ToolScript _tool, VisitorScript _visitor)
    {
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
