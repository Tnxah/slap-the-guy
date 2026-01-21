using RockInMyShoe.Global.Eventing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class LastUserDeviceTracker : MonoBehaviour
{
    private InputDevice lastDevice;

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        if (lastDevice == device)
            return;

        lastDevice = device;
        EventBus.Publish(device);
    }

    void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }
}
