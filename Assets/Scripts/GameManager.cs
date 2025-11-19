using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Sprite[] itemSprites;
    public float avoidRadius = 1.5f;

    void Start()
    {
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

        float minX = -camWidth + 1.0f;
        float maxX = camWidth - 1.0f;
        float minY = -camHeight + 1.0f;
        float maxY = camHeight - 1.0f;

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
            sr.sprite = itemSprites[Random.Range(0, itemSprites.Length)];
        }
    }
}