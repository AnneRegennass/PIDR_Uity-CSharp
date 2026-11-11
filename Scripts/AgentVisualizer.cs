using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Agent 
{
    public string name;
    public string type;
    public string status;
    public Location location;
    public int last_num;
}

[System.Serializable]
public class Location
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class Message
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

[System.Serializable]
public class Relationship
{
    public string source;
    public string target;
    public string type;
}

[System.Serializable]
public class Logs
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

[System.Serializable]
public class AgentData
{
    public List<Agent> agents;
    public List<Relationship> Relationship;
    public List<Message> messages;
    public List<Logs> logs;
    public int CurrentNum;
}

public class AgentVisualizer : MonoBehaviour
{
    private Dictionary<string, GameObject> agentObjects = new Dictionary<string, GameObject>();

    /// <summary>
    /// A chaque frame, repositionne le texte pour qu'il fasse face à la camera de l'utilisateur
    /// </summary>
    void Update()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        foreach (var agentObj in agentObjects.Values)
        {
            var textMesh = agentObj.GetComponentInChildren<TextMesh>();
            if (textMesh != null)
            {
                textMesh.transform.rotation = Quaternion.LookRotation(textMesh.transform.position - cam.transform.position);
            }
        }
    }
}