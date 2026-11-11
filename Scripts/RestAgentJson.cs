using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Classe permettant de faire les appels au serveur Rest et de traiter les données reçues pour les exploiter
/// </summary>
public class RestAgentJson : MonoBehaviour
{
    [Header("Paramètre de connexion au serveur REST")]
    [Tooltip("Intervalle en seconde entre chaque appel au serveur")]
    public float fetchInterval = 1f;

    [Tooltip("URL actuelle du serveur")]
    public static string url = "http://10.206.172.224:8080/data";

    [Tooltip("Clé permettant à se connecter au serveur pour récupérer les logs")]
    public string Key = "key_abc123xyz";

    public static float ScaleFactor = 0.001f;
    public static AgentData data;

    void Start()
    {
        StartCoroutine(FetchAgentDataRepeatedly(fetchInterval));
    }

    /// <summary>
    /// Déclenche un appel à la fonction FetchAgentDataXithUnityWebRequest tout les interval secondes.
    /// </summary>
    /// <param name="interval">Temps en seconde</param>
    /// <returns>void</returns>
    IEnumerator FetchAgentDataRepeatedly(float interval)
    {
        while (true)
        {
            yield return FetchAgentDataWithUnityWebRequest();
            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Fait une requête au serveur url, déséréalise les données reçues, et appel LoadScenario avec les données reçues.
    /// </summary>
    /// <returns>void</returns>
    IEnumerator FetchAgentDataWithUnityWebRequest()
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("[RestAgentJson] : L'URL est vide ou null !");
            yield break; 
        }

        using(UnityWebRequest request = UnityWebRequest.Get(url))
        {    
            request.SetRequestHeader("Authorization", $"Bearer {Key}");

            request.SendWebRequest(); 

            float maxAttente =3.0f;
            float tempsDebut = Time.time;

            while (!request.isDone)
            {
                if (Time.time - tempsDebut > maxAttente)
                {
                    Debug.LogError($"[RestAgentJson] Le serveur ne répond pas.");
                    request.Abort();
                    break;
                }
                yield return null; 
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(" [Erreur Réseau] : " + request.error);
                Debug.LogError(" [Code HTTP] : " + request.responseCode);
                yield break;
            }

            string json = request.downloadHandler.text;

            TraceViewerResponse response = JsonUtility.FromJson<TraceViewerResponse>(json);
            
            data = ConvertToVisualizerData(response);

            if (data == null)
            {
                Debug.LogError("Conversion des données échouée : data est null.");
                yield break;
            }
            else 
            {
                WorldAligner.Instance.dataRecues();
            }
        }
    }

    /// <summary>
    /// Converties les données déséréalisées reçues en AgentData pour permettre leur exploitation
    /// </summary>
    /// <param name="response">Données reçues via le serveur Rest déséréalisées</param>
    /// <returns>Renvoie les données reçues dans un format exploitables en AgentData</returns>
    private static AgentData ConvertToVisualizerData(TraceViewerResponse response)
    {
        AgentData data = new AgentData
        {
            agents = new List<Agent>(),
            Relationship = new List<Relationship>(),
            messages = new List<Message>(),
            logs = new List<Logs>(),
            CurrentNum = 0
        };

        if (response == null)
        {
            return data;
        }

        if(response.currentNum != 0)
        {
            data.CurrentNum = response.currentNum;
        }

        if (response.agents != null)
        {
            foreach (TraceViewerAgentDto agent in response.agents)
            {
                TraceViewerLocationDto location = agent.location ?? new TraceViewerLocationDto();
                data.agents.Add(new Agent
                {
                    name = agent.name,
                    type = agent.types != null && agent.types.Count > 0 ? agent.types[0] : string.Empty,
                    //ATTENTION : dans unity on a (x: abscisse, y: ordonnées (hauteur), z : profondeur) alors que dans les traces on a : (x, z, y)
                    //Donc on inverse pour avoir les bonnes coordonnées
                    location = new Location
                    {
                        x = location.x * ScaleFactor,
                        y = location.z * ScaleFactor,
                        z = location.y * ScaleFactor
                    },

                    status = !string.IsNullOrEmpty(agent.last_status) 
                        ? agent.last_status
                        : null,

                    last_num = agent.last_num
                });
            }
        }

        if (response.relationships != null)
        {
            foreach (TraceViewerRelationshipDto rel in response.relationships)
            {
                data.Relationship.Add(new Relationship
                {
                    source = rel.from,
                    target = rel.to,
                    type = rel.relationshipType
                });
            }
        }
        if(response.messages != null)
        {
            foreach(TraceViewerMessageDto mes in response.messages)
            {
                data.messages.Add(new Message
                {
                    performative = mes.performative,
                    sender = mes.sender ?? null,
                    receiver = mes.receiver ?? null,
                    language = mes.language,
                    ontology = mes.ontology,
                    protocol = mes.protocol,
                    content = mes.content ?? null,
                    conversationId = mes.conversationId ?? null,
                    replyWith = mes.replyWith ?? null,
                    inReplyTo = mes.inReplyTo ?? null
                });
            }
        }

        if(response.logs != null)
        {
            foreach(TraceViewerLogsDto log in response.logs)
            {
                data.logs.Add(new Logs
                {
                    file = log.file,
                    method = log.method ?? null,
                    level = log.level,
                    marker = log.marker ?? null,
                    name = log.name,
                    message = log.message ?? null,
                    lineNumber = log.lineNumber,
                    timeStamp = log.timeStamp
                });
            }
        }
        return data;
    }

    [Serializable]
    private class TraceViewerResponse
    {
        public List<TraceViewerAgentDto> agents;
        public List<TraceViewerRelationshipDto> relationships;
        public List<TraceViewerMessageDto> messages;
        public List<TraceViewerLogsDto> logs;
        public int currentNum;
    }

    [Serializable]
    private class TraceViewerAgentDto
    {
        public string name;
        public List<string> types;
        public List<string> status;
        public string last_status;
        public TraceViewerLocationDto location;
        public int last_num;
    }

    [Serializable]
    private class TraceViewerLocationDto
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    private class TraceViewerRelationshipDto
    {
        public string from;
        public string to;
        public string relationshipType;
    }

    [Serializable]
    private class TraceViewerMessageDto
    {
        public int performative;
        public string sender;
        public string receiver;
        public string language;
        public string ontology;
        public string protocol;
        public string content;
        public string conversationId;
        public string replyWith;
        public string inReplyTo;
    }

    [Serializable]
    private class TraceViewerLogsDto
    {
        public string file;
        public string method;
        public string level;
        public string marker;
        public string name;
        public string message;
        public string lineNumber;
        public string timeStamp;
    }
}

