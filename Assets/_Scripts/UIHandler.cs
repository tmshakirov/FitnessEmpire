using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : Singleton<UIHandler>
{
    public TMP_Text dollarCounter;

    public void SetCount (int _amount)
    {
        dollarCounter.text = _amount + "$";
    }
}
