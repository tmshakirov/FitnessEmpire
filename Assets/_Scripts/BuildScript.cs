using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

public class BuildScript : SerializedMonoBehaviour
{
    [SerializeField] protected GameObject confetti, light;
    protected float buildTimer;
    protected int buildCount;
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
            buildCount = 0;
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
                    if (buildCount < 100)
                        buildTimer = 8 - buildCount * 0.07f;
                    else
                        buildTimer = 1;
                    buildCount++;
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

            Instantiate(light, t.transform.position, Quaternion.identity);
            Instantiate(confetti, t.transform.position, Quaternion.identity);
            var curScale = t.transform.localScale;
            t.transform.DOScale(curScale * 0.8f, 0).OnComplete(() =>
           t.transform.DOScale(curScale * 1.1f, 0.1f).OnComplete(() =>
             t.transform.DOScale(curScale * 0.9f, 0.1f).OnComplete(() =>
             t.transform.DOScale(curScale * 1.05f, 0.15f).OnComplete(() =>
            t.transform.DOScale(curScale, 0.15f)))));

            t.transform.DOMoveY(finalPosY + 0.6f, 0.1f).OnComplete(() =>
              t.transform.DOMoveY(finalPosY - 0.05f, 0.1f).OnComplete(() =>
             t.transform.DOMoveY(finalPosY, 0.3f)));

            UIHandler.Instance.ShowBuildingText();
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
