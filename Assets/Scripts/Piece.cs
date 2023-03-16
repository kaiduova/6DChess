using System.Linq;
using UnityEngine;

public delegate void PieceFinishCallback();

public class Piece : MonoBehaviour
{
    private Action[] _orderedActions;
    private int _actionIterator;
    private PieceFinishCallback _finishCallback;
    
    public Actor Actor { get; set; }
    public Tile Tile { get; set; }

    private void Awake()
    {
        var actions = GetComponents<Action>();
        _orderedActions = actions.OrderBy(action => action.ActionOrder).ToArray();
    }

    public void Act(PieceFinishCallback callback)
    {
        _finishCallback = callback;
        NextAction();
    }

    private void NextAction()
    {
        if (_actionIterator >= _orderedActions.Length)
        {
            _actionIterator = 0;
            _finishCallback();
            return;
        }
        _actionIterator++;
        _orderedActions[_actionIterator - 1].PerformAction(NextAction);
    }
}
