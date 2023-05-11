using System;
using System.Linq;
using Actions;
using UnityEngine;

public delegate void PieceFinishCallback();

[Serializable]
public struct IndicatorIcon
{
    public Vector2 relativeCoordinate;
    public GameObject iconObject;
}

public class Piece : MonoBehaviour
{
    private Action[] _orderedActions;
    private int _actionIterator;
    private PieceFinishCallback _finishCallback;

    [SerializeField]
    private int damage;

    [SerializeField]
    private IndicatorIcon[] indicatorIcons;

    [SerializeField]
    private bool isFlipped;

    private bool _iconsFlipped;
    
    public bool InSludge { get; set; }
    
    public int Damage { get => damage; }
    public Actor Actor { get; set; }
    public Tile Tile { get; set; }

    public bool IsFlipped
    {
        get => isFlipped;
        set => isFlipped = value;
    }

    private void Awake()
    {
        var actions = GetComponents<Action>();
        _orderedActions = actions.OrderBy(action => action.ActionOrder).ToArray();
    }

    private void Start()
    {
        for (var i = 0; i < indicatorIcons.Length; i++)
        {
            indicatorIcons[i].iconObject = Instantiate(indicatorIcons[i].iconObject, gameObject.transform);
            var relativePosition = Tile.RelativeCoordinateToRelativePosition(indicatorIcons[i].relativeCoordinate, Tile.MaxWidth);
            indicatorIcons[i].iconObject.transform.localPosition =
                Actor.Side == Side.Normal ? relativePosition : InvertZ(relativePosition);
            indicatorIcons[i].iconObject.SetActive(false);
            //Set parent and relative locations and disable.
        }
    }

    public void ShowIcons()
    {
        for (var i = 0; i < indicatorIcons.Length; i++)
        {
            indicatorIcons[i].iconObject.SetActive(true);
            //Check for mirrored movement.
            if (IsFlipped && !_iconsFlipped)
            {
                indicatorIcons[i].iconObject.transform.position =
                    InvertX(indicatorIcons[i].iconObject.transform.position);
            }

            if (!IsFlipped && _iconsFlipped)
            {
                indicatorIcons[i].iconObject.transform.position =
                    InvertX(indicatorIcons[i].iconObject.transform.position);
            }
        }

        _iconsFlipped = IsFlipped;
    }

    public void HideIcons()
    {
        for (var i = 0; i < indicatorIcons.Length; i++)
        {
            indicatorIcons[i].iconObject.SetActive(false);
        }
    }

    public void Act(PieceFinishCallback callback)
    {
        _finishCallback = callback;
        NextAction();
    }

    private void NextAction()
    {
        if (Tile.OwningActor != null && Actor != Tile.OwningActor)
        {
            Tile.OwningActor.Health -= damage;
            Destroy();
            _finishCallback();
            return;
        }
        
        if (_actionIterator >= _orderedActions.Length)
        {
            _actionIterator = 0;
            _finishCallback();
            return;
        }
        _actionIterator++;
        _orderedActions[_actionIterator - 1].PerformAction(NextAction);
    }

    public void Destroy()
    {
        Actor.DestroyPiece(this);
    }
    
    public static Vector3 InvertZ(Vector3 input)
    {
        Vector3 output;
        output.x = input.x;
        output.y = input.y;
        output.z = -input.z;
        return output;
    }
    
    public static Vector3 InvertX(Vector3 input)
    {
        Vector3 output;
        output.x = -input.x;
        output.y = input.y;
        output.z = input.z;
        return output;
    }
}
