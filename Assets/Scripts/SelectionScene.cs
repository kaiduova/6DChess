using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionScene : MonoBehaviour
{
    public static SelectionScene Instance { get; private set; }
    public int NumSelectedCards { get; set; }

    [SerializeField]
    public int maxNumSelectedCards, cardsToAddPerSelection;

    private List<SelectableCard> _selectableCards = new();

    [SerializeField]
    private float spacing;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(List<GameObject> selectableCardPrefabs)
    {
        NumSelectedCards = 0;
        foreach (var card in _selectableCards)
        {
            Destroy(card.gameObject);
        }
        _selectableCards.Clear();
        
        List<GameObject> instantiatedCardPrefabs = new();
        var previousPosition = Vector3.zero;
        foreach (var prefab in selectableCardPrefabs)
        {
            instantiatedCardPrefabs.Add(Instantiate(prefab, previousPosition, Quaternion.Euler(0, 180, 0)));
            previousPosition += new Vector3(spacing, 0f);
        }
        previousPosition -= new Vector3(spacing, 0f);
        var moveLeftBy = previousPosition.x / 2f;
        foreach (var instanceTransform in instantiatedCardPrefabs.Select(instance => instance.transform))
        {
            instanceTransform.position -= new Vector3(moveLeftBy, 0f);
        }

        for (var i = 0; i < cardsToAddPerSelection; i++)
        {
            _selectableCards = instantiatedCardPrefabs
                .Select(instance => instance.gameObject.GetComponent<SelectableCard>()).ToList();
        }
    }

    public void ConfirmButton()
    {
        if (NumSelectedCards != maxNumSelectedCards) return;
        GameManager.Instance.AddedCardPrefabs.AddRange(_selectableCards.Where(card => card.Selected).Select(card => card.cardPrefab));
        GameManager.Instance.LoadNextGameScene();
    }

    public void DeselectAll()
    {
        foreach (var card in _selectableCards)
        {
            card.Selected = false;
        }
    }
}