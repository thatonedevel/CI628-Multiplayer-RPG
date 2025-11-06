using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;

[CreateAssetMenu(menuName="Scriptable Objects/Enemy Data Source")]
public class EnemyDataSourceSO : ScriptableObject
{
    // store following properties: enemy name, network id
    public List<BasicEnemy> enemies = new List<BasicEnemy>();
    public List<ulong> enemyNetworkObjectIDs = new List<ulong>();
    public List<string> enemyNames = new List<string>();

    public void ClearEnemies()
    {
        enemies.Clear();
        enemyNetworkObjectIDs.Clear();
    }

    public void AddEnemyToList(BasicEnemy target)
    {
        enemies.Add(target);
        enemyNames.Add(target.enemyBaseName);
        enemyNetworkObjectIDs.Add(target.GetComponent<NetworkObject>().NetworkObjectId);
    }
}
