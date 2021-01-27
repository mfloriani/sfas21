using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

enum GameState
{
    Running,
    Paused,
    Failed,
    Completed
}

public class GameManager : MonoBehaviour
{
    [SerializeField] float InfoDuration = 5.0f;

    private Transform _missionAccomplished;
    private Transform _missionFailed;
    private Transform _mainMenu;
    private Transform _missionInfo;
    private GameState _gameState;
    private TMP_Text _infoText;
    public bool _missionData;
    private GameObject _spy;
    private GameObject _soldier;

    void Start()
    {
        _spy = GameObject.Find("Spy");
        _soldier = GameObject.Find("Soldier");
        var cam = GameObject.Find("Main Camera");

        //GameConfig.Player = PlayerChoice.Soldier;

        if (GameConfig.Player == PlayerChoice.Spy)
        {
            _spy.SetActive(true);
            _soldier.SetActive(false);

            cam.GetComponent<CameraController>().PlayerTransform = _spy.transform.Find("Camera LookAt Point");
        }
        else
        {
            _spy.SetActive(false);
            _soldier.SetActive(true);

            cam.GetComponent<CameraController>().PlayerTransform = _soldier.transform.Find("Camera LookAt Point");
        }

        _gameState = GameState.Running;

        transform.Find("Canvas").gameObject.SetActive(true);

        _missionAccomplished = transform.Find("Canvas/Mission Accomplished");
        _missionFailed = transform.Find("Canvas/Mission Failed");
        _mainMenu = transform.Find("Canvas/Main Menu");
        _missionInfo = transform.Find("Canvas/Mission Info");

        _missionAccomplished.gameObject.SetActive(false);
        _missionFailed.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(false);
        _missionInfo.gameObject.SetActive(false);

        _infoText = _missionInfo.GetComponent<TMP_Text>();
        _missionData = false;

        MissionInfo("Find the computer and copy the data, then return here for extraction.");
    }

    internal void InformDeath(GameObject gameObject)
    {
        if(gameObject.tag == "Player")
        {
            var merc = gameObject.GetComponent<MercenaryController>();
            if (merc)
                merc.Die();

            var spy = gameObject.GetComponent<SpyController>();
            if (spy)
                spy.Die();

            MissionFailed();
        }
        else if(gameObject.tag == "Enemy")
        {
            gameObject.GetComponent<SoldierAI>().Die();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }

        if(_gameState != GameState.Running)
        {
            if (Input.GetButton("Action"))
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    public void AskForExtraction()
    {
        if (_missionData)
            MissionAccomplished();
        else
            MissionInfo("Find the computer and copy the data, then return here for extraction.");
    }

    public void MissionFailed()
    {
        _gameState = GameState.Failed;
        _missionFailed.gameObject.SetActive(true);
        _mainMenu.gameObject.SetActive(true);
    }

    public void MissionAccomplished()
    {
        _gameState = GameState.Completed;
        _missionAccomplished.gameObject.SetActive(true);
        _mainMenu.gameObject.SetActive(true);
    }

    public void MissionInfo(string text)
    {
        _infoText.text = text;
        StartCoroutine(ShowInfo());
    }

    private IEnumerator ShowInfo()
    {
        _missionInfo.gameObject.SetActive(true);

        yield return new WaitForSeconds(InfoDuration);

        _missionInfo.gameObject.SetActive(false);
    }

    public void InformDataCollection()
    {
        MissionInfo("Mission data collected. Go to the extraction zone");
        _missionData = true;
    }

}
