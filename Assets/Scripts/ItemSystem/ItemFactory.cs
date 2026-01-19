using System;

public static class ItemFactory
{
    public static AItem CreateItem(string name)
    {
        var item = Activator.CreateInstance(null, false, true, 0, null, null);

        return item as AItem;
    }
}
