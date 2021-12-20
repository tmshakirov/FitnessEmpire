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
        //если продление комнаты - активировать, если minlevel равен текущему уровню
        //если апгрейд - если <= текущему уровню
        foreach (var u in upgrades)
        {
            switch (u.upgradeType)
            {
                case UpgradeType.NEWROOM:
                    u.CheckUnlocked();
                    break;
                case UpgradeType.UPGRADE:
                    u.gameObject.SetActive(u.minLevel == currentUpgrade);
                    break;
            }
        }
    }
}
