using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandler : Singleton<UpgradeHandler>
{
    private int currentUpgrade;
    [SerializeField] private List<UpgradeScript> upgrades;

    public void NextUpgrade()
    {
        currentUpgrade++;
        if (currentUpgrade < upgrades.Count)
            upgrades[currentUpgrade].gameObject.SetActive(true);
    }
}
