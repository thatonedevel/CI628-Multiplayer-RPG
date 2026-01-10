using System;

[Serializable]
public abstract class AItem
{
    public string ItemName;
    public string Description;
    public int strength;
    public TargetType typeOfTarget;
    public bool isSingleTarget = true;

    public abstract void UseItem();
}
