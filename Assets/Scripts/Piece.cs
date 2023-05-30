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
    
    private bool _isFlipped;

    private bool _iconsFlipped;
    
    public bool InSludge { get; set; }
    
    public int Damage { get => damage; }
    public Actor Actor { get; set; }
    public Tile Tile { get; set; }

    [SerializeField]
    public Sprite pieceInfo;

    [SerializeField]
    public bool isVengeful;

    [SerializeField]
    private int lifestealHealValue;
    
    private int _pauseCounter;

    [SerializeField]
    private int pauseTurnsAfterTurn;

    [SerializeField]
    private bool isForwardMoving;

    public bool QueueDestroy { get; set; }

    public bool IsFlipped
    {
        get => _isFlipped;
        set => _isFlipped = value;
    }

    private void Awake()
    {
        var actions = GetComponents<Action>();
        _orderedActions = actions.OrderBy(action => action.ActionOrder).ToArray();
    }

    private void Start()
    {
        if (indicatorIcons != null)
        {
            for (var i = 0; i < indicatorIcons.Length; i++)
            {
                //Set parent and relative locations and disable.
                indicatorIcons[i].iconObject = Instantiate(indicatorIcons[i].iconObject, gameObject.transform);
                var relativePosition =
                    Tile.RelativeCoordinateToRelativePosition(indicatorIcons[i].relativeCoordinate, 2);
                indicatorIcons[i].iconObject.transform.localPosition =
                    Actor.Side == Side.Normal ? relativePosition : InvertZ(relativePosition);
                indicatorIcons[i].iconObject.SetActive(false);
            }
        }
    }

    public void ShowIcons()
    {
        if (indicatorIcons == null) return;
        for (var i = 0; i < indicatorIcons.Length; i++)
        {
            indicatorIcons[i].iconObject.SetActive(true);
            //Check for mirrored movement.
            if (IsFlipped && !_iconsFlipped)
            {
                indicatorIcons[i].iconObject.transform.localPosition =
                    InvertX(indicatorIcons[i].iconObject.transform.localPosition);
            }

            if (!IsFlipped && _iconsFlipped)
            {
                indicatorIcons[i].iconObject.transform.localPosition =
                    InvertX(indicatorIcons[i].iconObject.transform.localPosition);
            }
        }

        _iconsFlipped = IsFlipped;
    }

    public void HideIcons()
    {
        if (indicatorIcons == null) return;
        for (var i = 0; i < indicatorIcons.Length; i++)
        {
            indicatorIcons[i].iconObject.SetActive(false);
        }
    }

    public void Act(PieceFinishCallback callback)
    {
        _finishCallback = callback;
        if (_pauseCounter > 0)
        {
            _pauseCounter--;
            callback();
            return;
        }
        _pauseCounter = pauseTurnsAfterTurn;
        NextAction();
    }

    private void NextAction()
    {
        if (QueueDestroy)
        {
            Destroy();
            _finishCallback();
            return;
        }
        
        if (Tile.OwningActor != null && Actor != Tile.OwningActor)
        {
            Tile.OwningActor.Health -= damage;
            Actor.Health += lifestealHealValue;
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

    private void Update()
    {
        if (isForwardMoving) return;
        if (Actor.Side == Side.Normal)
        {
            transform.GetChild(0).rotation = _isFlipped ? Quaternion.Euler(0, -60, 0) : Quaternion.Euler(0, 60, 0);
        }
        else
        {
            transform.GetChild(0).rotation = _isFlipped ? Quaternion.Euler(0, 120, 0) : Quaternion.Euler(0, -120, 0);
        }
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
