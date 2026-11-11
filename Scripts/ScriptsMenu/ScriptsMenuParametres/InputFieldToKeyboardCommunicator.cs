using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Classe permettant de déclencher les interaction avec l'inputField
/// </summary>
public class InputFieldToKeyboardCommunicator : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        KeyboardManager.instance.inputField = GetComponent<TMP_InputField>();
    }
}