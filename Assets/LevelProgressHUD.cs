using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressHUD : MonoBehaviour
{

    [SerializeField] private RectTransform playerImage;

    public Transform Follow;
    public Transform Origin;
    public Transform Goal;

    private float followStartX = 0f;
    private float followEndX = 142f;

    // Update is called once per frame
    void Update()
    {

        float maxLength = GetComponent<RectTransform>().rect.width;
        playerImage.anchoredPosition = new Vector2(Progress() * followEndX, playerImage.anchoredPosition.y);
    }

    public float Progress()
    {
        float goalX = Goal.position.x;
        float originX = Origin.position.x;
        float f = (Follow.transform.position.x - originX) / (goalX - originX);
        return Mathf.Clamp(f, 0f, 1f);
    }

}
