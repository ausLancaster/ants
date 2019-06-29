using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scent : MonoBehaviour
{
    private const int MAX_SCENT = 300;

    public int amount { get;  private set; }
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
        print(amount);
    }

    public void AddRatio(float ratio)
    {
        amount += (int)(MAX_SCENT * ratio);
        if (amount > MAX_SCENT) amount = MAX_SCENT;
    }

    void Update()
    {
        amount--;
        if (amount == 0)
        {
            Destroy(gameObject);
        }
        spriteRenderer.color = Color.Lerp(transparent, color, ((float)amount)/MAX_SCENT);
    }
}
