using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldActif;

    public void ActiverLePanneau(TMP_InputField inputFieldCliqué)
    {
        inputFieldActif = inputFieldCliqué;
    }

    public void TaperCaractere(string textAAlimenter)
    {
        if(inputFieldActif != null)
        {
            inputFieldActif.text += textAAlimenter;
        }
    }

    public void EffacerDernierCaractere()
    {
        if (inputFieldActif != null && inputFieldActif.text.Length > 0)
        {
            inputFieldActif.text = inputFieldActif.text.Substring(0, inputFieldActif.text.Length - 1);
        }
    }

}