using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Start Point")]
    public Vector3 startPoint;

    [Header("Map Size")]
    public int countBlocks;

    [Header("Blocks for Frame")]
    public GameObject frameBlock;

    [Header("Blocks for Map")]
    public GameObject[] blocks;

    [Header("Bases")]
    public GameObject playerBase;
    public GameObject enemyBase;

    void Awake()
    {
        GenerateFrame();
        GenerateMap();
        AstarPath.active.Scan();
    }

    void GenerateMap()
    {
        int i = 0, j;

        Vector3 startPointDup;
        while (i < countBlocks)
        {
            startPointDup = startPoint;
            j = 0;
            while (j < countBlocks)
            {
                if ((i == 0) && (j == countBlocks - 1))
                    Instantiate(enemyBase, startPointDup, Quaternion.identity);

                else if ((i == countBlocks - 1) && (j == 0))
                    Instantiate(playerBase, startPointDup, Quaternion.identity);

                if (i < 3)
                {
                    if (j < countBlocks - 3)
                        SpawnBlock(startPointDup);
                }
                else if (i > countBlocks - 4)
                {
                    if (j > 2)
                        SpawnBlock(startPointDup);
                }
                else
                    SpawnBlock(startPointDup);
                startPointDup.x++;
                j++;
            }
            startPoint.y--;
            i++;
        }
    }

    void SpawnBlock(Vector3 startPointDup) 
    {
        int r = Random.Range(-2, blocks.Length);
        if (r > -1)
        {
            Instantiate(blocks[r], startPointDup, Quaternion.identity);
        }
    }

    void GenerateFrame()
    {
        Vector3 startFramePoint = new Vector3(startPoint.x-1, startPoint.y+1, startPoint.z);

        for (int i = 0; i < countBlocks + 2; i++)
        {
            Instantiate(frameBlock, startFramePoint, Quaternion.identity);
            startFramePoint.x++;
        }

        startFramePoint.x--;
        startFramePoint.y--;

        for (int i = 0; i < countBlocks + 1; i++)
        {
            Instantiate(frameBlock, startFramePoint, Quaternion.identity);
            startFramePoint.y--;
        }

        startFramePoint.y++;
        startFramePoint.x--;

        for (int i = 0; i < countBlocks + 1; i++)
        {
            Instantiate(frameBlock, startFramePoint, Quaternion.identity);
            startFramePoint.x--;
        }

        startFramePoint.x++;
        startFramePoint.y++;

        for (int i = 0; i < countBlocks; i++)
        {
            Instantiate(frameBlock, startFramePoint, Quaternion.identity);
            startFramePoint.y++;
        }
    }
}
