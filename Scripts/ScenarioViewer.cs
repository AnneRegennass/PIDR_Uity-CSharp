using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Unity.VisualScripting;

/// <summary>
/// Classe chargée de jouer sur Unity des événements donnés
/// </summary>
public class ScenarioViewer : MonoBehaviour
{
    public enum EtatCarte
    {
        NotBaked,
        Baked
    }

    [Tooltip("Etat de la carte (affichée = Baked, ou non = NotBaked)")]
    public static EtatCarte etatCarteCourant = EtatCarte.NotBaked;

    [Tooltip("Parent de la scène (pour définition de l'origine)")]
    public Transform ParentScene;

    [Header("Paramètres d'affichage")]
    [Tooltip("Préfabriqué des agents")]
    [SerializeField] private GameObject agentPrefab;
    Material lineMaterial;

    [Tooltip("Permet d'indiquer si on affiche ou non les éléments de la carte autre que les points objetcifs (forbidden zone, line, etc)")]
    [SerializeField]private bool showMapItems = true;

    [Tooltip("Permet d'indiquer si on affiche ou non les points objectif de la carte")]
    [SerializeField]private bool showGoalItems = true;

    [Header("Paramètres d'affichage des relations")]
    [Tooltip("Couleur des relations entre agents (par défaut bleu à 50% de transparence)")]
    [SerializeField] private UnityEngine.Color relationColor = new UnityEngine.Color(0f, 0f, 1f, 0.5f); // Blanc à 50% de transparence par défaut

    public static Dictionary<string, GameObject> LstAgents = new Dictionary<string, GameObject>();
    public static Dictionary<string, List<Logs>> LstAgLogs = new Dictionary<string, List<Logs>>();
    private Dictionary<string, GameObject> relationshipObjects = new Dictionary<string, GameObject>();
    public static ScenarioViewer Instance { get; private set; }
    public static Dictionary<Agent, Agent> LinkDeviceRessource = new Dictionary<Agent, Agent>(); //Pour savoir à quel agent appartient un agent logistique
    private static Dictionary<string, Location> pointsMap = new Dictionary<string, Location>();
    private Dictionary<string, int> lastNumStatus = new Dictionary<string, int>();
    private Dictionary<string, (int, bool)> IsPrinted = new Dictionary<string, (int, bool)>();
    private int CurrentEvent = -1;
    private Dictionary<string, bool> LstAgSeen = new Dictionary<string, bool>();

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
    /// Initialisation de la scène (récupération des points et de leurs coordnées)
    /// </summary>
    void Start()
    {
        lineMaterial = new(Shader.Find("Sprites/Default"));
    }

    /// <summary>
    /// Vérifie si on peut générer les points de la carte et lance leur création le cas échéant.
    /// </summary>
    public void CheckLoadScenarioBakePoint()
    {
        if (etatCarteCourant == EtatCarte.NotBaked)
        {
            if (!showGoalItems)
            {
                PointsGenerator.Instance.BakePoints(pointsMap);
            }
            else
            {
                MapLoader.Instance.BakePoints(showMapItems);
                if (showMapItems)
                {
                    pointsMap = InitializePointsMap(pointsMap);
                }
                else
                {
                    pointsMap = MapLoader.pointsMap;
                }
            }
            etatCarteCourant = EtatCarte.Baked;
        }
        return;
    }

    /// <summary>
    /// Vérifie si on peut lancer l'affichage des donénes reçues, et le lance le cas échéant
    /// </summary>
    public void CheckLoadScenario()
    {
        if(etatCarteCourant == EtatCarte.Baked)
        {
            if (RestAgentJson.data != null)
            {
                LoadScenario(RestAgentJson.data);
            }
            else
            {
                Debug.LogError("[ScenarioViewer] Les données du scénario sont nulles. Impossible de charger le scénario.");
            }
        }

        else
        {
            Debug.LogWarning("[ScenarioViewer] La carte n'est pas encore prête. Veuillez d'abord générer la carte avant de charger le scénario.");
        }
    }

    /// <summary>
    /// Joue les données reçues dans Unity (position des agents, des relations et des messages)
    /// </summary>
    /// <param name="AgData">Données reçues du serveur Rest</param>
    public void LoadScenario(AgentData AgData)
    {
        if (AgData == null)
        {
            Debug.LogError("Aucun scénario à charger : les données sont vides.");
            return;
        }
        else
        {
            if(AgData.CurrentNum != CurrentEvent)
            {
                CurrentEvent = AgData.CurrentNum;

                List<string> keysToReset = new List<string>(LstAgSeen.Keys);
                foreach(string ag in keysToReset)
                {
                    LstAgSeen[ag] = false;
                }

                if (AgData.agents != null)
                {
                    
                    Dictionary<string, Agent> filtrageParNom = new Dictionary<string, Agent>();

                    foreach(var agent in AgData.agents)
                    {
                        if(agent is Agent aAgent && aAgent != null)
                        {
                            LstAgSeen[agent.name] = true;

                            string realAgName = aAgent.name.Split("@")[0];

                            if (LstAgents.TryGetValue(realAgName, out GameObject agObj) && agObj != null)
                            {
                                Renderer rend = agObj.GetComponentInChildren<Renderer>();
                                if (rend != null)
                                {
                                    Color rougeMort = new Color(125f / 255f, 4f / 255f, 4f / 255f);

                                    if (rend.material.color == rougeMort)
                                    {
                                        rend.material.color = Color.white;
                                    }
                                }
                            }

                            bool agentExist = LstAgents != null && LstAgents.ContainsKey(realAgName);

                            bool positionRecueEstNulle = aAgent.location == null || 
                                                (Mathf.Approximately(aAgent.location.x, 0f) && 
                                                Mathf.Approximately(aAgent.location.y, 0f) && 
                                                Mathf.Approximately(aAgent.location.z, 0f));

                            if (aAgent.location == null)
                            {
                                aAgent.location = new Location();
                            }

                            if(positionRecueEstNulle)
                            {
                                if (agentExist)
                                {
                                        UnityEngine.Vector3 currentpos = LstAgents[realAgName].transform.localPosition;
                                        agent.location = new Location
                                        {
                                            x=currentpos.x,
                                            y=currentpos.y,
                                            z=currentpos.z   
                                        };
                                }
                                else
                                {

                                    if(realAgName == "viewer")
                                    {
                                        agent.location.x = 10;
                                        agent.location.y = 3;
                                        agent.location.z = 10;
                                    }
                                    else
                                    {    
                                        if(aAgent.type == "ProductLauncher" || aAgent.type == "productlauncher")
                                        {
                                            agent.location.x = 15;
                                            agent.location.y = 3;
                                            agent.location.z = 15;
                                        }
                                        else
                                        {
                                            System.Random rand = new System.Random();

                                            agent.location.x = rand.Next(-25, 75);
                                            agent.location.y = 0;
                                            agent.location.z = rand.Next(-25, 75);   
                                        }
                                    }
                                }
                            }
                            if (agentExist)
                            {
                                GameObject existingAgent = LstAgents[realAgName];
                                UnityEngine.Vector3 nouvelleCible = new UnityEngine.Vector3(agent.location.x, agent.location.y, agent.location.z);
                                if (existingAgent.TryGetComponent<AgentEntity>(out AgentEntity scriptMouvement))
                                {
                                    scriptMouvement.targetPosition = ParentScene.TransformPoint(nouvelleCible);
                                }
                                else
                                {
                                    existingAgent.transform.localPosition = nouvelleCible;
                                }
                            }
                            else
                            {
                                //L'agent n'existe pas, on le crée
                                UnityEngine.Vector3 position = new UnityEngine.Vector3(aAgent.location.x, aAgent.location.y, aAgent.location.z);
                                GameObject obj = Instantiate(agentPrefab, ParentScene);
                                obj.transform.localPosition = position;
                                obj.transform.localRotation = UnityEngine.Quaternion.identity;
                                obj.name = realAgName;

                                if (obj.TryGetComponent<AgentEntity>(out AgentEntity scriptMouvement))
                                {
                                    scriptMouvement.targetPosition = obj.transform.position;
                                }

                                if (LstAgents != null) {
                                    LstAgents[realAgName] = obj;
                                }
                                if(LstAgLogs != null)
                                {
                                    LstAgLogs[realAgName] = new List<Logs>();
                                }
                            }

                            if(!string.IsNullOrEmpty(aAgent.status))
                            {
                                
                                if (!lastNumStatus.ContainsKey(aAgent.name) || aAgent.last_num > lastNumStatus[aAgent.name])
                                {
                                    filtrageParNom[aAgent.name] = aAgent;
                                    
                                    // On mémorise l'étape à laquelle ce statut a été traité
                                    lastNumStatus[aAgent.name] = aAgent.last_num;

                                    IsPrinted[aAgent.name] = (aAgent.last_num, false);
                                }
                            }
                        }
                    }

                    //On regarde si il y a des agents qui n'ont pas été traité
                    foreach(string ag in LstAgSeen.Keys)
                    {
                        if (!LstAgSeen[ag])
                        {
                            string deadAgName = ag.Split('@')[0];
                            if (LstAgents != null && LstAgents.TryGetValue(deadAgName, out GameObject deadAgentObj) && deadAgentObj != null)                                    
                            {
                                Renderer rend = deadAgentObj.GetComponentInChildren<Renderer>();
                                
                                if (rend != null)
                                {
                                    rend.material.color = new Color(125f/255f, 4f/255f, 4f/255f);     
                                }
                                else
                                {
                                    Debug.LogWarning($"[ScenarioViewer] : Agent mort {ag} trouvé, mais aucun Renderer détecté.");
                                }
                            } 
                        }
                    }

                    Dictionary<Agent, string> ToPrintStatus = new Dictionary<Agent, string>();

                    foreach(var kvp in filtrageParNom)
                    {
                        Agent agentUnique = kvp.Value;
                        
                        if(IsPrinted.ContainsKey(agentUnique.name) && !IsPrinted[agentUnique.name].Item2)
                        {
                            ToPrintStatus[agentUnique] = agentUnique.status;

                            var currentvalue = IsPrinted[agentUnique.name];
                            IsPrinted[agentUnique.name] = (currentvalue.Item1, true);
                        }
                    } 

                    if(ToPrintStatus.Count > 0)
                    {
                        AgMessageAnimation.Instance.PrintTelemetryMessage(ToPrintStatus);
                    }
                }

                if(AgData.Relationship != null)
                {
                    HashSet<string> activeRelationships = new HashSet<string>();
                    HashSet<string> agentsInActiveRelationships = new HashSet<string>();

                    foreach (var rel in AgData.Relationship)
                    {
                        string realSource = rel.source.Split('@')[0];
                        string realTarget = rel.target.Split('@')[0];

                        bool sourceAlive = LstAgSeen.ContainsKey(rel.source) && LstAgSeen[rel.source];
                        bool targetAlive = LstAgSeen.ContainsKey(rel.target) && LstAgSeen[rel.target];

                        if (sourceAlive && targetAlive && LstAgents.ContainsKey(realSource) && LstAgents.ContainsKey(realTarget))
                        {
                            string key = $"{rel.source}-{rel.target}";
                            activeRelationships.Add(key);// Liste de relation active on s'en sert pour savoir si on doit supprimer une relation
                            agentsInActiveRelationships.Add(realSource); // On ajoute les agents source et cible à la liste des agents dans des relations actives
                            agentsInActiveRelationships.Add(realTarget); // pour pouvoir les colorier en rouge si ils ne sont pas actifs

                            if (relationshipObjects.ContainsKey(key))
                            {
                                // Mettre à jour la position de la ligne si nécessaire
                                GameObject lineObj = relationshipObjects[key];
                                LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                                lr.SetPosition(0, LstAgents[rel.source].transform.position);
                                lr.SetPosition(1, LstAgents[rel.target].transform.position);
                            }

                            else // Si la relation n'existe pas, la créer
                            {
                                GameObject lineObj = new GameObject($"Line_{key}");
                                lineObj.transform.SetParent(ParentScene, false);

                                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                                lr.material = lineMaterial; //paramètre de la ligne
                                lr.startWidth = 1f;
                                lr.endWidth = 1f;
                                lr.positionCount = 2;
                                lr.startColor = relationColor;
                                lr.endColor = relationColor;
                                lr.SetPosition(0, LstAgents[rel.source].transform.position);
                                lr.SetPosition(1, LstAgents[rel.target].transform.position);

                                relationshipObjects[key] = lineObj;
                            }
                        }
                    }
                    // Supprimer les relations obsolètes
                    List<string> keysToRemove = new List<string>();
                    foreach (var key in relationshipObjects.Keys)
                    {
                        if (!activeRelationships.Contains(key))
                        {
                            Destroy(relationshipObjects[key]);
                            keysToRemove.Add(key);
                        }
                    }

                    foreach (var key in keysToRemove)
                    {
                        relationshipObjects.Remove(key); 
                    }

                    foreach (var agent in AgData.agents)
                    {
                        string realAgName = agent.name.Split('@')[0];
                        if (LstAgents.TryGetValue(realAgName, out GameObject agObj) && agObj != null)
                        {
                            Renderer renderer = agObj.GetComponentInChildren<Renderer>();
                            if (renderer != null)
                            {
                                if (!agentsInActiveRelationships.Contains(realAgName))
                                {
                                    renderer.material.color = UnityEngine.Color.grey; // Vivant mais pas de relation
                                    var textMesh = agObj.GetComponentInChildren<TextMesh>();
                                    if (textMesh != null) textMesh.text = "";
                                }
                                else
                                {
                                    renderer.material.color = UnityEngine.Color.white; // Vivant et en relation
                                }
                            }
                        }
                    }
                }

                if(AgData.messages != null)
                {
                    List<Message> MesToRemove = new List<Message>();
                    foreach(var msg in AgData.messages)
                    {
                        if(msg is Message aMsg)
                        {
                            Location startLocation;
                            Location endLocation;

                            string senderName = aMsg.sender.Split("@")[0];
                            string receiverName = aMsg.receiver.Split("@")[0];

                            if (LstAgents != null && LstAgents.ContainsKey(senderName))
                            {
                                UnityEngine.Vector3 p = LstAgents[senderName].transform.position;
                                startLocation = new Location { x = p.x, y = p.y, z = p.z };
                            }
                            else
                            {
                                startLocation = FindPoint(senderName);
                                UnityEngine.Vector3 pos = new UnityEngine.Vector3(startLocation.x, startLocation.y, startLocation.z);

                                GameObject obj = Instantiate(agentPrefab, ParentScene);
                                obj.transform.localPosition = pos;
                                obj.transform.localRotation = UnityEngine.Quaternion.identity;
                                obj.name = senderName;
                                LstAgents[senderName] = obj;
                            }

                            if (LstAgents != null && LstAgents.ContainsKey(receiverName))
                            {
                                UnityEngine.Vector3 p = LstAgents[receiverName].transform.position;
                                endLocation = new Location { x = p.x, y = p.y, z = p.z };
                            }
                            else
                            {
                                // Si l'agent n'existe pas du tout dans la scène, on le crée à sa position fixe théorique
                                endLocation = FindPoint(receiverName);
                                UnityEngine.Vector3 pos = new UnityEngine.Vector3(endLocation.x, endLocation.y, endLocation.z);
                                GameObject obj = Instantiate(agentPrefab, ParentScene);
                                obj.transform.localPosition = pos;
                                obj.transform.localRotation = UnityEngine.Quaternion.identity;
                                obj.name = receiverName;
                                LstAgents[receiverName] = obj;
                            }
                            int performative = aMsg.performative;
                            if(performative == -1)
                            {
                                continue;
                            }
                            else
                            {
                                UnityEngine.Color couleur = Color.white;
                                switch (performative)
                                {
                                    case 0 : case 1: case 4: case 9: //On accepte la demande, la proposition, on confirme l'information, on répond à la question -> vert
                                        couleur = Color.green;
                                        break;
                                    case 3: //appel d'offre
                                        couleur = new Color(142/255f,39/255f,245/255f); //violet sombre
                                        break;
                                    case 2 : case 5: case 6: case 10: //on infirme, refuse, signal un échec -> rouge 
                                        couleur = Color.red;
                                        break;
                                    case 7 : //inform
                                        couleur = Color.blue;
                                        break;
                                    case 8: case 12: case 13: case 14: case 15://pose une question
                                        couleur = new Color(245/255f,142/255f,39/255f); //orange
                                        break;
                                    case 11 : //réponse à un 3
                                        couleur = new Color(180/255f,39/255f,245/255f); // violet clair
                                        break;
                                    case 16: case 17: case 18: //demande la réalisation d'une action
                                        break;
                                    case 19 : //abonnements
                                        couleur = Color.yellow;
                                        break; 
                                    case 20 : case 21 : //demande de transmettre des infos / messages d'autres agents
                                        couleur = Color.cyan;
                                        break;
                                    default : //pas trouvé ou pas connu
                                        couleur = Color.black;
                                        break;
                                }
                                AgMessageAnimation.Instance.PrintAgMessage(startLocation, endLocation, couleur);
                            }
                            MesToRemove.Add(msg);
                        }
                    }
                    foreach(Message mes in MesToRemove)
                    {
                        AgData.messages.Remove(mes);
                    }
                }
            }

            else
            {
                if(AgData.logs != null)
                {
                    foreach(var agLog in AgData.logs)
                    {
                        if(agLog is Logs logs)
                        {
                            if (!LstAgLogs.Keys.Contains(logs.name))
                            {
                                LstAgLogs[logs.name] = new List<Logs>();
                            }
                            LstAgLogs[logs.name].Add(logs);
                        }
                    }
                }

                else
                {
                    return;
                }
            }
        }
    }

//_________________________Section affichage points de la carte_______________________________

    /// <summary>
    /// Remplie et renvoie le dictionnaire pointsMap avec les noms et coordonnées des points présents sur la carte générée sur Unity
    /// </summary>
    /// <param name="pointsMap">Dictionnaire des points de la carte et de leurs coordnées</param>
    /// <returns>Dictionnaire rempli des noms et coordonnées des points de la carte (renvoie un dictionnaire vide si il n'y en a pas)</returns>
    Dictionary<string, Location> InitializePointsMap(Dictionary<string, Location> pointsMap)
    {
        //On ne renvoie rien mais on modifie la variable globale pointsMap avec tous les noms des points + leurs coordonnées x, y et z
        //On cherche tous les composants TextMeshPro de la scène, ie les composants qui ont un titre
        TMPro.TMP_Text[] allTexts = FindObjectsByType<TMPro.TMP_Text>(FindObjectsSortMode.None);

        if (allTexts.Length == 0)
        {
            Debug.LogWarning("[ScenarioManager] Aucun composant TextMeshPro (TMP_Text) n'a été trouvé dans la scène");
            return new Dictionary<string, Location>(); //data qui est renvoyé est vide ici
        }

        foreach (var textComp in allTexts)
        {
            // On récupère le parent qui porte le texte
            // le (x, y, z) de l'objet qui a le nom textComp avec x= la position, y = la rotation, z = l'échelle
            Transform pointTransform = textComp.transform.parent; 
            
            if (pointTransform != null)
            {
                if (pointTransform.name.Contains("Canvas") || pointTransform.name.Contains("Label"))
                {
                    if (pointTransform.parent != null)
                    {
                        pointTransform = pointTransform.parent;
                    }
                }
                float positionX = pointTransform.position.x; 
                float positionZ = pointTransform.position.z;
                float positiony = pointTransform.position.y;

                string pointName = pointTransform.name;

                if (!pointsMap.ContainsKey(pointName))
                {
                    pointsMap.Add(pointName, new Location { x = positionX, y = positiony, z = positionZ });
                }
            }
        }
        return pointsMap;
    }

    /// <summary>
    /// Trouve dans le dictionnaire pointsMap et renvoie les coordonnées du point namePoint
    /// </summary>
    /// <param name="namePoint">Nom du point dont on veut trouver les coordonnées</param>
    /// <returns>Coordonnées du point namePoint (=(0.0.0) si le point n'a pas été trouvé dans le dictionnaire)</returns>
    Location FindPoint(string namePoint)
    {
        Location res = new()
        {
            x=0f,
            y=0f,
            z=0f
        };
        string target = Regex.Replace(namePoint.ToLower(), @"[_\-.]", "");
        target = target.Split("@")[0];

        List<string> lstClees = pointsMap.Keys.ToList();

        foreach(string clee in lstClees)
        {
            string cleeTrans = Regex.Replace(clee.ToLower(), @"[_\-.]", "");
            if (target.Contains(cleeTrans))
            {
                res.x = pointsMap[clee].x;
                res.y = pointsMap[clee].y;
                res.z = pointsMap[clee].z;
                break;
            }
        }

        return res;
    }
}