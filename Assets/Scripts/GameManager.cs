using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public GameObject itemPrefab;
    public Sprite[] itemSprites; 
    [Range(0.1f, 5.0f)] public float avoidRadius = 1.2f;

    [Header("Inventory Data")]
    public List<string> inventory = new List<string>();
    public int maxCapacity = 30;

    [Header("UI Settings")]
    public Text noticeText;
    public GameObject inventoryOverlay;

    [Header("UI Grids")]
    public Transform craftingGrid;  
    public Transform inventoryGrid; 

    public GameObject slotPrefab;   
    public Sprite[] blackItems;     

    void Start()
    {
        if (noticeText != null) noticeText.gameObject.SetActive(false);
        if (inventoryOverlay != null) inventoryOverlay.SetActive(false);

        InitCraftingUI();

        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(SpawnRoutine());
    }

    void InitCraftingUI()
    {
        foreach (Transform child in craftingGrid) { Destroy(child.gameObject); }

        foreach (Sprite itemImg in blackItems)
        {
            GameObject slot = Instantiate(slotPrefab, craftingGrid);
            Image imgComponent = slot.GetComponent<Image>();
            if (imgComponent != null)
            {
                imgComponent.sprite = itemImg;
            }
            slot.name = "BlackSlot_" + itemImg.name;
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

        AddSlotToInventoryGrid(itemName);

        StartCoroutine(ShowNotice($"{itemName}¿ª(∏¶) »πµÊ«ﬂΩ¿¥œ¥Ÿ!", Color.white));
        return true;
    }

    void AddSlotToInventoryGrid(string itemName)
    {
        Sprite targetSprite = null;
        foreach (Sprite s in itemSprites)
        {
            if (s.name == itemName)
            {
                targetSprite = s;
                break;
            }
        }

        if (targetSprite == null) return;

        GameObject newSlot = Instantiate(slotPrefab, inventoryGrid);
        newSlot.name = "InvenSlot_" + itemName;

        Image img = newSlot.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = targetSprite;
        }
    }

    public void OpenInventory() { inventoryOverlay.SetActive(true); }
    public void CloseInventory() { inventoryOverlay.SetActive(false); }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(2f, 3f);
            yield return new WaitForSeconds(waitTime);
            int currentItems = FindObjectsOfType<ItemManager>().Length;
            if (currentItems < 2) SpawnItem();
        }
    }

    void SpawnItem()
    {
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        float padding = 1.0f;
        float minX = -camWidth + padding; float maxX = camWidth - padding;
        float minY = -camHeight + padding; float maxY = camHeight - padding;

        Vector2 randomPos;
        int attemptCount = 0;
        do
        {
            float x = Random.Range(minX, maxX); float y = Random.Range(minY, maxY);
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

    IEnumerator ShowNotice(string message, Color color)
    {
        noticeText.text = message;
        noticeText.color = color;
        noticeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        noticeText.gameObject.SetActive(false);
    }
}