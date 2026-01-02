using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Build;

[CreateAssetMenu(fileName = "BattleTargetDataSource", menuName = "Scriptable Objects/BattleTargetDataSource")]
public class BattleTargetDataSource : ScriptableObject
{
    [Header("Target List")] // make available to view for debug
    [SerializeField] private List<GameObject> targetGameObjects =  new();

    [Header("Binding Sources: Target Names")]
    public string firstTargetName;
    public string secondTargetName;
    public string thirdTargetName;
    public string fourthTargetName;


    private void Awake()
    {
        targetGameObjects.Capacity = 4; // cap limit at 4
    }

    public bool TryAddTarget(GameObject targetObject)
    {
        if (targetGameObjects.Count == targetGameObjects.Capacity)
            return false;

        targetGameObjects.Add(targetObject);
        UpdateBoundNames();
        return true;
    }

    public bool TryRemoveTargetAtPosition(int index)
    {
        if (index >= targetGameObjects.Count) 
            return false;

        targetGameObjects.RemoveAt(index);
        UpdateBoundNames();
        return true;
    }

    public bool TryRemoveTargetByReference(GameObject targetObject)
    {
        bool success = targetGameObjects.Remove(targetObject);
        UpdateBoundNames();
        return success;
    }

    private void UpdateBoundNames()
    {
        string targetName = "";
        PlayerUnit currentUnitIfPlayer = null;

        // go through the binding stuff & update stored names
        for (int i = 0; i < targetGameObjects.Count; i++)
        {
            currentUnitIfPlayer = targetGameObjects[i].GetComponent<PlayerUnit>();

            if (currentUnitIfPlayer is not null)
            {
                // get name via player name
                targetName = currentUnitIfPlayer.playerName;
            }
            else
            {
                targetName = targetGameObjects[i].name;
            }

            UpdateNumberedName(i, targetName);
        }
    }

    public void ClearTargets()
    {
        targetGameObjects.Clear();
        // update the bound data
        for (int i = 0; i < targetGameObjects.Capacity; i++)
        {
            UpdateNumberedName(i, "");
        }
    }

    private void UpdateNumberedName(int num, string newName)
    {
        switch (num) 
        {
            case 0:
                firstTargetName = newName;
                break;
            case 1:
                secondTargetName = newName;
                break;
            case 2:
                thirdTargetName = newName;
                break;
            case 3:
                fourthTargetName = newName;
                break;
        }
    }
}
