using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.AI;

public enum TaskType { GOING, WAITING, TRAINING, LEAVING }

public class VisitorScript : SerializedMonoBehaviour
{
    private bool gotItem;
    public TaskType task;
    public Vector3 target;
    private Animator anim;
    public float moveSpeed;
    public ToolScript currentTool;
    private float toolFindingTimer;
    private NavMeshAgent agent;
    private VisitorSpawner spawner;

    [SerializeField] private GameObject money;

    [SerializeField] private Dictionary<ToolType, List<GameObject>> tools;
    private List<GameObject> tool;

    private Vector3 screenPos;

    private Transform player;

    private void Start()
    {
        toolFindingTimer = 30;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        Invoke("Initialization", 0.1f);
    }

    public void SetSpawner (VisitorSpawner _spawner)
    {
        spawner = _spawner;
    }

    private void Initialization()
    {
        FindTool();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
            case TaskType.GOING:
                if (currentTool != null)
                {
                    anim.Play("Walk");
                    //transform.LookAt(target);
                    //transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                    //if (Vector3.Distance(transform.position, target) <= currentTool.waitingDistance)
                    if (DestinationReached())
                    {
                        agent.SetDestination(transform.position);
                        switch (currentTool.type)
                        {
                            case ToolType.BENCH:
                                transform.eulerAngles = new Vector3(currentTool.transform.eulerAngles.x, currentTool.transform.eulerAngles.y - 90, currentTool.transform.eulerAngles.z);
                                break;
                            case ToolType.BIKE:
                                transform.eulerAngles = new Vector3(currentTool.transform.eulerAngles.x, currentTool.transform.eulerAngles.y, currentTool.transform.eulerAngles.z);
                                break;
                            case ToolType.TREADMILL:
                                transform.eulerAngles = new Vector3(currentTool.transform.eulerAngles.x, currentTool.transform.eulerAngles.y + 90, currentTool.transform.eulerAngles.z);
                                break;
                            default:
                                transform.LookAt(target);
                                break;
                        }
                        task = TaskType.WAITING;
                    }
                }
                else
                {
                    anim.Play("Idle");
                    toolFindingTimer -= Time.deltaTime * 60;
                    if (toolFindingTimer <= 0)
                    {
                        FindTool();
                        toolFindingTimer = 30;
                    }
                }
            break;
            case TaskType.WAITING:
                WaitingAnimation();
                if (Vector3.Distance(transform.position, player.transform.position) <= currentTool.trainingDistance)
                {
                    if (player.GetComponent<StickmanController>().HasItem (currentTool.type) && !gotItem)
                    {
                        player.GetComponent<StickmanController>().RemoveItem(currentTool, this);
                        gotItem = true;
                    }
                }
                break;
            case TaskType.TRAINING:
                TrainingAnimation();
                break;
            case TaskType.LEAVING:
                anim.Play("Walk");
                //transform.LookAt(target);
                //transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                //if (Vector3.Distance(transform.position, target) <= 0)
                if (DestinationReached())
                {
                    spawner.Remove(this);
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void StartTraining ()
    {
        task = TaskType.TRAINING;
        currentTool.SetDisappearing(true);
        if (tools.TryGetValue(currentTool.type, out tool))
        {
            foreach (var t in tool)
                t.SetActive(true);
        }
    }

    private void TrainingFinished()
    {
        currentTool.SetFree();
        var exit = GameObject.FindGameObjectWithTag("exit");
        target = new Vector3(exit.transform.position.x, transform.position.y, exit.transform.position.z);
        task = TaskType.LEAVING;
        agent.stoppingDistance = 0;
        agent.SetDestination(target);
        if (tools.TryGetValue(currentTool.type, out tool))
        {
            foreach (var t in tool)
                t.SetActive(false);
        }
        StartCoroutine(MoneySpawner(Random.Range(2, 5)));
    }

    private IEnumerator MoneySpawner (int _money)
    {
        while (_money > 0)
        {
            var m = Instantiate(money, transform.position, transform.rotation);
            m.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            var startPos = m.transform.position;
            float randomX = Random.Range(-1.75f, 1.75f);
            float randomZ = Random.Range(-1.75f, 1.75f);
            float randomY = Random.Range(1.2f, 1.6f);
            Vector3[] path = new[] { new Vector3(startPos.x + 0.2f * randomX, randomY * 0.5f, startPos.z + 0.2f * randomZ),
                new Vector3(startPos.x + 0.5f * randomX, randomY, startPos.z + 0.5f * randomZ),
                new Vector3(startPos.x + 0.8f * randomX, randomY * 0.5f, startPos.z + 0.8f * randomZ),
            new Vector3(startPos.x + randomX, 0.25f, startPos.z + randomZ)};
            m.transform.DOPath(path, 0.8f);
            _money--;
        }
        yield return new WaitForSeconds(0.05f);
        if (_money > 0)
            StartCoroutine(MoneySpawner(_money));
    }

    private void FindTool()
    {
        currentTool = ToolsHandler.Instance.GetFreeTool();
        if (currentTool != null)
        {
            currentTool.SetVisitor(this);
            target = new Vector3(currentTool.transform.position.x, transform.position.y, currentTool.transform.position.z);
            agent.stoppingDistance = currentTool.waitingDistance;
            agent.SetDestination(target);
        }
    }

    private void WaitingAnimation()
    {
        switch (currentTool.type)
        {
            case ToolType.BENCH:
                anim.Play("Lying");
                break;
            case ToolType.BIKE:
                anim.Play("CyclingIdle");
                break;
            default:
                anim.Play("Idle");
                break;
        }
    }

    private void TrainingAnimation()
    {
        switch (currentTool.type)
        {
            case ToolType.BENCH:
                anim.Play("Bench");
                break;
            case ToolType.BAG:
                anim.Play("Punch");
                break;
            case ToolType.TREADMILL:
                anim.Play("Run");
                break;
            case ToolType.BIKE:
                anim.Play("Cycling");
                break;
            case ToolType.SQUAT:
                anim.Play("Squat");
                break;
            case ToolType.BARBELL:
                anim.Play("Bicep");
                break;
        }
    }

    private void OnGUI()
    {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (task == TaskType.WAITING)
        {
            GUI.DrawTexture(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 400, 200, 200), currentTool.texture);
        }
    }
}
