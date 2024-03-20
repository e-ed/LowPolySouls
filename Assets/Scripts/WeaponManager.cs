using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private List<GameObject> inventory;
    private GameObject player;
    public GameObject currentWeapon;
    public GameObject weaponSocket;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        inventory = player.GetComponent<PlayerScript>().inventory;
        currentWeapon = transform.GetChild(0).gameObject;
        weaponSocket = GameObject.FindGameObjectWithTag("WeaponSocket");

        for (int i = 0; i < 3; i++)
        {
            weaponSocket.transform.GetChild(i).GetComponent<WeaponScript>().isInPlayerInventory = true;
        }

    }

    // Update is called once per frame
    void Update()
    {



        if (player.GetComponent<PlayerScript>().isAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            swapWeapon(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            swapWeapon(2);
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            swapWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            swapWeapon(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            swapWeapon(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            swapWeapon(6);
        }


    }


    public void swapWeapon(int weaponChoice)
    {
        GameObject targetWeapon = weaponSocket.transform.GetChild(weaponChoice - 1).GetComponent<WeaponScript>().gameObject;

        if (!targetWeapon.GetComponent<WeaponScript>().isInPlayerInventory) return;

        currentWeapon.gameObject.SetActive(false);
        currentWeapon = gameObject.transform.GetChild(weaponChoice - 1).gameObject;
        currentWeapon.SetActive(true);


    }
}
