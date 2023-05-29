using System;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableCard : MonoBehaviour
{
    public bool Selected { get; private set; }

    [SerializeField]
    public GameObject cardPrefab;

    public void Select()
    {
        if (Selected) return;
        if (SelectionScene.Instance.maxNumSelectedCards > SelectionScene.Instance.NumSelectedCards)
        {
            Selected = true;
            gameObject.transform.localScale *= 1.2f;
            SelectionScene.Instance.NumSelectedCards++;
        }
    }

    public void Deselect()
    {
        if (!Selected) return;
        Selected = false;
        gameObject.transform.localScale /= 1.2f;
        SelectionScene.Instance.NumSelectedCards--;
    }
    
    private void OnMouseDown()
    {
        Select();
        Deselect();
    }
}