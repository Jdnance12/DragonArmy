using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---- Assets ----")]
    [Header("Player")]
    public GameObject player;
    public CharacterController playerScript;
    public GameObject playerCam;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<CharacterController>();
        playerCam = GameObject.Find("CameraPos");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
