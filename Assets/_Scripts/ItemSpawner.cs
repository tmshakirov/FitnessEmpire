using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemScript item;
    [SerializeField] private float itemTimer, maxItemTimer = 300;
    [SerializeField] private Sprite itemTexture;
    [SerializeField] private Image itemCircle, progressCircle, progressCirclePrefab;
    [SerializeField] private RectTransform canvasRect;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        progressCircle = Instantiate(progressCirclePrefab, canvasRect.transform);
        Instantiate(itemCircle, progressCircle.transform).sprite = itemTexture;
        progressCircle.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            itemTimer += Time.deltaTime * 60;
            progressCircle.gameObject.SetActive(true);
            progressCircle.fillAmount = itemTimer / maxItemTimer;
            if (itemTimer >= maxItemTimer)
            {
                var i = Instantiate(item, transform.position, Quaternion.identity);
                other.GetComponent<StickmanController>().AddItem(i);
                itemTimer = 0;
            }
        }
    }

    private void Update()
    {
        if (progressCircle.enabled)
        {
            float offsetPosY = transform.position.y + 1.5f;
            Vector3 offsetPos = new Vector3(transform.position.x, offsetPosY, transform.position.z);
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);
            progressCircle.transform.localPosition = new Vector2(canvasPos.x, canvasPos.y + 200);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        itemTimer = 0;
        progressCircle.gameObject.SetActive(false);
    }
}
