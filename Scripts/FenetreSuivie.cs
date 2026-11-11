using UnityEngine;

public class FenetreSuivie : MonoBehaviour
{
    [Header("Cible à suivre")]
    [Tooltip("CenterEyeAnchor de la camera")]
    public Transform cameraDuJoueur; 

    [Header("Paramètres de distance")]
    [Tooltip("Distance de la fenêtre par rapport à la position de l'utilisateur")]
    public float distanceDuJoueur = 1.5f;

    [Tooltip("Vitesse de suivi de la fenêtre")]
    public float vitesseSuivi = 5.0f;

    void Start()
    {
        if (cameraDuJoueur == null)
        {
            GameObject centerEye = GameObject.Find("CenterEyeAnchor");
            if (centerEye != null)
            {
                cameraDuJoueur = centerEye.transform;
            }
            else
            {
                cameraDuJoueur = Camera.main.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (cameraDuJoueur == null) return;

        Vector3 positionCible = cameraDuJoueur.position + cameraDuJoueur.forward * distanceDuJoueur;
        transform.position = Vector3.Lerp(transform.position, positionCible, Time.deltaTime * vitesseSuivi);

        Vector3 directionRegard = transform.position - cameraDuJoueur.position;
        directionRegard.y = 0; 
        
        if (directionRegard != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionRegard);
        }
    }

    public void MasquerFenetre()
    {
        gameObject.SetActive(false);
    }

    public void AfficherFenetre()
    {
        gameObject.SetActive(true);
    }

}