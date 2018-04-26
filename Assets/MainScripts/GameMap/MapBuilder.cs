using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapBuilder : MonoBehaviour
{

    [SerializeField]
    float LimetedReliefAngle = Mathf.PI / 4;
    [SerializeField]
    int width;
    [SerializeField]
    int height;
    [SerializeField]
    Slider ProgBar;
    RawMap rm;

    bool isScanned = false;

    // Use this for initialization
    void Start() {
        rm = new RawMap(width, height, LimetedReliefAngle);
    }

    // Update is called once per frame
    void Update() {

        rm.Iteration();
        ProgBar.value = rm.Progress;
        if (ProgBar.value == 1)
        {
            ProgBar.gameObject.SetActive(false);
        }

    }

    public void StartMapBuild()
    {
        rm.StartBuild();
        ProgBar.gameObject.SetActive(true);
    }


     public static void ToDataFile(CellType[,] cells)
    {
        BinaryWriter br = new BinaryWriter(new FileStream("map.dat", FileMode.OpenOrCreate));
        int height = cells.GetLength(0);
        int width = cells.GetLength(1);
        br.Write(height);
        br.Write(width);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CellType ct = cells[i, j];
                br.Write((int)ct);
            }
        }

        br.Close();
    }

}



