using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
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

    public Shader shader;
    Color colorAfterLerping;
    
    public NodeScript(bool isNil)
    {
        if(isNil)
        {
            colorAfterLerping = Color.black;
            leftChild = null;
            rightChild = null;
        }
    }
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
        //SetColorTo(col);
        LerpColorTo(col);
    }
    void SetColorTo(Color col)
    {
        if (col == Color.red)
            GetComponent<MeshRenderer>().sharedMaterial = redMaterial;
        else if (col == Color.black)
            GetComponent<MeshRenderer>().sharedMaterial = blackMaterial;
        else if (col == Color.green)
            GetComponent<MeshRenderer>().sharedMaterial = greenMaterial;
    }
    void LerpColorTo(Color col, float smoothness = 0.75f)
    {
        colorAfterLerping = col;
        StartCoroutine(LerpingColor(col, smoothness));
    }
    IEnumerator LerpingColor(Color col, float smoothness)
    {
        //here the shared material might be a non valid material
        // so check if is valid, if not, set the from material using  the colorafterLerping
        Material currentMaterial = GetComponent<MeshRenderer>().sharedMaterial;
       
        Material from = currentMaterial;
        if (!IsRedOrBlack(currentMaterial))
            from = GetMaterialOfColor(colorAfterLerping);

        Material to = GetMaterialOfColor(col);

        //Use lerpable material to transitionate smoothly between a color and another
        Material lerpableMaterial = new Material(shader);
        lerpableMaterial.color = from.color;
        GetComponent<MeshRenderer>().sharedMaterial = lerpableMaterial;
        while(lerpableMaterial.color != to.color)
        {
            lerpableMaterial.color = UnityEngine.Color.Lerp(lerpableMaterial.color, to.color, Time.deltaTime * smoothness);
            yield return new WaitForSeconds(0.001f);
        }
        colorAfterLerping = Color.UNKNOWN;

        yield return null;
    }
    public Color GetColor()
    {
        Material sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        if (sharedMaterial == null)
            return colorAfterLerping;
        if (sharedMaterial.color == redMaterial.color)
            return Color.red;

        if (sharedMaterial.color == blackMaterial.color)
            return Color.black;

        if (sharedMaterial.color == greenMaterial.color)
            return Color.green;

        //case while is lerping
        return colorAfterLerping;

    }
    Material GetMaterialOfColor(Color col)
    {
        if (col == Color.red) return redMaterial;

        if(col == Color.black) return blackMaterial;

        if(col == Color.green) return greenMaterial;

        return null;
    }
    bool IsRedOrBlack(Material mat)
    {
        if (mat.color == redMaterial.color)
            return true;
        if(mat.color == blackMaterial.color) 
            return true;

        return false;
    }
}
public enum Color
{
    red,
    black,
    green,
    UNKNOWN
}
