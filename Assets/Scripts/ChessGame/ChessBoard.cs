using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [SerializeField] public Material tileMaterial;
    [SerializeField] public Material hoverMaterial;
    [SerializeField] public Material possibleMoveMaterial;

    public int TILE_COUNT_X = 8;
    public int TILE_COUNT_Y = 8;
    public float yOffset = 0.0006644645f;
    public GameObject[,] tiles;
    //private Camera currentCamera;

    #region Unity Builtin Methods
    void Awake()
    {
        this.generateGrid(1, TILE_COUNT_X, TILE_COUNT_Y);
    }

    #endregion

    #region generateBoard
    /** Generates A tile Grid
     * 
     */
    private void generateGrid(int tileSize, int tileCountX, int tileCountY )
    {
        tiles = new GameObject[tileCountX, tileCountY];

        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = this.generateTile(tileSize, x, y);
            }
        }
    }

    /** Generates one tile
     * 
     */ 
    private GameObject generateTile(int tileSize, int x, int y)
    {
        GameObject tileObj = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObj.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObj.AddComponent<MeshFilter>().mesh = mesh;
        tileObj.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, 0, y * tileSize);
        vertices[1] = new Vector3(x * tileSize, 0, (y+1) * tileSize);
        vertices[2] = new Vector3((x+1) * tileSize, 0, y * tileSize);
        vertices[3] = new Vector3((x+1) * tileSize, 0, (y+1) * tileSize);

        int[] quadTri = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = quadTri;
        mesh.RecalculateNormals();

        tileObj.AddComponent<BoxCollider>();

        return tileObj;
    }
    #endregion

    /** Gets the tileindex that is being hit by the ray
     * 
     */
    public Vector2Int lookupTileIndex(RaycastHit hitInfo)
    {
        GameObject tile = hitInfo.transform.gameObject;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == tile)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;
    }
}