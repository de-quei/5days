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

    // [중요] 변수 이름 craftingColorItems로 통일 확인!
    public Sprite[] craftingColorItems; // Inspector에서 꼭 연결되어 있어야 함

    public Sprite[] tooltipSprites;

    public CraftingRecipe[] craftingRecipes;

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

        for (int i = 0; i < blackItems.Length; i++)
        {
            GameObject slot = Instantiate(slotPrefab, craftingGrid);
            slot.name = "BlackSlot_" + blackItems[i].name;

            // 1. 이미지 설정
            Image imgComponent = slot.GetComponentInChildren<Image>();
            if (imgComponent != null)
            {
                imgComponent.sprite = blackItems[i];
            }

            // 2. 툴팁 연결
            if (i < tooltipSprites.Length)
            {
                SlotTooltipTrigger trigger = slot.AddComponent<SlotTooltipTrigger>();
                trigger.tooltipSprite = tooltipSprites[i];
            }

            // 3. 버튼 기능 심기
            Button btn = slot.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.interactable = false; // [중요] 처음엔 꺼둠!

            string targetName = blackItems[i].name;
            btn.onClick.AddListener(() => TryCraftItem(targetName));
        }
    }

    // [로그 추가됨] 버튼이 눌리면 이 로그가 떠야 함
    public void TryCraftItem(string resultName)
    {
        Debug.Log($">>> [클릭됨] 제작 시도: {resultName}");

        // 1. 레시피 찾기
        CraftingRecipe targetRecipe = new CraftingRecipe();
        bool recipeFound = false;

        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            if (recipe.resultName == resultName)
            {
                targetRecipe = recipe;
                recipeFound = true;
                break;
            }
        }

        if (!recipeFound)
        {
            Debug.LogError($">>> [에러] {resultName}에 대한 레시피를 찾을 수 없습니다!");
            return;
        }

        // 2. 재료 소모 
        foreach (Ingredient ing in targetRecipe.ingredients)
        {
            for (int i = 0; i < ing.amount; i++)
            {
                RemoveItemFromInventory(ing.itemName);
            }
        }

        // 3. 결과물 지급
        inventory.Add(resultName);
        AddSlotToInventoryGrid(resultName);

        StartCoroutine(ShowNotice($"{resultName} 제작 성공!", Color.green));
        Debug.Log($">>> [성공] {resultName} 제작 완료 및 인벤토리 추가됨");

        // 4. 상태 갱신
        CheckCraftableStatus();
    }

    void RemoveItemFromInventory(string itemName)
    {
        inventory.Remove(itemName);

        Transform targetSlot = inventoryGrid.Find("InvenSlot_" + itemName);
        if (targetSlot != null)
        {
            DestroyImmediate(targetSlot.gameObject);
        }
    }

    public bool SaveToInventory(string itemName)
    {
        if (inventory.Count >= maxCapacity)
        {
            StartCoroutine(ShowNotice("가방이 꽉 찼습니다!", Color.red));
            return false;
        }

        inventory.Add(itemName);
        AddSlotToInventoryGrid(itemName);
        StartCoroutine(ShowNotice($"{itemName}을(를) 획득했습니다!", Color.white));

        CheckCraftableStatus();

        return true;
    }

    // [여기가 수정됨!] 버튼을 켜고 끄는 로직 추가
    void CheckCraftableStatus()
    {
        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            bool canCraft = true;
            foreach (Ingredient ing in recipe.ingredients)
            {
                int myCount = GetItemCount(ing.itemName);

                if (myCount < ing.amount)
                {
                    canCraft = false;
                    break;
                }
            }

            Transform slotTransform = craftingGrid.Find("BlackSlot_" + recipe.resultName);
            if (slotTransform != null)
            {
                // 이미지 찾기 (자식에 있을 수 있으므로 InChildren 사용)
                Image slotImage = slotTransform.GetComponentInChildren<Image>();

                // [추가] 버튼 컴포넌트 찾기
                Button btn = slotTransform.GetComponent<Button>();

                if (slotImage != null)
                {
                    if (canCraft)
                    {
                        slotImage.sprite = GetColorSprite(recipe.resultName);

                        // [핵심] 제작 가능하면 버튼을 켠다!
                        if (btn != null) btn.interactable = true;
                    }
                    else
                    {
                        slotImage.sprite = GetBlackSprite(recipe.resultName);

                        // [핵심] 제작 불가능하면 버튼을 끈다!
                        if (btn != null) btn.interactable = false;
                    }
                }
            }
        }
    }

    int GetItemCount(string targetName)
    {
        int count = 0;
        foreach (string item in inventory)
        {
            if (item == targetName) count++;
        }
        return count;
    }

    Sprite GetColorSprite(string name)
    {
        foreach (Sprite s in craftingColorItems)
        {
            if (s.name == name) return s;
        }
        // 혹시 몰라 재료에서도 찾음
        foreach (Sprite s in itemSprites)
        {
            if (s.name == name) return s;
        }
        return null;
    }

    Sprite GetBlackSprite(string name)
    {
        foreach (Sprite s in blackItems)
        {
            if (s.name == name) return s;
        }
        return null;
    }

    void AddSlotToInventoryGrid(string itemName)
    {
        Sprite targetSprite = null;
        foreach (Sprite s in craftingColorItems) { if (s.name == itemName) { targetSprite = s; break; } }

        if (targetSprite == null)
        {
            foreach (Sprite s in itemSprites) { if (s.name == itemName) { targetSprite = s; break; } }
        }

        if (targetSprite == null) return;

        GameObject newSlot = Instantiate(slotPrefab, inventoryGrid);
        newSlot.name = "InvenSlot_" + itemName;

        Image img = newSlot.GetComponentInChildren<Image>();
        if (img != null)
        {
            img.sprite = targetSprite;
        }
    }

    public void OpenInventory()
    {
        if (inventoryOverlay != null)
        {
            inventoryOverlay.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void CloseInventory()
    {
        if (inventoryOverlay != null)
        {
            inventoryOverlay.SetActive(false);
        }
        Time.timeScale = 1f;
    }

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

[System.Serializable]
public struct Ingredient
{
    public string itemName;
    public int amount;
}

[System.Serializable]
public struct CraftingRecipe
{
    public string resultName;
    public Ingredient[] ingredients;
}