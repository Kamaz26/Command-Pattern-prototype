using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommandManager : MonoBehaviour
{
    public Material originalMat;
    private static CommandManager _instance;
    public static CommandManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("The command manager is NULL.");

             return _instance;       
        }
    }

    private List<ICommand> _commandBuffer = new List<ICommand>();

    public void Awake()
    {
        _instance = this;
    }

    public void AddCommand(ICommand command)
    {
        _commandBuffer.Add(command);
    }

    public void Play()
    {
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        foreach(var command in _commandBuffer)
        {
            yield return new WaitForSeconds(.5f);
            command.Execute();
            yield return new WaitForSeconds(1.0f);
            Done();
        }        
    }

    public void Rewind()
    {
        StartCoroutine(RewindRoutine());
    }

    IEnumerator RewindRoutine()
    {
        foreach(var command in Enumerable.Reverse(_commandBuffer))
        {
            command.Undue();
            yield return new WaitForSeconds(1.0f);
        }

    }

    public void Done()
    {
        var cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach(var cube in cubes)
        {
            cube.GetComponent<MeshRenderer>().material = originalMat;
        }
    }

    public void Reset()
    {
        _commandBuffer.Clear();
    }
}
