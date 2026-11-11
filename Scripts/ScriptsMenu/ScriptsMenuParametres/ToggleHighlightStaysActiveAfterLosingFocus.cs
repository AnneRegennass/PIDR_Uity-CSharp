using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Permet à la touche Shift de garder une certaine couleur quand elle est pressée
/// </summary>
public class ToggleHightlightStaysActiveAfterLosingFocus : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image imageToKeepFocusActive;

    private void Reset()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(toggle.isOn);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if(imageToKeepFocusActive == null) return;

        imageToKeepFocusActive.color = toggle.isOn ? toggle.colors.highlightedColor : Color.clear;
    }
}