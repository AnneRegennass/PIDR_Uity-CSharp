using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvasManager : MonoBehaviour
{
    [Tooltip("Texte contenant l'URL")]
    [SerializeField] private AffichageURL textUrl;

    [Tooltip("Conteneur du clavier")]
    public GameObject keyboardContainer;

    [Tooltip("Conteneur du bouton Valider")]
    public Button validerButton;

    [Tooltip("InputField avec lequel on intéragit")]
    public TMP_InputField inputField;

    private void Awake()
    {
        validerButton.onClick.AddListener(Valider);
    }

    /// <summary>
    /// Lorsque le bouton "Valider" est pressé, vide l'inputField et sauvegarde la nouvelle URL
    /// Ferme aussi le clavier
    /// </summary>
    private void Valider()
    {
        string newUrl = inputField.text;
        inputField.text = "";
        textUrl.MajUrl(newUrl);
        keyboardContainer.SetActive(false);
    }
}