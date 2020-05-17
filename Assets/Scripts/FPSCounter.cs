using UnityEngine;

public class FPSCounter : MonoBehaviour {

    public int FPS { get; private set; }
    private int step;

    private void Start()
    {
        step = 0;
    }

    void Update()
    {
        step++;
        if (step % 5 == 0)
        {
            FPS = (int)(1f / Time.unscaledDeltaTime);
        }
    }
}