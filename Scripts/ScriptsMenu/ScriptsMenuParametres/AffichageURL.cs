using UnityEngine;
using TMPro;

/// <summary>
/// Permet d'afficher l'URL courante dans le panneau des paramètres et de la mettre à jour
/// </summary>
public class AffichageURL : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textURL;

    void Awake()
    {
        textURL.text = RestAgentJson.url;
    }

    /// <summary>
    /// Change l'URL courant par celle donnée en paramètre
    /// </summary>
    /// <param name="newUrl">Nouvelle URL à considérer</param>
    public void MajUrl(string newUrl)
    {
        RestAgentJson.url = newUrl;
        textURL.text = RestAgentJson.url;
    }
}