using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandler : Singleton<UpgradeHandler>
{
    public float coachSpeed = 3.5f;
    [SerializeField] private List<UpgradeScript> nextRooms;
    [SerializeField] private List<Upgrade> upgrades;

    public bool IsLast (UpgradeScript _upgrade)
    {
        return upgrades.Find(x => x.roomType == _upgrade.roomType).IsLast(_upgrade);
    }

    public int GetCurrentLevel (RoomType _roomType)
    {
        return upgrades.Find(x => x.roomType == _roomType).currentLevel;
    }

    public void RemovePreviousBarrier (RoomType _roomType, int _level)
    {
        if (_level > 0)
        {
            var upgrade = upgrades.Find(x => x.roomType == _roomType);
            if (_level < upgrade.upgrades.Count)
                upgrade.upgrades[_level - 1].RemoveBarrier();
        }

    }

    public void NextUpgrade(RoomType _type, UpgradeType _utype)
    {
        if (_utype == UpgradeType.UPGRADE)
            upgrades.Find(x => x.roomType == _type).CheckUpgrades();
        foreach (var n in nextRooms)
        {
            n.CheckUnlocked();
        }
    }
}

[System.Serializable]
public class Upgrade
{
    public int currentLevel;
    public RoomType roomType;
    public List<UpgradeScript> upgrades;

    public bool IsLast (UpgradeScript _upgrade)
    {
        return upgrades.IndexOf(_upgrade) == (upgrades.Count - 1);
    }

    public void CheckUpgrades()
    {
        currentLevel++;
        foreach (var u in upgrades)
        {
            u.CheckActivity(currentLevel);
        }
    }
}
