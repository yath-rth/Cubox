using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upgradeManager : MonoBehaviour
{
    Gun gunClass;
    Points pointsManager;

    [SerializeField] int thresold;
    [SerializeField] GameObject upgradeUI;

    public List<upgradeClass> upgrades;
    List<StatValue> appliedStatUpgrades = new List<StatValue>();
    List<upgradeNonStats> appliedNonStatUpgrades = new List<upgradeNonStats>();

    int upgradeCount = 0;

    private void Start()
    {
        gunClass = Player.playerInstance.GetComponent<Gun>();
        pointsManager = Points.instance;

        Points.pointsAdded += checkForUpgrade;
    }

    void checkForUpgrade()
    {
        if (upgradeCount < 4)
        {
            if (pointsManager.getPoints() > thresold)
            {
                Time.timeScale = 0;
                thresold += pointsManager.getPoints();
                upgradeUI.SetActive(true);
            }
       }
    }

    public void onButtonClickStat(int index)
    {
        foreach (var stat in upgrades)
        {
            if (stat.weapon == true)
            {
                if (stat.stat == gunClass.activeGun)
                {
                    foreach (var item in stat.StatUpgrades)
                    {
                        if (containsStatsUpgrades(item.requiredUgrade) && containsNonStatsUpgrades(item.requiredNonStatUgrade))
                                       //         && !appliedStatUpgrades.Contains(item))
                        {
                            if (item.type == (UpgradeTypes)index)
                            {
                         //       appliedStatUpgrades.Add(item);
                                Debug.Log(item.name);
                           //     stat.stat.unlockUpgrade(item);
                                upgradeUI.SetActive(false);
                                Time.timeScale = 1;
                                upgradeCount++;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Do not have required");
                        }
                    }
                }
            }

            if (stat.weapon == false)
            {
                foreach (upgradeStats item in stat.StatUpgrades)
                {
                    if (item.guntype.Contains(gunClass.activeGun.guntype))
                    {
                        if (containsStatsUpgrades(item.requiredUgrade) && containsNonStatsUpgrades(item.requiredNonStatUgrade))
//                                            && appliedStatUpgrades.Contains(item) == false)
                        {
                            if (item.type == (UpgradeTypes)index)
                            {
                         //       appliedStatUpgrades.Add(item);
                                Debug.Log(item.name);
                           //     stat.stat.unlockUpgrade(item);
                                upgradeUI.SetActive(false);
                                Time.timeScale = 1;
                                upgradeCount++;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Do not have required");
                        }
                    }
                }
            }
        }
    }

    public void onButtonClickNONStat(int index)
    {
        foreach (var stat in upgrades)
        {
            if (stat.weapon == true)
            {
                if (stat.stat == gunClass.activeGun)
                {
                    foreach (var item in stat.nonStatUpgrades)
                    {
                        if (containsNonStatsUpgrades(item.requiredNonStatUgrade) && containsStatsUpgrades(item.requiredUgrade)
                                                && !appliedNonStatUpgrades.Contains(item))
                        {
                            if (item.type == (UpgradeTypes)index)
                            {

                                if (item.type == UpgradeTypes.muzzleUnlock) gunClass.muzzleUnlock(item.useORdont);

                                appliedNonStatUpgrades.Add(item);
                                //stat.stat.unlockUpgrade(item);
                                upgradeUI.SetActive(false);
                                Time.timeScale = 1;
                                upgradeCount++;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Do not have required");
                        }
                    }
                }
            }

            if (stat.weapon == false)
            {
                foreach (var item in stat.nonStatUpgrades)
                {
                    if (item.guntype.Contains(gunClass.activeGun.guntype))
                    {
                        if (containsNonStatsUpgrades(item.requiredNonStatUgrade) && containsStatsUpgrades(item.requiredUgrade)
                                            && appliedNonStatUpgrades.Contains(item) ==  false)
                        {
                            if (item.type == (UpgradeTypes)index)
                            {
                                appliedNonStatUpgrades.Add(item);
                                //stat.stat.unlockUpgrade(item);
                                upgradeUI.SetActive(false);
                                Time.timeScale = 1;
                                upgradeCount++;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Do not have required");
                        }
                    }
                }
            }
        }
    }

    bool containsStatsUpgrades(List<upgradeStats> upgrades)
    {
        int count = 0;
        if (upgrades.Count > 0)
        {
            foreach (var item in upgrades)
            {
                //if (appliedStatUpgrades.Contains(item)) count++;
            }
            if (count == upgrades.Count) return true;
            return false;
        }
        return true;
    }

    bool containsNonStatsUpgrades(List<upgradeNonStats> nonStatupgrades)
    {
        int count = 0;
        if (nonStatupgrades.Count > 0)
        {
            foreach (var item in nonStatupgrades)
            {
                if (appliedNonStatUpgrades.Contains(item)) count++;
            }
            if (count == nonStatupgrades.Count) return true;
            return false;
        }
        return true;
    }
}

[System.Serializable]
public class upgradeClass
{
    public Stats stat;
    public bool weapon;
    public List<upgradeStats> StatUpgrades;
    public List<upgradeNonStats> nonStatUpgrades;
}
