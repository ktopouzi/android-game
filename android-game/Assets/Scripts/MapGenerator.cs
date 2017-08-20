using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //	The prefab used for the floor of the level
    public Transform tilePrefab;

    // A Vector2 for the size of the floor
    public Vector3 mapSize;

    // Do you want to have a small gap between the tiles ?
    [Range(0, 1)]
    public float outlinePercent;

    List<Coord> allTiledCoords;
    Queue<Coord> shuffledTileCoords;
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
                    Transform newTile = Instantiate(tilePrefab, tilePosition * 2f, Quaternion.Euler(Vector3.right * -90)) as Transform;//Quaternion.Euler(Vector3.right * 90)
                    newTile.localScale = new Vector3(2f, 2f, 2f) * (1 - outlinePercent);
                    // Rename it, so we can be aware of which block we are standing at.!
                    newTile.gameObject.name = "Block " + x + " " + y + " " + z;
                    // and make them ALL children of the FloorHandler we created before ^^.
                    newTile.parent = mapHolder;
                }
            }

        }
    }

    Vector3 CoordToWorldPosition(int x, int y, int z)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, -mapSize.y / 2 + 0.5f + y, -mapSize.z / 2 + 0.5f + z);
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
}