using UnityEngine;
using static ScenarioViewer.EtatCarte;

public class MenuManager : MonoBehaviour
{
    [Header("Assignations")]
    [Tooltip("Objet contenant la barre avec les boutons du menu")]
    public GameObject menuCanvas;

    [Tooltip("Conteneur du clavier")]
    public GameObject keyboardContainer;

    [Tooltip("CenterEyeAnchor de la camera")]
    public Transform playerHead;

    [Header("Paramètres")]
    [Tooltip("Distance du panneau par rapport au regard de l'utilisateur")]
    public float distanceDevantJoueur = 0.8f; 

    [Header("Configuration des Menus")]
    [Tooltip("Objet contenant la barre des boutons de sélection de menus")]
    [SerializeField] private Transform barreBoutons;

    [Tooltip("Décalage en mètres : X (gauche/droite), Y (haut/bas), Z (avant/arrière)")]
    [SerializeField] private Vector3 decalageLocal = new Vector3(0f, 0.8f, -0.02f);

    public static MenuManager Instance;
    private float enBasDuJoueur = 0.4f;
    public static GameObject canvasActuel = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    /// <summary>
    /// A chaque frame, vérifie si la touche de menu est pressée
    /// </summary>
    void Update()
    {        
        if (OVRInput.GetDown(OVRInput.Button.Start,OVRInput.Controller.LTouch) && ScenarioViewer.etatCarteCourant == Baked)
        {
            ToggleMenu();
        } 
    }

    /// <summary>
    /// Lorsque la touche menu est pressé, ferme ou ouvre le menu et le panneau associé.
    /// </summary>
    public void ToggleMenu()
    {
        // Inverse l'état d'activation du menu
        bool estActif = !menuCanvas.activeSelf;
        menuCanvas.SetActive(estActif);
        
        if(canvasActuel != null)
        {
            canvasActuel.SetActive(estActif);
        }
        
        if(!estActif && keyboardContainer.activeSelf)
        {
            keyboardContainer.SetActive(false);
        }
    
        if (estActif)
        {
            Vector3 buttonBarPosition = playerHead.position;

            buttonBarPosition.y = buttonBarPosition.y - enBasDuJoueur;

            menuCanvas.transform.position = buttonBarPosition + (playerHead.forward * distanceDevantJoueur);
            menuCanvas.transform.LookAt(playerHead);
            menuCanvas.transform.Rotate(0, 180, 0);

            if(canvasActuel != null)
            {
                canvasActuel.transform.rotation = barreBoutons.rotation;
                canvasActuel.transform.position = barreBoutons.TransformPoint(decalageLocal);
            }
        }
    }

    public void ChangerDeCanvas(GameObject menuAOuvrir)
    {
        if(menuAOuvrir == null) return;
        
        if(canvasActuel != null)
        {
            canvasActuel.SetActive(false);
        }

        menuAOuvrir.transform.rotation = barreBoutons.rotation;
        menuAOuvrir.transform.position = barreBoutons.TransformPoint(decalageLocal);

        menuAOuvrir.SetActive(true);
        canvasActuel = menuAOuvrir;
    }
}