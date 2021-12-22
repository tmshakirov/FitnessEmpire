using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType { UPGRADE, NEWROOM }
public enum RoomType { DEFAULT, LEFT, RIGHT }

public class UpgradeScript : BuildScript
{
    public UpgradeType upgradeType;
    public RoomType roomType;
    public int minLevel;
    public GameObject upgrade;
    private GameObject extension;
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private GameObject lockSprite, buildSprite;

    private bool IsUnlocked()
    {
        return minLevel <= UpgradeHandler.Instance.GetCurrentLevel (roomType);
    }

    public override void AddMoney(Transform player)
    {
        if (IsUnlocked())
        {
            if (capacity <= maxCapacity)
                capacity++;
            if (maxCapacity - capacity >= 0)
                capacityText.text = (maxCapacity - capacity).ToString();
            if (capacity >= maxCapacity)
            {
                BuildTool(player);
            }
        }
    }

    public void CheckUnlocked()
    {
        if (lockSprite != null)
        {
            lockSprite.SetActive(!IsUnlocked());
            buildSprite.SetActive(IsUnlocked());
        }
    }

    public void RemoveBarrier ()
    {
        try
        {
            extension.transform.Find("Barrier").gameObject.SetActive(false);
        }
        catch
        {
            Debug.Log("Unable to remove barrier for " + minLevel);
        }
    }

    public void CheckActivity (int _level)
    {
        if (minLevel == _level)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public override void BuildTool(Transform player)
    {
        if (spawnPos == Vector3.zero)
        {
            spawnPos = transform.position;
            UpgradeHandler.Instance.RemovePreviousBarrier(roomType, minLevel);
            extension = Instantiate(upgrade, spawnPos, Quaternion.identity);
            extension.transform.Find("Barrier").gameObject.SetActive(true);
        }
        else
        {
            upgrade.SetActive(true);
            ToolsHandler.Instance.ResetTools();
        }
        gameObject.SetActive(false);
        UpgradeHandler.Instance.NextUpgrade(roomType, upgradeType);
        NavmeshBaker.Instance.UpdateNavmesh();
    }
}