using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandler : Singleton<UpgradeHandler>
{
    public int currentUpgrade;
    [SerializeField] private List<UpgradeScript> upgrades;

    public void NextUpgrade()
    {
        currentUpgrade++;
        foreach (var u in upgrades)
        {
            if (u.gameObject.activeSelf)
                u.CheckUnlocked();
            else
                u.gameObject.SetActive(u.minLevel <= currentUpgrade);
        }
    }
}
