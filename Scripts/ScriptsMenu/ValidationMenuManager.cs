using UnityEngine;
using UnityEngine.UI;

public class ValidationMenuManager : MonoBehaviour
{
    [Header("Boutons")]
    public Button ValiderButton;
    public Button FermerButton;
    public Button LancerButton;

    [Header("Objets de la hiérarchie")]
    [Tooltip("Objet contenant le menu de validation")]
    public GameObject ValidationMenuContainer;

    [Tooltip("Objet contenant tous les objets de la carte")]
    public Transform ObjectsContainer;

    [Tooltip("CenterEyeAnchor de la camera")]
    public Transform playerHead;

    [Tooltip("Premier panneau de la phase de configuration de la carte")]
    public GameObject menuPtA;

    private void Awake()
    {
        ValidationMenuContainer.SetActive(false);
        ValiderButton.onClick.AddListener(Valider);
        FermerButton.onClick.AddListener(Fermer);
        LancerButton.onClick.AddListener(Lancer);
    }

    /// <summary>
    /// Ferme le panneau de validation sans lancer la phase de configuration
    /// </summary>
    private void Fermer()
    {
        ValidationMenuContainer.SetActive(false);
    }

    /// <summary>
    /// Ferme le panneau de validation et lance la phase de configuration (supprime les éléments de la carte et lance la phase de configuration)
    /// </summary>
    private void Valider()
    {
        ValidationMenuContainer.SetActive(false);
        MenuManager.canvasActuel.SetActive(false);
        MenuManager.Instance.menuCanvas.SetActive(false);
        ScenarioViewer.etatCarteCourant = ScenarioViewer.EtatCarte.NotBaked;

        foreach(Transform child in ObjectsContainer)
        {
            Destroy(child.gameObject);
        }
        WorldAligner.etatCourant = WorldAligner.EtatAlignement.EnAttentePointA;
        menuPtA.SetActive(true);
    }

    /// <summary>
    /// Ouvre le panneau de validation
    /// </summary>
    private void Lancer()
    {
        ValidationMenuContainer.SetActive(true);
        Vector3 ValidationMenuContainerPos = playerHead.position;
        ValidationMenuContainerPos.y = ValidationMenuContainerPos.y - 0.5f;
        ValidationMenuContainerPos.x = ValidationMenuContainerPos.x + (playerHead.forward.x * 0.3f);
        ValidationMenuContainerPos.z = ValidationMenuContainerPos.z + (playerHead.forward.z * 0.3f);

        ValidationMenuContainer.transform.position =  ValidationMenuContainerPos;
        ValidationMenuContainer.transform.LookAt(playerHead);
        ValidationMenuContainer.transform.Rotate(0, 180, 0);
    }
}