using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoutonDeroulant : MonoBehaviour
{
    [Tooltip("Texte du bouton")]
    public TextMeshProUGUI keyLabel;

    [Tooltip("Contenu du menu déroulant")]
    public GameObject Component;

    private string closed = ">";
    private string opened = "v";
    private Button thisButton;
    private bool StatusOpened;
    private Image ArrowButtonImage;

    private void Start()
    {
        keyLabel.text = closed;
        StatusOpened = false;
        Component.SetActive(false);
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(ChangeStatus);
        ArrowButtonImage = thisButton.gameObject.GetComponent<Image>();
    }

    /// <summary>
    /// Permet d'ouvrir/de fermer le menu déroulant
    /// </summary>
    private void ChangeStatus()
    {
        if (StatusOpened)
        {
            StatusOpened = false;
            keyLabel.text = closed;
            Component.SetActive(false);
            ArrowButtonImage.color = Color.white;
        }
        else
        {
            StatusOpened = true;
            keyLabel.text = opened;
            Component.SetActive(true);
            ArrowButtonImage.color = Color.grey;
        }
    }
}