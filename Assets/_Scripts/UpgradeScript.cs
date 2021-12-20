using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType { UPGRADE, NEWROOM }

public class UpgradeScript : BuildScript
{
    public UpgradeType upgradeType;
    public int minLevel;
    public GameObject upgrade;
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private GameObject lockSprite, buildSprite;

    private bool IsUnlocked()
    {
        return minLevel <= UpgradeHandler.Instance.currentUpgrade;
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

    public override void BuildTool(Transform player)
    {
        if (spawnPos == Vector3.zero)
        {
            spawnPos = transform.position;
            Instantiate(upgrade, spawnPos, Quaternion.identity);
        }
        else
        {
            upgrade.SetActive(true);
        }
        gameObject.SetActive(false);
        UpgradeHandler.Instance.NextUpgrade();
        NavmeshBaker.Instance.UpdateNavmesh();
    }
}