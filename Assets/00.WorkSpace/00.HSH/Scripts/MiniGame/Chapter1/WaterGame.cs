using UnityEngine;

public class WaterGame : MonoBehaviour
{
    public GameObject[] waterbarrel;

    [SerializeField] int ChoiceCost;
    private void Start()
    {
        ChoiceCost = 4;
    }
    public void WaterGallen()
    {
        ChoiceCost--;
        for (int i = 0; i < waterbarrel.Length; i++)
        {
            
            if (waterbarrel[i].activeSelf == false)
            {
                waterbarrel[i].SetActive(true);
                return;
            }
        }
    }
}