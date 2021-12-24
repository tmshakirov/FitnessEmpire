using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerobicsCoachScript : MonoBehaviour
{
    private enum AerobicsType { VISITOR, COACH }
    [SerializeField] private AerobicsType type;
    [SerializeField] private List<Animator> visitors;
    [SerializeField] private List<string> animations;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        SetAnimation();
    }

    private void SetAnimation()
    {
        if (type == AerobicsType.COACH)
        {
            string currentAnimation = animations[Random.Range(0, animations.Count)];
            anim.Play(currentAnimation);
            foreach (var v in visitors)
                v.Play(currentAnimation);
        }
    }
}
