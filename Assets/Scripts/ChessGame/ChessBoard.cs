using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material hoverMaterial;

    [SerializeField] Camera currentCamera;
    //Logic
    private const int TILE_COUNTX = 8;
    private const int TILE_COUNTY = 8;
    private GameObject[,] tiles;
    //private Camera currentCamera;
    private Vector2Int currentHover;

    #region Unity Builtin Methods
    void Awake()
    {
        this.generateGrid(1, TILE_COUNTX, TILE_COUNTY);
    }

    void Update()
    {
        if(!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        Ray ray = new Ray();
        switch (SystemInfo.deviceType)
        {
            case DeviceType.Unknown:
                break;
            case DeviceType.Handheld:
                ray = currentCamera.ScreenPointToRay(Input.GetTouch(0).position);
                break;
            case DeviceType.Console:
                break;
            case DeviceType.Desktop:
                ray = currentCamera.ScreenPointToRay(Input.mousePosition);
                break;
            default:
                break;
        }

        this.handleInputsTransitions(ray);
    }
    #endregion

    /** Handles selection of Transitions for Mouse
     * 
     */
    private void handleInputsTransitions(Ray ray)
    {
        RaycastHit info;

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
        {
            Vector2Int hitPos = this.lookupTileIndex(info);

            //Transition from not hovering to hover
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPos;
                tiles[currentHover.x, currentHover.y].GetComponent<Renderer>().sharedMaterial = this.hoverMaterial;
            }

            //Transition from Hovering one tile to the next one
            if (currentHover != hitPos)
            {
                //On Leave
                tiles[currentHover.x, currentHover.y].GetComponent<Renderer>().sharedMaterial = this.tileMaterial;
                currentHover = hitPos;
                //OnEnter
                tiles[currentHover.x, currentHover.y].GetComponent<Renderer>().sharedMaterial = this.hoverMaterial;
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                //On Leave
                tiles[currentHover.x, currentHover.y].GetComponent<Renderer>().sharedMaterial = this.tileMaterial;
                currentHover = -Vector2Int.one;
            }
        }
    }

    /** Gets the tileindex that is being hit by the ray
     * 
     */
    private Vector2Int lookupTileIndex(RaycastHit hitInfo)
    {
        GameObject tile = hitInfo.transform.gameObject;

        for (int x = 0; x < TILE_COUNTX; x++)
        {
            for (int y = 0; y < TILE_COUNTY; y++)
            {
                if (tiles[x, y] == tile)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;
    }

    #region generateBoard Methods
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

        //TODO
        int[] quadTri = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = quadTri;
        mesh.RecalculateNormals();

        tileObj.layer = LayerMask.NameToLayer("Tile"); 
        tileObj.AddComponent<BoxCollider>();

        return tileObj;
    }
    #endregion

}
