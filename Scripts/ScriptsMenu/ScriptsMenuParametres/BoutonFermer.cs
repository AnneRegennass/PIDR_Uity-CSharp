using UnityEngine;
using UnityEngine.UI;

public class BoutonFermer : MonoBehaviour
{
    private Button thisButton;

    [Tooltip("Ligne de Log à fermer")]
    public GameObject Log;

    [Tooltip("Objet contenant le détail du log à fermer")]
    public GameObject DetailLog;

    private void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(Fermer);
    }

    /// <summary>
    /// Ferme la fenêtre associée
    /// </summary>
    private void Fermer()
    {
        Destroy(Log);
        Destroy(DetailLog);
    }
}