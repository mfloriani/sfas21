using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private StoryData _data;

    private TextDisplay _output;
    private BeatData _currentBeat;
    private WaitForSeconds _wait;

    private void Awake()
    {
        _output = GetComponentInChildren<TextDisplay>();
        _currentBeat = null;
        _wait = new WaitForSeconds(0.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (_output.IsIdle)
        {
            if (_currentBeat == null)
            {
                DisplayBeat(1);
            }
            else
            {
                UpdateInput();
            }
        }
    }

    private void UpdateInput()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if(_currentBeat != null)
        //    {
        //        if (_currentBeat.ID == 1)
        //        {
        //            Application.Quit();
        //        }
        //        else
        //        {
        //            DisplayBeat(1);
        //        }
        //    }
        //}
        //else
        //{
            KeyCode alpha = KeyCode.Alpha1;
            KeyCode keypad = KeyCode.Keypad1;

            for (int count = 0; count < _currentBeat.Decision.Count; ++count)
            {
                if (alpha <= KeyCode.Alpha9 && keypad <= KeyCode.Keypad9)
                {
                    if (Input.GetKeyDown(alpha) || Input.GetKeyDown(keypad))
                    {
                        ChoiceData choice = _currentBeat.Decision[count];
                        //Debug.Log("choice.NextID:" + choice.NextID);
                        DisplayBeat(choice.NextID);
                        break;
                    }
                }

                ++alpha;
                ++keypad;
            }
        //}
    }

    private void DisplayBeat(int id)
    {
        BeatData data = _data.GetBeatById(id);
        StartCoroutine(DoDisplay(data));
        _currentBeat = data;
    }

    private IEnumerator DoDisplay(BeatData data)
    {
        _output.Clear();

        while (_output.IsBusy)
        {
            yield return null;
        }

        _output.Display(data.DisplayText);

        while(_output.IsBusy)
        {
            yield return null;
        }

        

        for (int count = 0; count < data.Decision.Count; ++count)
        {
            ChoiceData choice = data.Decision[count];
            
            //Debug.Log("choice:" + choice.DisplayText);

            _output.Display(string.Format("{0}: {1}", (count + 1), choice.DisplayText));

            while (_output.IsBusy)
            {
                yield return null;
            }
        }

        if(data.Decision.Count > 0)
        {
            _output.ShowWaitingForInput();
        }

        //Debug.Log("data.ID:" + data.ID);
        if (data.ID == 2)
        {
            GameConfig.Player = PlayerChoice.Spy;
            SceneManager.LoadScene("GameScene");
        }
        else if (data.ID == 3)
        {
            GameConfig.Player = PlayerChoice.Soldier;
            SceneManager.LoadScene("GameScene");
        }


    }
}
