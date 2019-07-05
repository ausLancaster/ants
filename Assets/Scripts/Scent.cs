using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scent : MonoBehaviour
{
    private const int MAX_SCENT = 1000;
    private const int START_FADE_TIME = 1000;

    public int amount { get;  private set; }
    private float fadeLeft;
    private SpriteRenderer spriteRenderer;
    private Color color;
    private Color transparent;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
        transparent = new Color(color.r, color.g, color.b, 0);
    }

    public void Initialize(float ratio)
    {
        amount = (int)(MAX_SCENT * ratio);
        fadeLeft = START_FADE_TIME;
    }

    public void AddRatio(float ratio)
    {
        print((amount, ratio));
        amount += (int)(MAX_SCENT * ratio);
        if (amount > MAX_SCENT) amount = MAX_SCENT;
        fadeLeft = START_FADE_TIME;
    }

    void Update()
    {
        if (fadeLeft > 0)
        {
            fadeLeft--;
        }
        else if (amount > 0)
        {
            amount--;
        }
        else if (amount == 0)
        {
            Destroy(gameObject);
        }
        spriteRenderer.color = Color.Lerp(transparent, color, ((float)amount)/MAX_SCENT);
    }
}
