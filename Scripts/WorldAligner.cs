using UnityEngine;

public class WorldAligner : MonoBehaviour
{
    public enum EtatAlignement
    {
        EnAttentePointA,
        EnAttentePointB,
        Termine
    }

    [Header("Statut de l'alignement")]
    public static EtatAlignement etatCourant = EtatAlignement.EnAttentePointA;

    [Header("Coordonnées des point de repère dans la scène)")]
    [Tooltip("Coordonnées du point de repère A (dock le plus à droite)")]
    public Vector3 refPointA_fichier = new Vector3(0.815f, 1f, 1.126f); //On considère pour l'instant qu'on positionne le point au dessus du dock le plus à droite et le plus à gauche

    [Tooltip("Coordonnées du point de repère B (dock le plus à gauche)")]
    public Vector3 refPointB_fichier = new Vector3(0.822f, 1f, -0.856f);

    [Header("Références aux objets de la scène")]
    [Tooltip("RightHandAnchor de la hiérarchie")]
    public Transform rightControllerTransform; 
    
    [Tooltip("Objet qui continent l'origine de la scène virtuelle")]
    public Transform sceneParent;
    
    [Header("Fenêtres d'instructions de parametrages des points")]
    [Tooltip("Menu à afficher lors de la recherche du point A")]
    public FenetreSuivie menuInstructionPtA;
    
    [Tooltip("Menu à afficher lors de la recherche du point B")]
    public FenetreSuivie menuInstructionPtB;

//__________________________
    private Vector3 refPointA_reel;
    private Vector3 refPointB_reel;
    public static WorldAligner Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if(etatCourant != EtatAlignement.Termine && OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            CollectPoints();
        }
    }

    /// <summary>
    /// Appelée lorsque les données du serveur sont reçues. Vérifie si la carte est positionnée et si on peut donc afficher les données.
    /// </summary>
    public void dataRecues()
    {
        if(etatCourant == EtatAlignement.Termine)
        {
            ScenarioViewer.Instance.CheckLoadScenario();
        }
        else return;
    }

    /// <summary>
    /// Collecte les points de repère A et B en fonction de l'état courant de l'alignement. Lance ensuite le calcul de l'origine du repère virtuel.
    /// </summary>
    public void CollectPoints()
    {
        switch (etatCourant)
        {
            case EtatAlignement.EnAttentePointA:
                refPointA_reel = rightControllerTransform.position;
                etatCourant = EtatAlignement.EnAttentePointB;
                menuInstructionPtA.MasquerFenetre();
                menuInstructionPtB.AfficherFenetre();
                break;

            case EtatAlignement.EnAttentePointB:
                refPointB_reel = rightControllerTransform.position;
                CalculOrigin();
                menuInstructionPtB.MasquerFenetre();
                break;
        }
    }

    /// <summary>
    /// Calcule l'origine du repère virtuel en fonction des deux points collectés. Lance ensuite le scénario.
    /// </summary>
    public void CalculOrigin()
    {
        Vector3 vecteurFichier = refPointB_fichier - refPointA_fichier;
        Vector3 vecteurReel = refPointB_reel - refPointA_reel;

        vecteurFichier.y = 0;
        vecteurReel.y = 0;

        Quaternion rotationCalculée = Quaternion.FromToRotation(vecteurFichier, vecteurReel);
        sceneParent.rotation = rotationCalculée;
        sceneParent.position = refPointA_reel - rotationCalculée * refPointA_fichier;
        
        etatCourant = EtatAlignement.Termine;
        ScenarioViewer.Instance.CheckLoadScenarioBakePoint();
    }
}