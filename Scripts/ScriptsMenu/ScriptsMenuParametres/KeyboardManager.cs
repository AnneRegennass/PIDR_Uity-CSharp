using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour
{
    [Header("Boutons Spéciaux du clavier")]
    [Tooltip("Conteneur du bouton Shift du clavier")]
    public Button shiftButton;

    [Tooltip("Conteneur du bouton Delete du clavier")]
    public Button deleteButton;

    [Header("Objets de la hiérarchie")]
    [Tooltip("Conteneur du bouton Fermer du clavier")]
    public Button fermerButton;

    [Tooltip("Objet inputField avec lequel on veut intéragir")]
    public TMP_InputField inputField;

    [Tooltip("Conteneur du clavier")]
    public GameObject keyboardContainer;

    public static KeyboardManager instance;
    private Image shiftButtonImage;
    private bool isShifted = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        deleteButton.onClick.AddListener(Delete);
        shiftButton.onClick.AddListener(Shifted);
        fermerButton.onClick.AddListener(Fermer);
        shiftButtonImage = shiftButton.gameObject.GetComponent<Image>();
    }

    /// <summary>
    /// Gère l'intéraction avec le bouton Shift (en change la couelur et modifie l'état du Shift)
    /// </summary>
    private void Shifted()
    {
        isShifted = !isShifted;

        if (isShifted)
        {
            shiftButtonImage.color = Color.yellow;
        }
        else
        {
            shiftButtonImage.color = Color.white;
        }
    }

    /// <summary>
    /// Supprime le dernier caractère de la chaîne qui a été entrée
    /// </summary>
    private void Delete()
    {
        int length = inputField.text.Length -1;
        inputField.text = inputField.text.Substring(0, length);
    }

    /// <summary>
    /// Ferme le clavier
    /// </summary>
    private void Fermer()
    {
        keyboardContainer.SetActive(false);
    }

}