using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomAligner : MonoBehaviour
{
    void Start()
    {
        Camera mainCamera = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float spriteHeight = sr.bounds.size.y * transform.localScale.y;

        float bottomEdge = -mainCamera.orthographicSize;

        Vector3 newPosition = new Vector3(0f, bottomEdge + spriteHeight / 2f, 0f);
        transform.position = newPosition;
    }
}
