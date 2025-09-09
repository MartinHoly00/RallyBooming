using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    Speed,
    Acceleration,
    Health,
    Repair,
    Steering,
    Brake,
    Nitro,
    XPBoost
}
public class Upgrade
{
    public string header;
    public Sprite icon;
    public string description;
    public UpgradeType type;

    public Upgrade(string header, Sprite icon, string description, UpgradeType upgradeType)
    {
        this.header = header;
        this.icon = icon;
        this.description = description;
        this.type = upgradeType;// Default type, can be modified later
    }
}

public class ShowUpgrade : MonoBehaviour
{
    public TextMeshProUGUI headerObject;
    public Image iconObject;
    public TextMeshProUGUI descriptionObject;

    public string header;
    public Sprite icon;
    public string description;

    public void SetUpgrade(Upgrade upgrade)
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
