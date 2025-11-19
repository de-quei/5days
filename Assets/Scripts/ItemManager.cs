using System.Collections;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private bool isClicked = false;

    void Start()
    {
        StartCoroutine(LifeCycle());
    }

    IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(5f);

        if (!isClicked)
        {
            Debug.Log("시간 초과! 아이템이 사라집니다.");
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        if (isClicked) return;

        GameManager gm = FindObjectOfType<GameManager>();

        if (gm != null)
        {
            string myName = GetComponent<SpriteRenderer>().sprite.name;

            bool isSuccess = gm.SaveToInventory(myName);

            if (isSuccess)
            {
                isClicked = true;
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("가방이 꽉 차서 아이템을 주울 수 없습니다.");
            }
        }
    }
}