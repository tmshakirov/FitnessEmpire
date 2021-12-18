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
