using UnityEngine;

public class UIHPController : MonoBehaviour
{
    [SerializeField] private GameObject[] heartSlots;
    [SerializeField] private Player playerScript;
    private int hp;

    void Start()
    {
        if (playerScript != null)
            hp = playerScript.GetHP();
        UpdateHearts();
    }

    void Update()
    {
        if (playerScript != null)
            hp = playerScript.GetHP();
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartSlots.Length; i++)
        {
            GameObject full = heartSlots[i].transform.Find("fullHeart").gameObject;
            GameObject half = heartSlots[i].transform.Find("halfHeart").gameObject;

            int heartHp = i * 2;

            if (hp >= heartHp + 2)
            {
                full.SetActive(true);
                half.SetActive(false);
            }
            else if (hp == heartHp + 1)
            {
                full.SetActive(false);
                half.SetActive(true);
            }
            else
            {
                full.SetActive(false);
                half.SetActive(false);
            }
        }
    }
}
