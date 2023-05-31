using System;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private bool _enlarged;
    
    private void Start()
    {
        if (_enlarged)
        {
            transform.localScale /= 1.2f;
            _enlarged = false;
        }
    }

    public void Enlarge()
    {
        if (_enlarged) return;
        transform.localScale *= 1.2f;
        _enlarged = true;
    }

    public void Shrink()
    {
        if (!_enlarged) return;
        transform.localScale /= 1.2f;
        _enlarged = false;
    }
}