using UnityEngine;

public class HudText : MonoBehaviour
{
    #region Singleton
    public static HudText Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Deleted PlayerInteract dublicate");
            Destroy(gameObject);
        }
    }
    #endregion

    public TMPro.TMP_Text tmp;

    public void ShowText(string text)
    {
        tmp.text = text;
        tmp.gameObject.SetActive(true);
    }

    public void HideText()
    {
        tmp.gameObject.SetActive(false);
    }
}
