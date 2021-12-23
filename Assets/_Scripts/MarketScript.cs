using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketScript : MonoBehaviour
{
    private bool isOpen;
    private Transform player;
    private int coachIndex;
    [SerializeField] private TMP_Text speedPrice, speedDescription;
    [SerializeField] private GameObject speedBuy;
    [SerializeField] private Transform shopContent;
    [SerializeField] private List<Coach> coaches;
    [SerializeField] private GameObject buyPrefab;
    [SerializeField] private TrainerScript coachObject;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (var c in coaches)
        {
            var t = Instantiate(buyPrefab, shopContent.transform);
            c.priceCount = t.transform.Find("Price").GetComponent<TMP_Text>();
            switch (c.type)
            {
                case CoachType.ADS:
                    c.priceCount.text = "ADS";
                    break;
                case CoachType.MONEY:
                    c.priceCount.text = "$" + c.price;
                    break;
            }
            t.transform.Find("Description").GetComponent<TMP_Text>().text = "Coach " + (coaches.IndexOf(c) + 1).ToString();
            c.buyButton = t.transform.Find("Buy").gameObject;
            c.buyButton.GetComponent<Button>().onClick.AddListener(() => BuyCoach());
            c.buyButton.SetActive(c == coaches[0]);
            c.priceCount.gameObject.SetActive(c == coaches[0]);
        }
    }

    private void Update()
    {
        if (Vector3.Distance (transform.position, player.transform.position) <= 3)
        {
            if (!isOpen)
            {
                UIHandler.Instance.OpenShop();
                isOpen = true;
            }
        }
        if (Vector3.Distance(transform.position, player.transform.position) > 4)
        {
            if (isOpen)
            {
                UIHandler.Instance.CloseShop();
                isOpen = false;
            }
        }
    }

    public void BuySpeed()
    {
        int price = Mathf.CeilToInt(2000 * (UpgradeHandler.Instance.coachSpeed - 3.4f));
        float speed = UpgradeHandler.Instance.coachSpeed - 3.5f;
        if (StickmanController.Instance.EnoughMoney(price))
        {
            StickmanController.Instance.AddDollars(-price);
            UpgradeHandler.Instance.coachSpeed += 0.1f;
            if (UpgradeHandler.Instance.coachSpeed < 4.5f)
            {
                speedPrice.text = "$" + (price + 200);
                speedDescription.text = string.Format("Current speed : <u>{0:0.0}</u>", UpgradeHandler.Instance.coachSpeed);
            }
            else
            {
                speedBuy.SetActive(false);
            }
        }
    }

    public void BuyCoach ()
    {
        switch (coaches[coachIndex].type)
        {
            case CoachType.ADS:
                coaches[coachIndex].buyButton.SetActive(false);
                NextCoach();
                break;
            case CoachType.MONEY:
                if (StickmanController.Instance.EnoughMoney(coaches[coachIndex].price))
                {
                    StickmanController.Instance.AddDollars(-coaches[coachIndex].price);
                    Instantiate(coachObject, transform.position + transform.forward * 2, Quaternion.identity);
                    coaches[coachIndex].buyButton.SetActive(false);
                    coaches[coachIndex].priceCount.gameObject.SetActive(false);
                    NextCoach();
                }
                break;
        }
        
    }

    private void NextCoach()
    {
        coachIndex++;
        if (coachIndex < coaches.Count)
        {
            coaches[coachIndex].buyButton.SetActive(true);
            coaches[coachIndex].priceCount.gameObject.SetActive(true);
        }
    }
}

public enum CoachType { MONEY, ADS }

[System.Serializable]
public class Coach
{
    public CoachType type; 
    public int price;
    public GameObject buyButton;
    public TMP_Text priceCount;
}
