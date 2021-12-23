using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIHandler : Singleton<UIHandler>
{
    public TMP_Text dollarCounter;
    public TMP_Text label;
    [SerializeField] private List<string> buildingTexts;
    [SerializeField] private CanvasGroup shopCanvas;
    [SerializeField] private ScrollRect shopScroll;

    private void Start()
    {
        shopCanvas.gameObject.SetActive(false);
        shopCanvas.transform.localScale = Vector3.zero; 
    }

    public void SetCount (int _amount)
    {
        dollarCounter.text = "$" + _amount;
    }

    public void OpenShop()
    {
        shopCanvas.gameObject.SetActive(true);
        shopCanvas.transform.DOScale(1, 0.25f);
        shopScroll.verticalNormalizedPosition = 1;
    }

    public void CloseShop()
    {
        shopCanvas.transform.DOScale(0, 0.25f).OnComplete(() =>
        shopCanvas.gameObject.SetActive(false));
    }

    public void ShowBuildingText()
    {
        ShowText(buildingTexts[Random.Range(0, buildingTexts.Count)]);
    }

    public void ShowUpgradeText()
    {
        ShowText("New Part Unlocked!");
    }

    public void ShowRoomText()
    {
        ShowText("New Room Unlocked!");
    }

    public void ShowText (string _text)
    {
        label.DOFade(1, 0.75f);
        label.transform.DOScale(1, 0.75f);
        label.text = _text;
        StartCoroutine(FadingOut());
        StartCoroutine(ScaleOut());
    }

    private IEnumerator FadingOut()
    {
        yield return new WaitForSeconds(1f);
        label.DOFade(0, 1f);
    }

    private IEnumerator ScaleOut()
    {
        yield return new WaitForSeconds(1.25f);
        label.transform.DOScale(0, 0.5f);
    }
}
