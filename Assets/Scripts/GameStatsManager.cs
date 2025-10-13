using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }

    private Dictionary<string, int> stats = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeStat(string statName, int amount)
    {
        if (!stats.ContainsKey(statName))
            stats[statName] = 0;

        stats[statName] += amount;
        Debug.Log($"Stat {statName} is now {stats[statName]}");
    }

    public int GetStat(string statName)
    {
        return stats.ContainsKey(statName) ? stats[statName] : 0;
    }
}
