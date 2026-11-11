using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapLoader : MonoBehaviour
{
    [Header("Hiérarchie")]
    [Tooltip("Objet contenant l'origine de la scène virtuelle")]
    public Transform ParentScene;

    [Tooltip("Fichier .Map contenu dans les agents mobiles")]
    public TextAsset fichierMap;

    [Header("Préfabriqués utilisés")]
    [Tooltip("préfabriqué des points pour les lignes de la carte")]
    public GameObject pointPrefab;

    [Tooltip("Préfabriqué des points objectifs de la carte")]
    public GameObject pointGoal;

    [Tooltip("Matériel pour les lignes (forbidden lines)")]
    public Material lineMaterial; 

//_________________
    public static MapLoader Instance { get; private set; }
    public static Dictionary<string, Location> pointsMap = new Dictionary<string, Location>();
    private GameObject container;
    string contenu;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        if(fichierMap != null)
        {
            contenu = fichierMap.text;
        }
    }

    /// <summary>
    /// Crée un ligne rouge sur la scène Unity entre les deux points passés en paramètre.
    /// </summary>
    /// <param name="start">Coordonnées d'une extrémité de la ligne</param>
    /// <param name="end">Coordonénes de l'autre extrémité de la ligne</param>
    void CreateLine(Vector3 start, Vector3 end)
    {
        if(ParentScene == null)
        {
            Debug.LogError("ParentScene n'est pas assigné dans l'inspecteur !");
            return;
        }
        
        GameObject lineObj = new GameObject("MapLine");
        lineObj.transform.SetParent(ParentScene.transform, false);

        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localRotation = Quaternion.identity;
        lineObj.transform.localScale = Vector3.one;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 3;
        lr.useWorldSpace = false;

        lr.startColor = Color.red;
        lr.endColor = Color.red;

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    /// <summary>
    /// Crée une zone rouge près du sol de la scène Unity, avec comme deux sommets opposés (x1, 0, y1) et (x2, 0, y2)
    /// </summary>
    /// <param name="x1">Absisse du premier point</param>
    /// <param name="y1">Profondeur du premier point</param>
    /// <param name="x2">Absisse du second point</param>
    /// <param name="y2">Profondeur du second point</param>
    void CreateForbiddenZone(float x1, float y1, float x2, float y2)
    {
        if(ParentScene == null)
        {
            Debug.LogError("ParentScene n'est pas assigné dans l'inspecteur !");
            return;
        }

        GameObject zoneObj = new GameObject("ForbiddenZone");
        zoneObj.transform.SetParent(ParentScene.transform, false);
        zoneObj.transform.localPosition = Vector3.zero;
        zoneObj.transform.localRotation = Quaternion.identity;
        zoneObj.transform.localScale = Vector3.one;

        LineRenderer lr = zoneObj.AddComponent<LineRenderer>();
        
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 0.08f; 
        lr.useWorldSpace = false;
        
        // On définit 4 points pour le rectangle + 1 pour refermer la boucle
        lr.positionCount = 5; 
        lr.loop = true; // Ferme automatiquement la ligne entre le dernier et le premier point

        Vector3 p1 = new Vector3(x1, 0, y1) * RestAgentJson.ScaleFactor;
        Vector3 p2 = new Vector3(x2, 0, y1) * RestAgentJson.ScaleFactor;
        Vector3 p3 = new Vector3(x2, 0, y2) * RestAgentJson.ScaleFactor;
        Vector3 p4 = new Vector3(x1, 0, y2) * RestAgentJson.ScaleFactor;

        lr.SetPosition(0, p1);
        lr.SetPosition(1, p2);
        lr.SetPosition(2, p3);
        lr.SetPosition(3, p4);
        lr.SetPosition(4, p1);
    }

    /// <summary>
    /// Construit les éléments de la carte
    /// </summary>
    /// <param name="showMapItems">Bool</param>
    public void BakePoints(bool showMapItems)
    {
        if(ParentScene == null)
        {
            Debug.LogError("[MapLoader] ParentScene n'est pas assigné dans l'inspecteur !");
            return;
        }

        container = new GameObject("ImportedPoints_" + System.DateTime.Now.ToString("HHmmss"));

        container.transform.SetParent(ParentScene.transform, false);

        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;

        string[] lines = contenu.Split(new string[] {"\r\n", "\n"}, System.StringSplitOptions.None);

        foreach (string line in lines)
        {
            if (line.StartsWith("Cairn: "))
            {
                string cleanedLine = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", " ");
                cleanedLine = cleanedLine.Replace("\"", "");
                string[] parts = cleanedLine.Split(' ');
                switch (parts[1])
                {
                    case "ForbiddenLine" :
                        if(showMapItems)
                        {
                            try {
                                float x1 = float.Parse(parts[parts.Length - 4], CultureInfo.InvariantCulture);
                                float y1 = float.Parse(parts[parts.Length - 3], CultureInfo.InvariantCulture);
                                float x2 = float.Parse(parts[parts.Length - 2], CultureInfo.InvariantCulture);
                                float y2 = float.Parse(parts[parts.Length - 1], CultureInfo.InvariantCulture);

                                Vector3 startPoint = new Vector3(x1, 0, y1) * RestAgentJson.ScaleFactor;
                                Vector3 endPoint = new Vector3(x2, 0, y2) * RestAgentJson.ScaleFactor;

                                CreateLine(startPoint, endPoint);
                            }
                            catch { /* Gestion d'erreur */ }
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case "ForbiddenArea" :    
                        if(showMapItems)
                        {
                            try {
                                float x1 = float.Parse(parts[parts.Length - 4], CultureInfo.InvariantCulture);
                                float y1 = float.Parse(parts[parts.Length - 3], CultureInfo.InvariantCulture);
                                float x2 = float.Parse(parts[parts.Length - 2], CultureInfo.InvariantCulture);
                                float y2 = float.Parse(parts[parts.Length - 1], CultureInfo.InvariantCulture);

                                CreateForbiddenZone(x1, y1, x2, y2);
                            }
                            catch { /* Ligne mal formatée ignorée */ }
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case "Goal":
                        try {
                                float x = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                float y = float.Parse(parts[4], CultureInfo.InvariantCulture);
                                string goalName = parts[7];

                                if(!showMapItems)
                                {
                                    pointsMap[goalName] = new Location { x = x, y = y, z = z };
                                    break;
                                }

                                Vector3 positionLocale = new Vector3(x, y, z) * RestAgentJson.ScaleFactor;

                                GameObject dot = Instantiate(pointGoal, container.transform);
                                dot.transform.localPosition = positionLocale;
                                dot.transform.localRotation = Quaternion.identity;

                                dot.name = goalName;
                                TMP_Text textComponent = dot.GetComponentInChildren<TMP_Text>();

                                if (textComponent != null)
                                {
                                    textComponent.text = goalName;
                                }
                                
                                #if UNITY_EDITOR
                                // Indique à Unity que cet objet fait partie de la scène sauvegardable
                                Undo.RegisterCreatedObjectUndo(dot, "Create Point");
                                #endif
                            }
                            catch { /* Ignorer les erreurs de parsing */ }
                            break;

                    case "GoalWithHeading":
                        try {
                                float x = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                float y = float.Parse(parts[4], CultureInfo.InvariantCulture);

                                Vector3 positionLocale = new Vector3(x, y, z) * RestAgentJson.ScaleFactor;
                                GameObject dot = Instantiate(pointGoal, container.transform);
                                dot.transform.localPosition = positionLocale;
                                dot.transform.localRotation = Quaternion.identity;

                                string goalName = parts[7];
                                dot.name = goalName;
                                TMP_Text textComponent = dot.GetComponentInChildren<TMP_Text>();
                                if (textComponent != null)
                                {
                                    textComponent.text = goalName;
                                    break;
                                }
                                
                                #if UNITY_EDITOR
                                // Indique à Unity que cet objet fait partie de la scène sauvegardable
                                Undo.RegisterCreatedObjectUndo(dot, "Create Point");
                                #endif
                            }
                            catch { /* Ignorer les erreurs de parsing */ }
                            break;
                    case "DockLynx":
                        try {
                                float x = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                float y = float.Parse(parts[4], CultureInfo.InvariantCulture);

                                Vector3 positionLocale = new Vector3(x, y, z) * RestAgentJson.ScaleFactor;
                                GameObject dot = Instantiate(pointGoal, container.transform);
                                dot.transform.localPosition = positionLocale;
                                dot.transform.localRotation = Quaternion.identity;

                                string goalName = parts[9] + " " + parts[10];
                                dot.name = goalName;
                                TMP_Text textComponent = dot.GetComponentInChildren<TMP_Text>();
                                if (textComponent != null)
                                {
                                    textComponent.text = goalName;
                                }
                                
                                #if UNITY_EDITOR
                                // Indique à Unity que cet objet fait partie de la scène sauvegardable
                                Undo.RegisterCreatedObjectUndo(dot, "Create Point");
                                #endif 
                            }
                            catch { /* Ignorer les erreurs de parsing */ }
                            break;
                    default :
                        try {
                            float x = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[4], CultureInfo.InvariantCulture);

                            if (showMapItems)
                            {
                                Vector3 positionLocale = new Vector3(x, y, z) * RestAgentJson.ScaleFactor;

                                GameObject dot = Instantiate(pointPrefab, container.transform);
                                dot.transform.localPosition = positionLocale;
                                dot.transform.localRotation = Quaternion.identity;
                                
                                #if UNITY_EDITOR
                                Undo.RegisterCreatedObjectUndo(dot, "Create Point");
                                #endif
                            }
                        }
                        catch { /* Ignorer les erreurs de parsing */ }
                        break;
                }
            }
        }
    }
}