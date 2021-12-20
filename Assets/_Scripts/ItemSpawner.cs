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
    private StickmanController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StickmanController>();
        progressCircle = Instantiate(progressCirclePrefab, canvasRect.transform);
        Instantiate(itemCircle, progressCircle.transform).sprite = itemTexture;
        progressCircle.gameObject.SetActive(false);
    }

    public ToolType ItemType()
    {
        return item.type;
    }

    private void Update()
    {
        if (item.type != ToolType.BARBELL)
            transform.Rotate(Vector3.up * Time.deltaTime * 30);
        progressCircle.gameObject.SetActive(Vector3.Distance (transform.position, player.transform.position) <= 2);
        if (progressCircle.gameObject.activeSelf)
        {
            float offsetPosY = transform.position.y + 1.5f;
            Vector3 offsetPos = new Vector3(transform.position.x, offsetPosY, transform.position.z);
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);
            progressCircle.transform.localPosition = new Vector2(canvasPos.x, canvasPos.y + 200);

            itemTimer += Time.deltaTime * 90;
            progressCircle.fillAmount = itemTimer / maxItemTimer;
            if (itemTimer >= maxItemTimer)
            {
                var i = Instantiate(item, transform.position, Quaternion.identity);
                player.AddItem(i);
                itemTimer = 0;
            }
        }
    }

    public ItemScript SpawnItem ()
    {
        return Instantiate(item, transform.position, Quaternion.identity);
    }
}
