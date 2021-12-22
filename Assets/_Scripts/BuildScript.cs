using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

public class BuildScript : SerializedMonoBehaviour
{
    [SerializeField] protected GameObject confetti;
    protected float buildTimer;
    public ToolType type;
    public Dictionary<ToolType, BuildingTool> builds;
    public int capacity, maxCapacity;
    public TMP_Text capacityText;

    private void Start()
    {
        buildTimer = 100;
        capacityText.text = maxCapacity.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            buildTimer = 100;
        }
    }


    public virtual void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<StickmanController>().IsMoving())
            {
                buildTimer -= Time.deltaTime * 120;
                if (buildTimer <= 0)
                {
                    if (other.GetComponent<StickmanController>().GetDollars() > 0)
                    {
                        other.GetComponent<StickmanController>().AddDollars(-1);
                        AddMoney(other.transform);
                    }
                    buildTimer = 5;
                }
            }
        }
    }


    public virtual void AddMoney(Transform player)
    {
        if (capacity <= maxCapacity)
            capacity++;
        if (maxCapacity-capacity >= 0)
            capacityText.text = (maxCapacity-capacity).ToString();
        if (capacity >= maxCapacity)
        {
            BuildTool(player);
        }
    }

    public virtual void BuildTool(Transform player)
    {
        BuildingTool tool = null;
        if (builds.TryGetValue (type, out tool))
        {
            var finalPosY = tool.position.y;
            var t = Instantiate(tool.tool, new Vector3 (transform.position.x + tool.position.x,
                tool.position.y, transform.position.z + tool.position.z), Quaternion.Euler(tool.angles));
            Instantiate(confetti, t.transform.position, Quaternion.identity);
            t.transform.DOMoveY(finalPosY + 0.6f, 0.15f).OnComplete(() =>
              t.transform.DOMoveY(finalPosY - 0.05f, 0.15f).OnComplete(() =>
             t.transform.DOMoveY(finalPosY, 0.2f)));
            ToolsHandler.Instance.AddTool(t);
            Destroy(gameObject);
        }
        player.position = new Vector3(player.position.x, player.position.y, player.position.z - 1);
        NavmeshBaker.Instance.UpdateNavmesh();
    }
}

[System.Serializable]
public class BuildingTool
{
    public ToolScript tool;
    public Vector3 angles;
    public Vector3 position;
}
