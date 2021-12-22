using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorRandomizer : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer visitor;
    [SerializeField] private List<Material> shirtMaterials, pantsMaterials;

    void Start()
    {
        var copyMats = visitor.sharedMaterials;
        copyMats[1] = shirtMaterials[Random.Range(0, shirtMaterials.Count - 1)];
        copyMats[2] = pantsMaterials[Random.Range(0, pantsMaterials.Count - 1)];
        visitor.sharedMaterials = copyMats;
    }
}
