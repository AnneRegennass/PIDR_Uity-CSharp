using UnityEngine;
using UnityEngine.EventSystems;

public class DetectInputFieldClick : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Conteneur du clavier que l'on veut utiliser avec le champs")]
    public GameObject keyboardContainer;

    [Tooltip("CenterEyeAnchor de la camera")]
    public Transform playerHead;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        keyboardContainer.SetActive(true);
        Vector3 keyboardPosition = playerHead.position;

        keyboardPosition.y = keyboardPosition.y - 0.5f;
        keyboardPosition.x = keyboardPosition.x + (playerHead.forward.x * 0.3f);
        keyboardPosition.z = keyboardPosition.z + (playerHead.forward.z * 0.3f);

        keyboardContainer.transform.position =  keyboardPosition;
        keyboardContainer.transform.LookAt(playerHead);
        keyboardContainer.transform.Rotate(0, 180, 0);
    }
}
