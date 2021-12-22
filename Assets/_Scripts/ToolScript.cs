using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ToolType { BENCH, TREADMILL, BAG, BARBELL, SQUAT, BIKE }

public class ToolScript : MonoBehaviour
{
    public bool busy;
    public List<MeshRenderer> meshes;
    public float trainingDistance = 0.75f;
    public float waitingDistance;
    public ToolType type;
    public VisitorScript currentVisitor;
    public Texture2D texture;

    public void SetVisitor (VisitorScript _visitor)
    {
        busy = true;
        currentVisitor = _visitor;
    }

    public void SetDisappearing (bool _disable)
    {
        foreach (var m in meshes)
            m.enabled = !_disable;
    }

    public void SetFree()
    {
        busy = false;
        currentVisitor = null;
        SetDisappearing(false);
    }
}
