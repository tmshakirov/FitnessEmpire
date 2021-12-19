using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScript : BuildScript
{
    public GameObject upgrade;

    public override void BuildTool(Transform player)
    {
        Instantiate(upgrade, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        UpgradeHandler.Instance.NextUpgrade();
    }
}