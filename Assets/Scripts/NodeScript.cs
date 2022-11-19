using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    [Header("Node Properties")]
    [SerializeField] int key;
    public NodeScript parent = null;
    public NodeScript leftChild = null;
    public NodeScript rightChild = null;

    [Header("GameObject Properties")]
    public Vector3 rbNodeLocalPosition; //The object is moved to it's newest position using this location
    //object local position is separate

    [Header("Attributes")]
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material greenMaterial;
    
    public void Init(int k)
    {
        keyText = transform.GetChild(0).GetComponent<TMP_Text>();

        parent = null;
        leftChild = null;
        rightChild = null;
        SetKey(k);
        SetColor(Color.red);
    }
    public bool IsLeftChild()
    {
        try
        {
            if (this.parent.leftChild == this)
                return true;
        }
        catch { }

        return false;
    }
    public void SetKey(int k)
    {
        key = k;
        if (keyText != null)
             keyText.text = k.ToString();
    }
    public int GetKey ()
    { return key; }
    public void SetColor(Color col)
    {
        if (col == Color.red)            
            GetComponent<MeshRenderer>().material = redMaterial;
        else if (col == Color.black)
            GetComponent<MeshRenderer>().material = blackMaterial;
        else if (col == Color.green)
            GetComponent<MeshRenderer>().material = greenMaterial;
    }
    public Color GetColor()
    {
        if (GetComponent<MeshRenderer>().sharedMaterial == redMaterial)
            return Color.red;

        if (GetComponent<MeshRenderer>().sharedMaterial == blackMaterial)
            return Color.black;

        if (GetComponent<MeshRenderer>().sharedMaterial == greenMaterial)
            return Color.green;

        return Color.UNKNOWN;
    }
}
public enum Color
{
    red,
    black,
    green,
    UNKNOWN
}
