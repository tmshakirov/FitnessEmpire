using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class BuildScript : SerializedMonoBehaviour
{
    private float buildTimer;
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


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<StickmanController>().IsMoving())
            {
                buildTimer -= Time.deltaTime * 60;
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


    public void AddMoney(Transform player)
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
            var t = Instantiate(tool.tool, new Vector3 (transform.position.x + tool.position.x,
                tool.position.y, transform.position.z + tool.position.z), Quaternion.Euler(tool.angles));
            ToolsHandler.Instance.AddTool(t);
            Destroy(gameObject);
        }
        player.position = new Vector3(player.position.x, player.position.y, player.position.z - 1);
    }
}

[System.Serializable]
public class BuildingTool
{
    public ToolScript tool;
    public Vector3 angles;
    public Vector3 position;
}
