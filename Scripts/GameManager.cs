using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> _cubes = new List<GameObject>();
    public List<GameObject> _choosenCubes = new List<GameObject>();
    public List<GameObject> _playerCubes = new List<GameObject>();
    public Material originalMat;
    private int _randCube;
    public bool stopped;
    public bool _replayCd;
    public int _diff = 5;

    public GameObject _startButton;
    public GameObject _replayButton;
    public Text _score;

    void Start()
    {
        
    }

    
    void Update()
    {
        UpdateUI();
        UserInput();
    }

    public void StartGame()
    {
        if (PlayerPrefs.GetInt("score") > 9 && PlayerPrefs.GetInt("score") < 20)
            _diff = 6;
        else if (PlayerPrefs.GetInt("score") > 19)
            _diff = 7;

        CommandManager.Instance.Reset();
        _choosenCubes.Clear();
        StartCoroutine(StartPicking());
    }

    public void Replay()
    {
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") - 2);
        foreach (var cubes in _choosenCubes)
        {
            cubes.GetComponent<MeshRenderer>().material = originalMat;
        }
        StartCoroutine(ReplayCd());
    }

    public void UpdateUI()
    {
        if (_choosenCubes.Count > 0)
            _startButton.SetActive(false);
        else
            _startButton.SetActive(true);

        if (_choosenCubes.Count >= _diff)
        {
            _replayButton.SetActive(true);
            stopped = true;
        }
        else
        {
            _replayButton.SetActive(false);
            stopped = false;
        }

        if (_playerCubes.Count == _choosenCubes.Count)
        {
            foreach (var cubes in _choosenCubes)
            {
                cubes.GetComponent<MeshRenderer>().material = originalMat;
            }
            _choosenCubes.Clear();
            _playerCubes.Clear();
        }

        _score.text = "Score: " + PlayerPrefs.GetInt("score");

        if (PlayerPrefs.GetInt("score") < 0)
            PlayerPrefs.SetInt("score", 0);
    }

    public void UserInput()
    {
        if (_replayCd == false && stopped == true && Input.GetMouseButtonDown(0))
        {
            Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            
            if (Physics.Raycast(rayOrigin, out hitInfo))
            {
                if (hitInfo.collider.tag == "Cube")
                {
                    var playerCubes = _playerCubes.Count;
                    if (hitInfo.collider.name == _choosenCubes[playerCubes].name)
                    {
                        _playerCubes.Add(_choosenCubes[playerCubes]);
                        _choosenCubes[playerCubes].GetComponent<MeshRenderer>().material.color = Color.green;
                        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 1);
                        Debug.Log("GJ");
                    }
                    else
                    {
                        _playerCubes.Clear();
                        PlayerPrefs.SetInt("score", 0);
                        foreach (var cubes in _choosenCubes)
                        {
                            cubes.GetComponent<MeshRenderer>().material = originalMat;
                        }

                        Debug.Log("Wrong!");
                    }
                }
            }
        }
    }

    IEnumerator StartPicking()
    {
        do
        {
            _randCube = Random.Range(0, _cubes.Count);
            var _color = new Color(Random.value, Random.value, Random.value);
            _cubes[_randCube].GetComponent<MeshRenderer>().material.color = _color;
            _choosenCubes.Add(_cubes[_randCube]);
            ICommand pick = new CubeCommand(_cubes[_randCube], _color);
            pick.Execute();
            CommandManager.Instance.AddCommand(pick);
            yield return new WaitForSeconds(1);
            _cubes[_randCube].GetComponent<MeshRenderer>().material = originalMat;

        } while (!stopped && _choosenCubes.Count < _diff);
    }

    IEnumerator ReplayCd()
    {
        _replayCd = true;
        yield return new WaitForSeconds(_diff);
        _replayCd = false;
    }
}
