using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardKey : MonoBehaviour
{
    [Tooltip("Caractère concerné (caractère de la touche) (pas besoin de remplir)")]
    public string character;

    [Tooltip("Caractère de la touche en majuscule (pas besoin de remplir)")]
    public string shiftCharacter;

    [Tooltip("Conteneur du texte de la touche")]
    public TextMeshProUGUI keyLabel;

    private Button thisKey;
    private bool isShifted = false;

    private void Start()
    {
        KeyboardManager.instance.shiftButton.onClick.AddListener(HandleShift);
        thisKey = GetComponent<Button>();
        thisKey.onClick.AddListener(TypeKey);
        character = keyLabel.text;
        shiftCharacter = keyLabel.text.ToUpper();
        string numbers = "0123456789";

        if (numbers.Contains(keyLabel.text))
        {
            shiftCharacter = GetShiftCharacter();
        }
    }

    /// <summary>
    /// Permet d'obtenir le caractère "Shifté" associé
    /// </summary>
    /// <returns>Le charactère en majuscule de la touche (ou le caractère correspondant quand la touche "Shift" est enclenchée)</returns>
    private string GetShiftCharacter()
    {
        switch (keyLabel.text)
        {
            case "1":
                return ".";
            case "2":
                return "-";
            case "3":
                return "_";
            case "4":
                return ":";
            case "5":
                return "?";
            case "6":
                return "&";
            case "7":
                return "=";
            case "8":
                return "%";
            case "9":
                return "/";
            case "0":
                return "~";
            default :
                break;
        }
        return string.Empty;
    }

    /// <summary>
    /// Passe les caractères du clavier en "Shift", ou à l'inverse, en non "Shifté".
    /// </summary>
    private void HandleShift()
    {
        isShifted = !isShifted;

        if (isShifted)
        {
            keyLabel.text = shiftCharacter;
        }
        else
        {
            keyLabel.text = character;
        }
    }

    /// <summary>
    /// Renvoie le caractère à taper entre le caractère classique ou le caractère correspondant quand la touche "Shift" est enclenchée.
    /// </summary>
    private void TypeKey()
    {
        if (isShifted)
        {
            KeyboardManager.instance.inputField.text += shiftCharacter;
        }
        else
        {
            KeyboardManager.instance.inputField.text += character;
        }
    }
    
}