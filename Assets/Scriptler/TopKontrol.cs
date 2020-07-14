using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopKontrol : MonoBehaviour
{
    Rigidbody _rb;
    OyunKontrol _oyunKontrol;
    RaycastHit hit;
    MeshRenderer _topMesh, _cubukMesh;
    LayerMask hedefLayer;

    private Vector3 ilkPozisyon;

    [SerializeField]
    private GameObject _topCubuk;

    [SerializeField]
    private int firlatmaHizi;

    public int skor, rekor;
    private int mouseKontrolSayaci = 1;

    private bool ziplamaKontrol = true, topKontrol = true; //Kontrol mekanizması için kullanıldı

    void Start()
    {
        ilkPozisyon = transform.position;

        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;                   // Topun havada asılı kalması için kullanıldı

        _oyunKontrol = GameObject.FindGameObjectWithTag("OyunKontrol").GetComponent<OyunKontrol>();

        _topMesh = GetComponent<MeshRenderer>();
        _cubukMesh = _topCubuk.GetComponent<MeshRenderer>();
        _cubukMesh.enabled = true;

        hedefLayer = LayerMask.GetMask("HedefTahtasi");  // Hedef tahtasının merkezindeki colliderı algılamak için layer kullanıldı
    }

    void Update()
    {
        if (topKontrol)
        {
            Hareket();
        }

        LevelSonuPuanlama();

    }

    private void Hareket()
    {

        if (Input.GetMouseButtonDown(0)) // ilk basma
        {

            if (Physics.Raycast(transform.position, Vector3.left, out hit))
            {
                Debug.DrawRay(transform.position, Vector3.left * 100, Color.yellow);

                _rb.isKinematic = true;
                _cubukMesh.enabled = true; // Havada asılı kaldığında, duvara çubukla tutunduğu hissiyatı vermek için kullanıldı
                mouseKontrolSayaci++;  // Arka arkaya zıplamalar olmasını engellemek için kontrol sayacı eklendi

                firlatmaHizi = 700;
                skor += 400; // Duvara her tutuşta skor attırıldı
            }

            if (Physics.Raycast(transform.position, Vector3.left, 10, hedefLayer))  //Hedef tahtasının merkezinin vurulup vurulmadığı kontrol ediliyor
            {
                firlatmaHizi = 1200;
                skor += 9200;
            }
        }



        if (Input.GetMouseButton(0) && ziplamaKontrol && mouseKontrolSayaci == 2) // Basılı tutma
        {
            ziplamaKontrol = false;
            _rb.isKinematic = false; //Kinematic false haline getirilerek zıplama önündeki engel kaldırıldı


            _rb.useGravity = false; // Basılı tutulduğu süre boyunca topun düşmesi engellendi
        }

        if (Input.GetMouseButtonUp(0) && !ziplamaKontrol && mouseKontrolSayaci == 2) // Moustan çektikten sonra
        {
            _rb.AddForce(new Vector3(0, firlatmaHizi, 0), ForceMode.Acceleration);
            ziplamaKontrol = true;
            mouseKontrolSayaci = 0;
            _rb.useGravity = true;
            _cubukMesh.enabled = false;
        }
    }

    private void LevelSonuPuanlama()
    {

        if (Input.GetMouseButtonDown(0))
        {

            if (Physics.Raycast(transform.position, Vector3.left, 10, _oyunKontrol._layer[0])) // Layer katmanlarına eklenen bölüm sonu küplerine göre altın kazanılması sağlandı
            {
                _oyunKontrol.altin += 2;
                StartCoroutine(LevelSonuNumerator());
            }

            if (Physics.Raycast(transform.position, Vector3.left, 10, _oyunKontrol._layer[1]))
            {
                _oyunKontrol.altin += 4;
                StartCoroutine(LevelSonuNumerator());
            }

            if (Physics.Raycast(transform.position, Vector3.left, 10, _oyunKontrol._layer[2]))
            {
                _oyunKontrol.altin += 5;
                StartCoroutine(LevelSonuNumerator());
            }

            if (Physics.Raycast(transform.position, Vector3.left, 10, _oyunKontrol._layer[3]))
            {
                _oyunKontrol.altin += 10;
                StartCoroutine(LevelSonuNumerator());
            }

            if (Physics.Raycast(transform.position, Vector3.left, 10, _oyunKontrol._layer[4]))
            {
                _oyunKontrol.altin += 16;
                StartCoroutine(LevelSonuNumerator());
            }

            if (Physics.Raycast(transform.position, Vector3.left, 10, _oyunKontrol._layer[5]))
            {
                skor = 0;
                _oyunKontrol.gameOverText.text = "Game Over...";
                _oyunKontrol.siyahEkran.SetActive(true);
                topKontrol = false;

                StartCoroutine(Zaman());
            }
        }


    }

    private IEnumerator LevelSonuNumerator()
    {
        topKontrol = false;
        yield return new WaitForSeconds(1);
        _rb.isKinematic = false;
        _cubukMesh.enabled = false;

        yield return new WaitForSeconds(2);

        YeniLevel();
    }

    private void YeniLevel()
    {
        _oyunKontrol.collidersSayac = 0;
        _oyunKontrol._slider.value = 0;

        topKontrol = true;
        _topMesh.enabled = true;

        _oyunKontrol.levelSeviyeDizisi[_oyunKontrol.levelSayac].SetActive(true); // Yeni levela geçildi
        transform.position = ilkPozisyon;
        _oyunKontrol.bitisDeligi.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "katman")  // Her bir colliderdan geçildiğinde sayaç sayesinde skorbarda ilerleme elde edilir
        {
            _oyunKontrol.collidersSayac += 1;
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "altin")
        {
            _oyunKontrol.altin += 1;
            other.gameObject.SetActive(false);
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "BitisBayragi")
        {
            _oyunKontrol.bitisDeligi.SetActive(true);
        }


        if (other.gameObject.tag == "BitisDeligi")
        {
            _topMesh.enabled = false;
            _rb.isKinematic = true;
            other.gameObject.SetActive(false);

            if (skor > rekor)
            {
                rekor = skor;
                PlayerPrefs.SetInt("EnYüksekSkor", rekor);
            }

            _oyunKontrol.levelSeviyeDizisi[_oyunKontrol.levelSayac].SetActive(false);  // Level bittikten sonra bu objeyi false ederek aynı sahnede diğer levela geçildi
            _oyunKontrol.levelSayac += 1;

            if (_oyunKontrol.levelSayac >= _oyunKontrol.levelSeviyeDizisi.Length)
            {
                _oyunKontrol.levelSayac = 0;
            }


            PlayerPrefs.SetInt("LevelSayac", _oyunKontrol.levelSayac);

            YeniLevel();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "zemin")
        {
            skor = 0;
            _oyunKontrol._slider.value = 0;
            _oyunKontrol.gameOverText.text = "Game Over...";
            _oyunKontrol.siyahEkran.SetActive(true);
            topKontrol = false;

            StartCoroutine(Zaman());
        }
    }

    private IEnumerator Zaman()
    {
        yield return new WaitForSeconds(2);
        topKontrol = true;
        _rb.isKinematic = true;
        transform.position = ilkPozisyon;
        _oyunKontrol.gameOverText.text = "";
        _oyunKontrol.siyahEkran.SetActive(false);
    }


}

