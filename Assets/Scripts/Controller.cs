using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    [SerializeField]
    private Actor actor;

    public Actor Actor => actor;

    protected virtual void Update()
    {
        if (Actor.CanAct && !Actor.IsActing && Actor.Hand.Count < actor.HandCapacity)
        {
            Actor.Draw();
        }
    }
}