using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Cette classe permet la mise en place du passthough/de la réalité mixte
/// </summary>
public class ActiverPassthrough : MonoBehaviour
{
    private ARCameraManager arCameraManager;

    void Start()
    {
        // On cherche le composant ARCameraManager sur la caméra
        arCameraManager = GetComponent<ARCameraManager>();

        if (arCameraManager == null)
        {
            arCameraManager = GetComponentInChildren<ARCameraManager>();
        }

        if (arCameraManager != null)
        {
            // On force l'activation du fond de caméra AR (le Passthrough)
            arCameraManager.enabled = true;
        }
        else
        {
            Debug.LogError("ARCameraManager introuvable. Assure-toi de l'avoir ajouté sur ta Main Camera.");
        }
    }
}