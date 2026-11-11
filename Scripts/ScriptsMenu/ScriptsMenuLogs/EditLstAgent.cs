using UnityEngine;
using TMPro;

/// <summary>
/// ATTENTION : classe non testée
/// </summary>
public class EditLstAgent : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Conteneur du texte avec le nom de l'agent")]
    [SerializeField] private TMP_Text nomAgentText;

    [Tooltip("Conteneur avec le nombre total de logs de l'agent")]
    [SerializeField] private TMP_Text nbLogsText;

    public void Init(string nom, int nbLogs)
    {
        if (nomAgentText != null) 
            nomAgentText.text = nom;
            
        if (nbLogsText != null) 
            nbLogsText.text = nbLogs.ToString();
    }

    /// <summary>
    /// Met à jour le nombre de logs quand on en ajoute à un agent
    /// </summary>
    /// <param name="enPlus">Nomre de logs qu'on ajoute à l'agent</param>
    public void AddNumLog(int enPlus)
    {
        if(int.TryParse(nbLogsText.text, out int nb))
        {
            nb += enPlus;
            nbLogsText.text = nb.ToString();
        }
        else
        {
            Debug.LogError($"[EditLstAgent] : Conversion en int impossible");
        }
    }

    /// <summary>
    /// Met à jour le nombre de logs de l'angent quand on supprime des logs
    /// </summary>
    public void SpprNumLog()
    {
        if(int.TryParse(nbLogsText.text, out int nb))
        {
            nb -= 1;
            nbLogsText.text = nb.ToString();
        }
        else
        {
            Debug.LogError($"[EditLstAgent] : Conversion en int impossible");
        }
    }
}