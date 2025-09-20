using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public GameObject shopUI;
    public GameObject mainMenuUI;

    void Start()
    {

    }

    public void SetActiveCar(int carIndex)
    {
        PlayerPrefs.SetString("SelectedCarId", carIndex.ToString());
        Debug.Log("SelectedCarId: " + carIndex);
    }
}
