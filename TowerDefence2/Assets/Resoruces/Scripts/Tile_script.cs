using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Tile_script : MonoBehaviour
{
    GameManager gameManager_script;
    GameObject Tower;
    public bool isOcupied = false;

    public Vector3Int DenyRedRGB = new Vector3Int(250, 150, 150);
    public Vector3Int PermitGreenRGB = new Vector3Int(100, 250, 100);
    public Vector3Int DefaultGreenRGB = new Vector3Int(70, 200, 70);

    private void Awake()
    {
        GameObject GameManager = GameObject.Find("GameManager");
        gameManager_script = GameManager.GetComponent<GameManager>();
    }

    public Vector3 mousePosOverTile;



    private void OnMouseDown()
    {

    }

    private void OnMouseUp()
    {
        if (Tower != null)
        {
            if (this.gameObject.tag == "nature" && isOcupied == false)
            {
                Tower.GetComponent<Tower_script>().TryTowerPlacement();
            }
        }
        
    }

    private void OnMouseOver()
    {
        mousePosOverTile = GetMouseWorldPos();
        gameManager_script.mousePosOverTile = mousePosOverTile;
    }
    private Vector3 GetMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        Vector3 worldPos = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            worldPos = ray.GetPoint(distance);
        }

        return new Vector3(worldPos.x, 0.2f, worldPos.z);
    }


    public void ChangeGrassColor(Vector3 RGB)
    {
        Color colorChanged = new Color(RGB.x / 255f, RGB.y / 255f, RGB.z / 255f);
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material[] newMaterials = new Material[meshRenderer.materials.Length];
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            newMaterials[i] = new Material(meshRenderer.materials[i]);
        }

        Material[] materials = meshRenderer.materials;
        foreach(Material material in materials)
        {
            if (material.name == "grass (Instance)")
            {
                material.color = colorChanged;
            }
        }
        meshRenderer.materials = materials;
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "tower")
        {
            Tower = other.gameObject;
            other.gameObject.GetComponent<Tower_script>().Tile = gameObject;

            if (this.gameObject.tag == "nature" && isOcupied == false)
            {
                ChangeGrassColor(PermitGreenRGB);
            }
            else
            {
                ChangeGrassColor(DenyRedRGB);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "tower" && other.GetComponent<Tower_script>().dragging)
        {
            ChangeGrassColor(DefaultGreenRGB);
        }
    }

}
