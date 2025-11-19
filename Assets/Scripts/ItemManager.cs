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

        yield return new WaitForSeconds(13f);

        if (!isClicked)
        {
            Debug.Log("시간 초과! 아이템이 사라집니다.");
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        if (isClicked) return;

        isClicked = true;
        Debug.Log(">>> 획득 성공! 인벤토리에 저장 <<<");

        // 저장 로직 필요

        Destroy(gameObject);
    }
}