using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AgMessageAnimation : MonoBehaviour
{
    [Header("Paramètres d'affichage")]
    [Tooltip("Coefficiet de déplacement des boules des messages agents")]
    public float CoefSpeed = 5f;

    [Tooltip("Hauteur maximale du message lorsqu'il se déplace")]
    public float hauteurArc = 2.0f;

    [Tooltip("Nombre de répétition de l'envoie des boules des messages agents")]
    public int maxRep = 1;

    [Tooltip("Temps d'apparition des messages télémétriques")]
    public float lifeTimeTelemetryBubble = 10f;

    public static AgMessageAnimation Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Déclenche l'affichage d'un message d'un agent de couleur couleur entre les points startPos et endPos.
    /// </summary>
    /// <param name="startPos">Coordnnées de départ du message</param>
    /// <param name="endPos">Coordnnées d'arrivée du message</param>
    /// <param name="couleur">Couleur d'affichage du message</param>
    public void PrintAgMessage(Location startPos, Location endPos, Color couleur)
    {
        StartCoroutine(AnimateAgMessage(new Vector3(startPos.x, startPos.y, startPos.z), new Vector3(endPos.x, endPos.y, endPos.z), couleur));
    }

    /// <summary>
    /// Coroutone pour gérer le déplacement fluide entre les deux coorodnnées
    /// </summary>
    /// <param name="startPos"Coordonnées de départ du message></param>
    /// <param name="endPos">Coordonnées d'arrivée du message</param>
    /// <param name="couleur">Couleur d'affichage du message</param>
    /// <returns></returns>
    System.Collections.IEnumerator AnimateAgMessage(Vector3 startPos, Vector3 endPos, Color couleur)
    {
        // Création d'une petite sphère temporaire pour simuler le message qui circule
        GameObject msgObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        msgObj.transform.localScale = Vector3.one * 1.0f; 

        Renderer r = msgObj.GetComponent<Renderer>();
        if (r != null)
        {
            r.material.color = couleur;  
        } 
        
        float journeyLength = Vector3.Distance(startPos, endPos);
        if (journeyLength <= 0f) journeyLength = 0.01f;
        int currentRepeats = 0;
        float speed = CoefSpeed;

        while (currentRepeats < maxRep)
        {
            float startTime = Time.time;
            float fractionOfJourney = 0f;

            while (fractionOfJourney < 1.0f)
            {
                float distCovered = (Time.time - startTime) * speed;
                fractionOfJourney = distCovered / journeyLength;

                Vector3 positionActuelle = Vector3.Lerp(startPos, endPos, fractionOfJourney);

                float arc = Mathf.Sin(fractionOfJourney * Mathf.PI) * hauteurArc;

                positionActuelle.y += arc;

                msgObj.transform.position = positionActuelle;

                if (Camera.main != null)
                    msgObj.transform.LookAt(Camera.main.transform);

                yield return null;
            }
            currentRepeats++;
        }
        Destroy(msgObj);
    }

    /// <summary>
    /// Permet d'afficher des bulles de messages données dans le dictionnaire data
    /// </summary>
    /// <param name="data"></param>
    public void PrintTelemetryMessage(Dictionary<Agent, string> data)
    {
        List<Agent> lstNameAgents = data.Keys.ToList();

        foreach(Agent AgName in lstNameAgents)
        {
            string message = data[AgName];
            Vector3 pos = new(AgName.location.x, AgName.location.y, AgName.location.z);
                
            GameObject bubble =new GameObject("telemetry_container_" + message);
            if (bubble == null) return;

            bubble.name = "telemetry_bubble_" + message;
            bubble.transform.position = pos + Vector3.up * 1.4f;        

            // Création d'un GameObject enfant dédié uniquement au texte
            GameObject textObject = new GameObject("Text_Label");
            textObject.transform.SetParent(bubble.transform); // On l'attache à la bulle
            textObject.transform.localPosition = Vector3.zero;
            textObject.transform.localScale = Vector3.one;

            textObject.AddComponent<LookAtCameraYonly>();

            TextMesh tm = textObject.AddComponent<TextMesh>();
            if (tm != null)
            {
                tm.text = message;
                tm.characterSize = 0.12f;
                tm.fontSize = 50; 
                tm.alignment = TextAlignment.Center;
                tm.anchor = TextAnchor.MiddleCenter; 
                tm.color = Color.white;
            }
            StartCoroutine(FadeTelemetryBubble(bubble, tm, lifeTimeTelemetryBubble));
        }
    }

    /// <summary>
    /// Permet de faire disparaître l'objet 3D bubble dans un fondu pendant la durée lifetime.
    /// </summary>
    /// <param name="bubble">Objet 3D</param>
    /// <param name="textMesh">Texte rattaché à l'objet 3D donné en paramètre</param>
    /// <param name="lifetime">Durée de vie de l'objet 3D donné en paramètre</param>
    /// <returns></returns>
    System.Collections.IEnumerator FadeTelemetryBubble(GameObject bubble, TextMesh textMesh, float lifetime)
    {
        float elapsed = 0f;
        Vector3 startPos = bubble.transform.position;
        Renderer renderer = bubble.GetComponent<Renderer>();
        Color bubbleColor = renderer != null ? renderer.material.color : Color.white;
        Color textColor = textMesh != null ? textMesh.color : Color.white;
        

        while (elapsed < lifetime)
        {
            if (bubble == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lifetime);

            if (renderer != null)
            {
                Color c = bubbleColor;
                c.a = Mathf.Lerp(bubbleColor.a, 0f, t);
                renderer.material.color = c;
            }
            if (textMesh != null)
            {
                Color c = textColor;
                c.a = Mathf.Lerp(textColor.a, 0f, t);
                textMesh.color = c;
            }
            bubble.transform.position = startPos + Vector3.up * (t * 0.15f);
            yield return null;
        }
        if (bubble != null)
        {
            Destroy(bubble);
        }
    }
}