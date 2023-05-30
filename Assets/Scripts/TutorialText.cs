using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TextSectionTrigger
{
    PiecePlaced,
    PieceDestroyed,
    DamageDealt,
    Hovered
}

public class TutorialText : MonoBehaviour
{
    //Starting text should just be in the box at the start, sections are only applied when triggered.

    [SerializeField]
    private TMP_Text tmpTextBox;

    //Each section can only be triggered once.
    [SerializeField, TextArea]
    private string[] textSections;

    private List<int> _triggeredIndices = new();

    public static TutorialText Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerSection(int index)
    {
        if (_triggeredIndices.Contains(index)) return;
        tmpTextBox.text = textSections[index];
        _triggeredIndices.Add(index);
    }
}