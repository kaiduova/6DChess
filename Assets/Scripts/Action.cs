using System;
using UnityEngine;

public delegate void ActionFinishCallback();

public abstract class Action : MonoBehaviour
{
    [SerializeField]
    private int actionOrder;

    protected Piece Piece { get; private set; }

    public int ActionOrder => actionOrder;

    protected virtual void Awake()
    {
        Piece = GetComponent<Piece>();
    }

    //Animation & logic here.
    public abstract void PerformAction(ActionFinishCallback callback);
}
