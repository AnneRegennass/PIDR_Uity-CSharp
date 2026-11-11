using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// ATTENTION : classe non testée
/// </summary>
public class EditLstLogs : MonoBehaviour
{
    [Tooltip("Conteneur du texte du log réduit")]
    [SerializeField] private TMP_Text SmallLog;

    [Tooltip("Conteneur du texte de log détaillé")]
    [SerializeField] private TMP_Text CompleteLog;

    [Tooltip("Pictogramme du niveau/du type du log")]
    [SerializeField] private Image imageLevel;

    public void Init(Logs logAAjouter)
    {
        if(logAAjouter != null)
        {
            if(logAAjouter.level != null)
            {
                Sprite spriteCharge = Resources.Load<Sprite>("Levels/" + logAAjouter.level);
                if(spriteCharge != null)
                {
                    imageLevel.sprite = spriteCharge;
                }
                else
                {
                    Debug.LogError($"[EditLstLogs] : Pictogramme non trouvé");
                }
            }
            else
            {
                Debug.LogError($"[EditLstLogs] : valeur level du Log non trouvé : {logAAjouter.level}");
            }
            SmallLog.text = logAAjouter.timeStamp + " | " + logAAjouter.marker;
            CompleteLog.text = logAAjouter.level + " | in " + logAAjouter.file + " > " + logAAjouter.method + 
                                " | Le message [" + logAAjouter.message + "] à été répété " + logAAjouter.lineNumber + " fois.";
        }
    }

}