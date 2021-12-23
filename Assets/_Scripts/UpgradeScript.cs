using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
            {
                capacity++;
                var m = Instantiate(money, player.position, Quaternion.identity);
                m.transform.DOScale(0.3f, 0.15f);
                m.transform.DOMove(transform.position, 0.15f).OnComplete(() =>
                {
                    Destroy(m.gameObject);
                });
            }
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

    public override void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<StickmanController>().IsMoving() && IsUnlocked())
            {
                buildTimer -= Time.deltaTime * 150;
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

    public override void BuildTool(Transform player)
    {
        if (spawnPos == Vector3.zero)
        {
            spawnPos = transform.position;
            UpgradeHandler.Instance.RemovePreviousBarrier(roomType, minLevel);
            extension = Instantiate(upgrade, spawnPos, Quaternion.identity);
            extension.transform.Find("Barrier").gameObject.SetActive(true);
            UIHandler.Instance.ShowUpgradeText();
        }
        else
        {
            upgrade.SetActive(true);
            extension = upgrade;
            ToolsHandler.Instance.ResetTools();
            UIHandler.Instance.ShowRoomText();
        }
        Instantiate(confetti, transform.position, Quaternion.identity);
        var curScale = extension.transform.localScale;
        Camera.main.transform.DOShakePosition(0.5f, 0.2f);
        extension.transform.DOScale(curScale * 0.6f, 0).OnComplete(() =>
       extension.transform.DOScale(curScale * 1.1f, 0.1f).OnComplete(() =>
         extension.transform.DOScale(curScale * 0.9f, 0.1f).OnComplete(() =>
         extension.transform.DOScale(curScale * 1.05f, 0.15f).OnComplete(() =>
        extension.transform.DOScale(curScale, 0.15f)))));
        gameObject.SetActive(false);
        UpgradeHandler.Instance.NextUpgrade(roomType, upgradeType);
        NavmeshBaker.Instance.UpdateNavmesh();
    }
}