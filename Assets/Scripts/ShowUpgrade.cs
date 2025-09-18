using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowUpgrade : MonoBehaviour
{
    public TextMeshProUGUI headerObject;
    public Image iconObject;
    public TextMeshProUGUI descriptionObject;

    public string header;
    public Sprite icon;
    public string description;

    public void SetUpgrade(UpgradeData upgrade)
    {
        header = upgrade.header;
        icon = upgrade.icon;
        description = upgrade.description;

        if (headerObject != null)
        {

            headerObject.text = header;

        }

        if (iconObject != null)
        {
            iconObject.sprite = icon;
        }

        if (descriptionObject != null)
        {
            descriptionObject.text = description;

        }
    }
}
