using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //	The prefab used for the floor of the level
    public Transform tilePrefab;
    public float ScaleModifier = 1;

    // A Vector2 for the size of the floor
    public Vector3 mapSize;

    // Do you want to have a small gap between the tiles ?
    [Range(0, 1)]
    public float outlinePercent;

    List<Coord> allTiledCoords;
    Queue<Coord> shuffledTileCoords;

    void Start()
    {

        //Call the "DeleteBlock" method, after 1 second the game has begun, and keep calling it every 1 second! THE DOOM IS NEAR!
        InvokeRepeating("DeleteBlock", 1, 3.3f);
        //Create a map that we can PLAY!!!
        GenerateMap();

    }

    public void GenerateMap()
    {

        allTiledCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    allTiledCoords.Add(new Coord(x, y, z));
                }


            }
        }

        shuffledTileCoords = new Queue<Coord>(Shuffle.ShuffleArray(allTiledCoords.ToArray(), UnityEngine.Random.Range(0, 10)));



        // Name the Gameobject that holds the floor!
        string holderName = "Generated Map";
        //create the holder for our floor tiles!
        Transform mapHolder = new GameObject(holderName).transform;

        // if exists, destroy it, otherwise create it!
        if (transform.Find(holderName))
        {

            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        mapHolder.parent = transform;

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    //	Calculate the position of the new tile.
                    Vector3 tilePosition = CoordToWorldPosition(x, y, z);
                    Transform newTile = Instantiate(tilePrefab, tilePosition * ScaleModifier, Quaternion.Euler(Vector3.right * -90)) as Transform;//Quaternion.Euler(Vector3.right * 90)
                    newTile.localScale = new Vector3(ScaleModifier, ScaleModifier, ScaleModifier) * (1 - outlinePercent);
                    // Rename it, so we can be aware of which block we are standing at.!
                    newTile.gameObject.name = "Block " + x + " " + y + " " + z;
                    // and make them ALL children of the FloorHandler we created before ^^.
                    newTile.parent = mapHolder;
                }
            }

        }
    }

    public Coord GetRngCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    Vector3 CoordToWorldPosition(int x, int y, int z)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, -mapSize.y / 2 + 0.5f + y, -mapSize.z / 2 + 0.5f + z);
    }

    void DeleteBlock()
    {

        Coord randomCoord = GetRngCoord();
        //Vector3 deleteTerrainPosition = CoordToWorldPosition(randomCoord.x, randomCoord.y);
        // Debug.Log(randomCoord.x.ToString() + " " + randomCoord.y.ToString());

        GameObject deleteOne = GameObject.Find("Block " + randomCoord.x.ToString() + " " + randomCoord.y.ToString() + " " + randomCoord.z.ToString());
       StartCoroutine(deleteOne.GetComponent<TriangleExplosion>().SplitMesh(true));


    }
}
public struct Coord
{
    public int x;
    public int y;
    public int z;

    public Coord(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}