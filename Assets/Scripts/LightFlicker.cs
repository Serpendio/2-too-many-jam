using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    Light2D light2D;
    [SerializeField] float flickerSpeed = 1f;
    [SerializeField] float secondaryFlickerSpeed = 1f;
    [SerializeField] float flickerIntensity = 1f;
    [SerializeField] float secondaryFlickerIntensity = 1f;
    float offset = 0f, secondaryOffset = 0f;
    float initialIntensity;
    void Awake()
    {
        light2D = GetComponent<Light2D>();
        initialIntensity = light2D.intensity;
        offset = Random.Range(0f, 2f * Mathf.PI);
        secondaryOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        light2D.intensity = initialIntensity + 
            Mathf.Sin(Time.time * flickerSpeed + offset) * flickerIntensity + 
            Mathf.Sin(Time.time * secondaryFlickerSpeed + secondaryOffset) * secondaryFlickerIntensity;
    }
}
