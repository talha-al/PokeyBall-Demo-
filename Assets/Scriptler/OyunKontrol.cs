using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OyunKontrol : MonoBehaviour
{
    TopKontrol _topKontrolScript;

    [SerializeField]
    private Text solLevelText, sagLevelText, skorText, enYüksekSkor, altinText;
    public Text gameOverText;

    public GameObject bitisDeligi, siyahEkran;
    public GameObject[] levelSeviyeDizisi;

    public LayerMask[] _layer;
    public Slider _slider;

    public int _maxColliders, collidersSayac;
    public int skor, rekor, levelSayac, altin;

    void Start()
    {
        _topKontrolScript = GameObject.FindGameObjectWithTag("Top").GetComponent<TopKontrol>();
        _maxColliders = 20;
        bitisDeligi.SetActive(false);

        levelSayac = PlayerPrefs.GetInt("LevelSayac");
        levelSeviyeDizisi[levelSayac].SetActive(true);  // En son hangi bölümde kalındıysa o bölümden oyun devam eder
    }

    void Update()
    {
        _slider.maxValue = _maxColliders;
        _slider.value = collidersSayac;

        rekor = PlayerPrefs.GetInt("EnYüksekSkor");
        skorText.text = "Skor: " + _topKontrolScript.skor;
        enYüksekSkor.text = "Best: " + rekor;

        solLevelText.text = "" + (levelSayac + 1); //Skorbarda level ilerlemesinin görülmesi için eklendi
        sagLevelText.text = "" + (levelSayac + 2);

        altinText.text = "" + altin;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    //[MenuItem("My Menu/Kayit Sil")]
    //static void KayitSil()
    //{
    //    PlayerPrefs.DeleteAll();
    //}
}
