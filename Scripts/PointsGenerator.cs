using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public class PointsGenerator : MonoBehaviour
{
    [Tooltip("Objet contenant toute la scène 3D virtuelle")]
    public Transform ParentScene;

    [Tooltip("Fichier .Map contenu dans les agents mobiles")]
    public TextAsset fichierMap;

    public static PointsGenerator Instance { get; private set; }
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
    /// Renvoie le dictionnaire donné en paramètre rempli des points trouvés dans le fichier .map
    /// </summary>
    /// <param name="pointsMap">Dictionnaire des nom des points et de leurs coordonnées</param>
    /// <returns></returns>
    public Dictionary<string, Location> BakePoints(Dictionary<string, Location> pointsMap)
    {
        if (ParentScene == null)
        {
            Debug.LogError("ParentScene n'est pas assigné ! Impossible de calculer les coordonnées réelles.");
            return pointsMap;
        }

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
                    case "Goal":
                        try {
                                float x = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                float y = float.Parse(parts[4], CultureInfo.InvariantCulture);
                                string goalName = parts[7];
                                Vector3 positionLocale = new Vector3(x, y, z) * RestAgentJson.ScaleFactor;
                                Vector3 positionRelle = ParentScene.TransformPoint(positionLocale);

                                pointsMap[goalName] = new Location { x = positionRelle.x, y = positionRelle.y, z = positionRelle.z }; 
                            }
                            catch { /* Ignorer les erreurs de parsing */ }
                            break;
                    case "GoalWithHeading":
                        try {
                                float x = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                float y = float.Parse(parts[4], CultureInfo.InvariantCulture);

                                string goalName = parts[7];

                                Vector3 positionLocale = new Vector3(x, y, z) * RestAgentJson.ScaleFactor;
                                Vector3 positionRelle = ParentScene.TransformPoint(positionLocale);

                                pointsMap[goalName] = new Location { x = positionRelle.x, y = positionRelle.y, z = positionRelle.z };
                            }
                            catch { /* Ignorer les erreurs de parsing */ }
                            break;
                    case "DockLynx":
                        try {
                                float x = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                float y = float.Parse(parts[4], CultureInfo.InvariantCulture);

                                string goalName = parts[9] + " " + parts[10];

                                Vector3 positionLocale = new Vector3(x, y, z) * RestAgentJson.ScaleFactor;
                                Vector3 positionRelle = ParentScene.TransformPoint(positionLocale);

                                pointsMap[goalName] = new Location { x = positionRelle.x, y = positionRelle.y, z = positionRelle.z };

                            }
                            catch { /* Ignorer les erreurs de parsing */ }
                            break;
                    default :
                        break;
                }
            }
        }
        return pointsMap;
    }
}