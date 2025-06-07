using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    [SerializeField] private ParallaxCameraDelegate onCameraTranslate;

    private float oldPosition;

    public ParallaxCameraDelegate OnCameraTranslate
    {
        get { return onCameraTranslate; }
        set { onCameraTranslate = value; }
    }

    private void Start()
    {
        oldPosition = transform.position.x;
    }

    private void FixedUpdate()
    {
        if (transform.position.x != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                float deltaX = transform.position.x - oldPosition;
                onCameraTranslate(deltaX);
            }

            oldPosition = transform.position.x;
        }
    }
}
