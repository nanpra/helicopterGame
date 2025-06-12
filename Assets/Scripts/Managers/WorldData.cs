using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldData : MonoBehaviour
{
    public TextMeshProUGUI worldNameText;
    public TextMeshProUGUI priceText;
    public Image coinsImage;
    public TextMeshProUGUI buyButtonText;

    public bool isBought = false;
    public int price;

    public int sceneBuildIndex;

    private void OnEnable()
    {
        isBought = PlayerPrefsExtra.GetBool("isBought" + worldNameText, isBought);
        if(isBought)
        {
            coinsImage.gameObject.SetActive(false);
            buyButtonText.SetText("Play");
            priceText.gameObject.SetActive(false);
        }
    }
    public void BuyButton()
    {
        if(isBought)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
        else
        {
            if (UiManager.instance.HasEnoughCoins(price))
            {
                coinsImage.gameObject.SetActive(false);
                buyButtonText.SetText("Play");
                priceText.gameObject.SetActive(false);
                isBought = true;
                PlayerPrefsExtra.SetBool("isBought" + worldNameText, isBought);
            }
        }
    }
}
