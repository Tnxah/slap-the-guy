using RockInMyShoe.Global.Eventing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsHints : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;

    [SerializeField] private GameObject mobileHints;
    [SerializeField] private GameObject pcHints;

    [SerializeField]
    private Dictionary<InputDevice, Sprite> turnDictionary;

    [SerializeField]
    private GameObject alwaysOnScreenPc;

    public void OnInputDevice(InputDevice device)
    {
        pcHints.SetActive(false);
        mobileHints.SetActive(false);
        alwaysOnScreenPc.SetActive(false);

        switch (device)
        {
            case Touchscreen:
                mobileHints.SetActive(true);

                break;
            case Keyboard:

            case Mouse:
                pcHints.SetActive(true);
#if UNITY_WEBGL || UNITY_EDITOR
                alwaysOnScreenPc.SetActive(true);
#endif
                break;
            case Gamepad:
                pcHints.SetActive(true);

                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<InputDevice>(OnInputDevice, true);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<InputDevice>(OnInputDevice);
    }
}
