using TMPro;
using UnityEngine;

public class CarCardDisplay : MonoBehaviour
{
    [Header("Car info")]
    public TextMeshProUGUI carNameText;
    public GameObject carImageObject;
    public TextMeshProUGUI carDescriptionText;
    public TextMeshProUGUI carPriceText;
    public TextMeshProUGUI carHealthText;
    public TextMeshProUGUI carSpeedText;
    [Header("Buttons")]
    public GameObject buyButton;
    public GameObject equipButton;

    public TextMeshProUGUI equippedCarText;

    public void SetCarData(CarData carData)
    {
        if (carData == null)
        {
            Debug.LogWarning("CarCardDisplay: carData is null.");
            return;
        }

        if (carNameText != null)
            carNameText.text = carData.carName;

        if (carImageObject != null && carData.carImage != null)
        {
            var imageComponent = carImageObject.GetComponent<UnityEngine.UI.Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = carData.carImage;
            }
        }

        if (carDescriptionText != null)
            carDescriptionText.text = carData.description;

        if (carPriceText != null)
            carPriceText.text = "Price: " + carData.price.ToString() + "$";

        if (carHealthText != null)
            carHealthText.text = "Health: " + carData.health.ToString();

        if (carSpeedText != null)
            carSpeedText.text = "Speed: " + carData.maxSpeed.ToString();

        // Update button visibility based on unlock status
        if (buyButton != null)
            buyButton.SetActive(!carData.isUnlocked);

        if (equipButton != null)
            equipButton.SetActive(carData.isUnlocked && !carData.isSelected);
    }
}
