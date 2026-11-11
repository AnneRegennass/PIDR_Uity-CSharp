using UnityEngine;

/// <summary>
/// Classe permettant de gérer le déplacement des agents
/// </summary>
public class AgentEntity : MonoBehaviour
{
    public string AgentID;
    public Vector3 targetPosition;
    public int number;
    
    [SerializeField] private float smoothSpeed = 5f;

    void Start()
    {
        this.targetPosition = transform.position;
    }

    /// <summary>
    /// A chaque frame, met à jour la position de l'agent pour assurer un déplacement fluide
    /// </summary>
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    /// <summary>
    /// Met à jour la position de l'agent donné 
    /// </summary>
    /// <param name="x">Coordonnée de latitude</param>
    /// <param name="y">Coorodnnée de hauteur</param>
    /// <param name="z">Coordonnée de longitude</param>
    public void UpdateStatus(float x, float y, float z)
    {
        this.targetPosition = new Vector3(x, y, z);
    }
}