using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ToolsHandler : Singleton<ToolsHandler>
{
    public List<ToolScript> tools;

    private void Start()
    {
        tools = FindObjectsOfType<ToolScript>().ToList();
    }

    public void ResetTools()
    {
        tools = FindObjectsOfType<ToolScript>().ToList();
    }

    public void InitTool (ToolScript _tool)
    {
        StartCoroutine(AddTool(_tool));
    }

    private IEnumerator AddTool (ToolScript _tool)
    {
        yield return new WaitForSeconds(0.15f);
        tools.Add(_tool);
        foreach (var v in FindObjectsOfType<VisitorSpawner>())
            v.ChangeLimit();
    }

    public ToolScript GetFreeTool()
    {
        List<ToolScript> freeTools = new List<ToolScript>();
        foreach (var t in tools)
        {
            if (!t.busy)
                freeTools.Add(t);
        }
        if (freeTools.Count < 1)
            return null;
        return freeTools[Random.Range (0, freeTools.Count)];
    }
}
