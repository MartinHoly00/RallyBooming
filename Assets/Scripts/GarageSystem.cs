using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageSystem : MonoBehaviour
{
    [Header("Data")]
    public CarDatabase carDatabase;

    [Header("UI")]
    public GameObject garagePanel;
    public GameObject carButton;       // simple button prefab to instantiate for each car
    public Transform carListContent;   // content/parent (should be a RectTransform)
    public GameObject carCardPrefab;

    [Header("Player")]
    public int money;
    public TextMeshProUGUI moneyText;


    void Start()
    {
        money = PlayerPrefs.GetInt("Money", 0);
        if (moneyText != null) moneyText.text = money.ToString() + "$";
        RenderCarButtons();
        RenderCarCard(carDatabase.cars[0]);
    }
    public void RenderCarButtons()
    {
        if (carDatabase == null || carButton == null || carListContent == null || carDatabase.cars == null || carDatabase.cars.Count == 0)
        {
            return;
        }

        foreach (Transform child in carListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (CarData car in carDatabase.cars)
        {
            GameObject btnObj = Instantiate(carButton, carListContent);
            btnObj.name = "CarButton_" + (string.IsNullOrEmpty(car.carId) ? (car.carPrefab ? car.carPrefab.name : "Unknown") : car.carId);
            //set text of button
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = car.carName;

            TextMeshProUGUI label = btnObj.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                label.text = !string.IsNullOrEmpty(car.carId) ? car.carId : (car.carPrefab ? car.carPrefab.name : "Unnamed Car");
            }

            Button uiButton = btnObj.GetComponent<Button>();
            if (uiButton != null)
            {
                uiButton.onClick.RemoveAllListeners();
                uiButton.GetComponentInChildren<TextMeshProUGUI>().text = car.carName;
                uiButton.onClick.AddListener(() => RenderCarCard(car));
            }
        }
    }

    public void RenderCarCard(CarData car)
    {
        if (carCardPrefab == null || car == null)
        {
            return;
        }

        //destroy previous card based on CarCardDisplay
        foreach (Transform child in garagePanel.transform)
        {
            CarCardDisplay existingCard = child.GetComponent<CarCardDisplay>();
            if (existingCard != null)
            {
                Destroy(child.gameObject);
            }
        }
        GameObject cardObj = Instantiate(carCardPrefab, garagePanel.transform);
        cardObj.name = "CarCard_" + (string.IsNullOrEmpty(car.carId) ? (car.carPrefab ? car.carPrefab.name : "Unknown") : car.carId);
        CarCardDisplay cardDisplay = cardObj.GetComponent<CarCardDisplay>();

        //handle buttons
        cardDisplay.buyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        cardDisplay.buyButton.GetComponent<Button>().onClick.AddListener(() => BuyCar(car));
        cardDisplay.equipButton.GetComponent<Button>().onClick.RemoveAllListeners();
        cardDisplay.equipButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            foreach (CarData c in carDatabase.cars)
            {
                c.isSelected = false;
            }
            car.isSelected = true;
            PlayerPrefs.SetString("SelectedCarId", car.carId);
            RenderCarCard(car);
            RenderCarButtons();
        });

        if (car.isUnlocked)
        {
            cardDisplay.buyButton.SetActive(false);
            cardDisplay.equipButton.SetActive(!car.isSelected);
            cardDisplay.equippedCarText.gameObject.SetActive(car.isSelected);
        }
        else
        {
            cardDisplay.buyButton.SetActive(true);
            cardDisplay.equipButton.SetActive(false);
            cardDisplay.equippedCarText.gameObject.SetActive(false);
        }
        //set text of card
        cardDisplay.SetCarData(car);
    }

    public void BuyCar(CarData car)
    {
        if (car == null || car.isUnlocked)
        {
            return;
        }
        if (money >= car.price)
        {
            money -= car.price;
            PlayerPrefs.SetInt("Money", money);
            if (moneyText != null) moneyText.text = money.ToString() + "$";
            car.isUnlocked = true;
            RenderCarCard(car);
            RenderCarButtons();
        }
        else
        {
            Debug.Log("Not enough money to buy this car.");
        }
    }
}
