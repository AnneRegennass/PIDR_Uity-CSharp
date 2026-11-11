using UnityEngine;

/// <summary>
/// Classe permettant d'orienter les textes en direction de la caméra de l'utilisateur
/// </summary>
public class LookAtCameraYonly : MonoBehaviour
{
    public Camera cameraToLookAt;
    private Transform cameraTransform;
    
    void Start() 
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate() 
    {
        Vector3 targetPosition = cameraTransform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition); 
        transform.Rotate(0,180,0);
    }
}