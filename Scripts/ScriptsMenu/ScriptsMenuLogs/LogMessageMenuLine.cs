using UnityEngine;
using System.Collections.Generic;

public class LogMessageMenuLine : MonoBehaviour
{
    [Tooltip("Préfabriqué des logs d'un agent")]
    public GameObject LogLstPrefab;

    int MajLstLogs(List<Logs> logLst, Transform Container)
    {
        int enPlus = 0;
        foreach(Logs log in logLst)
        {
            if(log != null)
            {
                //On ajoute un log à la liste de l'agent donné en paramètre
                GameObject newLog = Instantiate(LogLstPrefab, Container);

                EditLstLogs controller = newLog.GetComponent<EditLstLogs>();

                if(controller != null)
                {
                    controller.Init(log);
                }
                else
                {
                    Debug.LogError($"[LogMessageMenuLine] : Script EditLstLogs manquant");
                }
                enPlus += 1;
            }
        }
        return enPlus;
    }

}