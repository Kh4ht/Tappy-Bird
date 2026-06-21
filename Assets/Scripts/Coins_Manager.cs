using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Coins_Manager : MonoBehaviour
{
    public static Coins_Manager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI coins_txt;

    private const string Coins_key = "Coins";

    public int Coins { get; private set; }

    [Header("_-_-_-_-_ Events -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-")]
    [SerializeField] private UnityEvent On_Add_Coins;
    [SerializeField] private UnityEvent On_Subtract_Coins;

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Prevent duplicates

        Load_Coins();
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    public void Add_Coins(int Value)
    {
        Coins += Value;
        coins_txt.text = Coins.ToString();

        PlayerPrefs.SetInt(Coins_key, Coins);
        PlayerPrefs.Save();

        On_Add_Coins?.Invoke();
    }

    public void Subtract_Coins(int Value)
    {
        if (Value <= Coins)
        {
            Coins -= Value;
            coins_txt.text = Coins.ToString();

            PlayerPrefs.SetInt(Coins_key, Coins);
            PlayerPrefs.Save();

            On_Subtract_Coins?.Invoke();
        }

        else
        {
            Debug.Log("Coins Are Not Enough To Make The Purchase");
        }
    }

    private void Load_Coins()
    {
        Coins = PlayerPrefs.GetInt(Coins_key, 0);

        coins_txt.text = Coins.ToString();
    }
}