using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshBaker : Singleton<NavmeshBaker>
{
    public NavMeshSurface surface;

    private void Start()
    {
        surface.RemoveData();
        surface.BuildNavMesh();
    }

    public void UpdateNavmesh()
    {
        Invoke("NextFrame", 0.1f);
    }

    private void NextFrame()
    {
        surface.UpdateNavMesh(surface.navMeshData);
    }
}
