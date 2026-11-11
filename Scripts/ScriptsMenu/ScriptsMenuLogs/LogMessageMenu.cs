using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LogMessageMenu : MonoBehaviour
{
    [Tooltip("Parent du menu liste agents")]
    public Transform MenuAgentParent;

    [Tooltip("Préfabriqué pour afficher l'agent dans la menu")]
    public GameObject AgentLstPrefab;

    [Tooltip("Préfabriqué pour afficher les logs de l'agent")]
    public GameObject LogLstPrefab;
    private Dictionary<string, GameObject> LogsData;

    void MajLstAGLogs(Dictionary<string, List<Logs>> DataRecues)
    {
        foreach(string AgName in DataRecues.Keys)
        {
            if (LogsData.Keys.Contains(AgName))
            {
                int enPlus = MajLstLogs(DataRecues[AgName], LogsData[AgName]);
                LogsData[AgName].GetComponent<EditLstAgent>().AddNumLog(enPlus);
            }
            else
            {
                //On ajoute l'agent
                GameObject newAg = Instantiate(AgentLstPrefab, MenuAgentParent);

                EditLstAgent controller = newAg.GetComponent<EditLstAgent>();

                if(controller != null)
                {
                    controller.Init(AgName, 0);
                }
                else
                {
                    Debug.LogError($"[LogMessageMenu] : Script EditLstAgent manquant");
                }
                LogsData[AgName] = newAg;

                int enPlus = MajLstLogs(DataRecues[AgName], LogsData[AgName]);
                LogsData[AgName].GetComponent<EditLstAgent>().AddNumLog(enPlus);
            }
        } 
    }

    /// <summary>
    /// Ajoute les logs présent dans logLst au conteneur Container donné en paramètre
    /// </summary>
    /// <param name="logLst">Liste des logs à ajouter</param>
    /// <param name="Container">Ligne correspondant à l'agent correspondant aux logs</param>
    /// <returns></returns>
    int MajLstLogs(List<Logs> logLst, GameObject Container)
    {
        int enPlus = 0;
        foreach(Logs log in logLst)
        {
            if(log != null)
            {
                //On ajoute un log à la liste de l'agent donné en paramètre
                GameObject newLog = Instantiate(LogLstPrefab, Container.transform);

                EditLstLogs controller = newLog.GetComponent<EditLstLogs>();

                if(controller != null)
                {
                    controller.Init(log);
                }
                else
                {
                    Debug.LogError($"[LogMessageMenu] : Script EditLstLogs manquant");
                }
                enPlus += 1;
            }
        }
        return enPlus;
    }
}