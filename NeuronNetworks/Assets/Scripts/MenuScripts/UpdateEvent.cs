using System;
using UnityEngine;

public class UpdateEvent : MonoBehaviour
{
    public static event Action<float> onUpdate;
    public static event Action<float> onUpdateScaledTime;
    public static event Action onUpdatePerSecond;
    public static event Action<float> onUpdateLowTickRate;

    private float remainder = 0.0f;
    private float reminderToLowTick = 0.0f;
    private float framesUntillLowTick = 10;
    private float currentFramesUntillLowTick;

    private void Awake()
    {
        currentFramesUntillLowTick = framesUntillLowTick;
    }
    void Update()
    {
        onUpdate?.Invoke(Time.unscaledDeltaTime);
        onUpdateScaledTime?.Invoke(Time.deltaTime);
        remainder += Time.unscaledDeltaTime;
        if (remainder >= 1)
        {
            onUpdatePerSecond?.Invoke();
            remainder -= 1;
        }

        reminderToLowTick += Time.unscaledDeltaTime;
        if (currentFramesUntillLowTick > 0)
        {
            currentFramesUntillLowTick--;
        }
        else
        {
            currentFramesUntillLowTick = framesUntillLowTick;
            onUpdateLowTickRate?.Invoke(reminderToLowTick);
            reminderToLowTick = 0.0f;
        }
    }
}

