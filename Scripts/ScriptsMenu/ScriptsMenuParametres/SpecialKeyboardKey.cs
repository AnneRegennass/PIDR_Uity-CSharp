using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Paramètrages pour les touches du clavier pré-remplies (touches "http://", "/data", etc)
/// </summary>
public class SpecialKeyboardKey : MonoBehaviour
{
    [Tooltip("Caractère(s) que l'on veut taper (peut être une chaîne de caractères)")]
    public string character;

    [Tooltip("Conteneur du texte de la touche")]
    public TextMeshProUGUI keyLabel;

    private Button thisKey;

    private void Start()
    {
        thisKey = GetComponent<Button>();
        thisKey.onClick.AddListener(TypeKey);
        character = keyLabel.text; //On récupère le caractère que l'on veut ajouter en prenant la composante texte du bouton (le texte sur le bouton sera celui tapé)
    }

    /// <summary>
    /// Ajoute la chaîne de caractères visée à l'URL en cours d'écriture
    /// </summary>
    private void TypeKey()
    {
        KeyboardManager.instance.inputField.text += character;
    }
    
}