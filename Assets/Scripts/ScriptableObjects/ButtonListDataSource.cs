using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ButtonListDataSource", menuName = "Scriptable Objects/ButtonListDataSource")]
public class ButtonListDataSource : ScriptableObject
{
    // yes i am writing my own backend for a listview

    [Header("Button Text bindings")]
    public string label0 = "";
    public string label1 = "";
    public string label2 = "";
    public string label3 = "";

    public int labelStartIndex { get; private set; } = -1; // internal values for tracking 
    public int labelEndIndex { get; private set; } = -1;

    [Header("Main Data")]
    [SerializeField] private List<string> textDataList = new();

    public void Awake()
    {
        labelEndIndex = textDataList.Count == 0 ? -1 : textDataList.Count - 1;
        labelStartIndex = textDataList.Count == 0 ? -1 : labelStartIndex;
        UpdateDisplayList();
    }

    public void AddItem(string item)
    {
        textDataList.Add(item);
        UpdateDisplayList();
    }

    public void RemoveItem(string data)
    {
        textDataList.Remove(data);
        UpdateDisplayList();
    }

    public void RemoveItemAtIndex(int index)
    {
        textDataList.RemoveAt(index);
        UpdateDisplayList();
    }

    public void UpdateDisplayList()
    {
        // grab the list, go through it and update labels

        int end = labelEndIndex < 3 ? textDataList.Count - 1 : 3;

        for (int i = 0; i < end; i++)
        {
            switch (i)
            {
                case 0:
                    label0 = textDataList[labelStartIndex + i];
                    break;
                case 1:
                    label1 = textDataList[labelStartIndex + i];
                    break;
                case 2:
                    label2 = textDataList[labelStartIndex + i];
                    break;
                case 3:
                    label3 = textDataList[labelStartIndex + i];
                    break;
            }
        }
    }


    public void TryScrollUp()
    {
        if (labelStartIndex > 0)
        {
            labelStartIndex--;
            labelEndIndex--;
        }
    }

    public void TryScrollDown()
    {
        if (labelEndIndex < textDataList.Count - 1)
        {
            labelEndIndex++;
            labelStartIndex++;
        }
    }

    public void RefreshScroll()
    {
        // resets the scroll to the top of the list

        labelStartIndex = 0;
        labelEndIndex = textDataList.Count > 4? 3: textDataList.Count - 1;
        UpdateDisplayList();
    }

    public void ClearStringList()
    {
        textDataList.Clear();
    }
}
