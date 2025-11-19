using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Sprite[] itemSprites;

    [Range(0.1f, 5.0f)]
    public float avoidRadius = 1.2f;

    public List<string> inventory = new List<string>();
    public int maxCapacity = 30;

    public Text noticeText;

    void Start()
    {
        if (noticeText != null) noticeText.gameObject.SetActive(false);

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(2f, 3f);
            yield return new WaitForSeconds(waitTime);

            int currentItems = FindObjectsOfType<ItemManager>().Length;

            if (currentItems < 2)
            {
                SpawnItem();
            }
        }
    }

    void SpawnItem()
    {
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float padding = 1.0f;
        float minX = -camWidth + padding;
        float maxX = camWidth - padding;
        float minY = -camHeight + padding;
        float maxY = camHeight - padding;

        Vector2 randomPos;
        int attemptCount = 0;

        do
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            randomPos = new Vector2(x, y);
            attemptCount++;
            if (attemptCount > 100) break;

        } while (Vector2.Distance(randomPos, Vector2.zero) < avoidRadius);

        GameObject newItem = Instantiate(itemPrefab, randomPos, Quaternion.identity);
        SpriteRenderer sr = newItem.GetComponent<SpriteRenderer>();
        if (itemSprites.Length > 0)
        {
            Sprite selectedSprite = itemSprites[Random.Range(0, itemSprites.Length)];
            sr.sprite = selectedSprite;
            newItem.name = selectedSprite.name;
        }
    }

    public bool SaveToInventory(string itemName)
    {
        if (inventory.Count >= maxCapacity)
        {
            StartCoroutine(ShowNotice("∞°πÊ¿Ã ≤À √°Ω¿¥œ¥Ÿ!", Color.red));
            return false;
        }

        inventory.Add(itemName);

        StartCoroutine(ShowNotice($"{itemName}¿ª(∏¶) »πµÊ«ﬂΩ¿¥œ¥Ÿ!", Color.white));

        Debug.Log($"¿˙¿Â øœ∑·: {itemName}");
        return true;
    }

    IEnumerator ShowNotice(string message, Color color)
    {

        noticeText.text = message;
        noticeText.color = color;

        noticeText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        noticeText.gameObject.SetActive(false);
    }
}