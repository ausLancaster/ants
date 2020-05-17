using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FPSCounter))]
public class FPSDisplay : MonoBehaviour
{
    private Text fpsLabel;
    private FPSCounter fpsCounter;

    private void Start()
    {
        fpsLabel = GetComponent<Text>();
        fpsCounter = GetComponent<FPSCounter>();
    }

    private void Update()
    {
        fpsLabel.text = "FPS: " + fpsCounter.FPS;
    }
}