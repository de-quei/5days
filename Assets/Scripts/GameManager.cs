using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 
    public GameObject itemPrefab;

    public Sprite[] itemSprites;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(waitTime);

            int currentItems = FindObjectsOfType<ItemManager>().Length;

            if (currentItems < 2)
            {
                SpawnItem();
            }
            else
            {
                Debug.Log("아이템이 이미 2개라 생성을 건너뜁니다.");
            }
        }
    }

    void SpawnItem()
    {
    
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;


        float minX = -camWidth + 1.5f;
        float maxX = camWidth - 1.5f;
        float minY = -camHeight + 1.5f;
        float maxY = camHeight - 1.5f;

        Vector2 randomPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

        GameObject newItem = Instantiate(itemPrefab, randomPos, Quaternion.identity);

        SpriteRenderer sr = newItem.GetComponent<SpriteRenderer>();
        if (itemSprites.Length > 0)
        {
            sr.sprite = itemSprites[Random.Range(0, itemSprites.Length)];
        }
    }
}