using UnityEngine.UI;
using UnityEngine;
using System;
using _game.Scripts.System;
using UnityEngine.Serialization;

public class KillFeed : MonoBehaviour
{
    [FormerlySerializedAs("weaponIcons")]
    [SerializeField] private Sprite[] _weaponIcons;
    [FormerlySerializedAs("suicideIcon")]
    [SerializeField] private Sprite _suicideIcon;
    [FormerlySerializedAs("killLogs")]
    [SerializeField] private KillFeedLog[] _killLogs;

    private int _currentLogIndex;


    public void AddKillLog(string killerName, int weaponIndex, string victimName)
    {
        //Debug.Log("killer: " + killerName + " wepIndex: " + weaponIndex + " victim: " + victimName + " currentLogIndex: " + _currentLogIndex);

        KillFeedLog killLog = _killLogs[_currentLogIndex];
        Sprite weaponIcon;

        if (killerName == "")
        {
            weaponIcon = _suicideIcon;
        }
        else
        {
            weaponIcon = _weaponIcons[weaponIndex];
        }

        killLog.Spawn(killerName, victimName, weaponIcon);

        killLog.transform.SetAsLastSibling();

        _currentLogIndex++;
        if (_currentLogIndex >= _killLogs.Length)
            _currentLogIndex = 0;
    }

    private void OnEnable() { PlayerStats.UpdateKillFeed += AddKillLog; }

    private void OnDisable() { PlayerStats.UpdateKillFeed -= AddKillLog; }
}
