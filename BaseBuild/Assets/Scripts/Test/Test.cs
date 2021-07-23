using System.Collections;
using System.Collections.Generic;
using Build.Map;
using UnityEngine;
using Zenject;

public class Test : MonoBehaviour
{
    [Inject]
    private IMap map;

    [Inject]
    private IGridPositions _gridPositions;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var pos = map.ClampPosition(Input.mousePosition);
        Debug.Log(_gridPositions.GetGridPosition(pos));
    }
}
