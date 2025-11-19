using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    // 배경 스프라이트를 카메라 크기에 맞게 조절
    void Start()
    {
        Camera mainCamera = Camera.main;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null) return;

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        double worldScreenHeight = mainCamera.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 newScale = transform.localScale;
        newScale.x = (float)(worldScreenWidth / width);
        newScale.y = (float)(worldScreenHeight / height);

        transform.localScale = newScale;
    }

    
}
