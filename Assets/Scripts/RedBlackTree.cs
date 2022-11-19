using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class RedBlackTree : MonoBehaviour
{
    [SerializeField] NodeScript root = null;
    [SerializeField] int totalNodes = 0;
    [SerializeField] float distanceBetweenNodes = 1f;
    [Header("Attributes")]
    [SerializeField] GameObject NodePrefab;
    [SerializeField] Camera camera;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text warningText;
    [SerializeField] Slider slider;

 
    //error 224, it says rooot is null but it is not

    public void InsertNode()
    {
        int key = 0;
        try
        {
            key = int.Parse(inputField.text);
            inputField.text = "";
            warningText.text = key.ToString() + " inserted.";
        }catch {
            warningText.text = "Error: Invalid input!";
                return; 
        }
        
        GameObject newNode = Instantiate(NodePrefab, this.transform);
        NodeScript script = newNode.GetComponent<NodeScript>();
        script.Init(key);
        Insert(script);
    }
    public void DeleteNode()
    {

        int key = 0;
        try
        {
            key = int.Parse(inputField.text);
            inputField.text = "";
        }
        catch
        {
            warningText.text = "Error: Invalid input!";
            return;
        }
        //Find Node
        //delete node
        //delete the gameobject
        //fixup
        return;
        

        warningText.text = key.ToString() + " deleted.";
    }
    //Tree operations
    void Insert(NodeScript z)
    {
        totalNodes++;
        camera.orthographicSize += Mathf.Log(1 + 1/camera.orthographicSize); 
        NodeScript y = null;
        NodeScript x = root;
        while (x != null)
        {
            y = x;
            x = (z.GetKey() < x.GetKey()) ? x.leftChild : x.rightChild;
        }
        z.parent = y;
        if (y == null)
            root = z;
        else if (z.GetKey() < y.GetKey())
            y.leftChild = z;
        else
            y.rightChild = z;

        z.leftChild = null;
        z.rightChild = null;
        z.SetColor(Color.red);
            InsertFixup(z);
        //set root position
    }
    NodeScript Delete(NodeScript z)
    {
        NodeScript y = (z.leftChild == null || z.rightChild == null) ? z : GetSuccessor(z);
        NodeScript x = y.leftChild != null ? y.leftChild : y.rightChild;
        x.parent = y.parent;
        if (y.parent == null)
        {
            root = x;
        }
        else
        {
            if (y == y.parent.leftChild)
            {
                y.parent.leftChild = x;
            }
            else
                y.parent.rightChild = x;
        }
        if (y != z)
        {
            z.SetKey(y.GetKey());
        }
        if (y.GetColor() == Color.black)
            DeleteFixup(x);
        return y;
    }

    void InsertFixup(NodeScript z)
    {
        try
        {
            while (z.parent.GetColor() == Color.red)
            {
                    var hasGrandParent = z.parent.parent == null ? null : z.parent.parent.leftChild;
                    if (z.parent == hasGrandParent)
                    {
                        NodeScript y = null;
                        if (hasGrandParent != null)
                            y = z.parent.parent.rightChild;
                       
                        if (y != null && y.GetColor() == Color.red)
                        {
                            z.parent.SetColor(Color.black);
                            y.SetColor(Color.black);
                            z.parent.parent.SetColor(Color.red);
                            z = z.parent.parent;
                        }
                        else
                        {
                            var compareto = z.parent == null ? null : z.parent.rightChild;
                            if (z == compareto)
                            {
                                z = z.parent;
                                LeftRotate(z);
                            }
                            z.parent.SetColor(Color.black);
                            z.parent.parent.SetColor(Color.red);
                            RightRotate(z.parent.parent);
                        }
                    }
                    else
                    {
                         NodeScript y = null;
                         if (hasGrandParent != null)
                             y = z.parent.parent.leftChild;
                         if (y != null && y.GetColor() == Color.red)
                        {
                            z.parent.SetColor(Color.black);
                            y.SetColor(Color.black);
                            z.parent.parent.SetColor(Color.red);
                            z = z.parent.parent;
                        }
                        else
                        {
                            var compareto = z.parent == null?null:z.parent.leftChild;
                            if (z == compareto)
                            {
                                z = z.parent;
                                RightRotate(z);
                            }
                            z.parent.SetColor(Color.black);

                            z.parent.parent.SetColor(Color.red);
                            LeftRotate(z.parent.parent);
                        }
                    }
            }
        }
        catch (System.Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            UnityEngine.Debug.Log("[InsertFix]Node: " + z.GetKey() + " -> Exit line: " + line);

        }
        root.SetColor(Color.black);
    }
    void DeleteFixup(NodeScript x)
    {

    }

    void LeftRotate(NodeScript x)
    {
       
        NodeScript y = x.rightChild;

        x.rightChild = y.leftChild;
        if (y.leftChild != null)
            y.leftChild.parent = x;

        y.parent = x.parent;
        if (x.parent == null)
            root = y;
        else if (x == x.parent.leftChild)
            x.parent.leftChild = y;
        else
            x.parent.rightChild = y;

        y.leftChild = x;
        x.parent = y;
       
    }
    void RightRotate(NodeScript y)
    {
       
       NodeScript x = y.leftChild;
       y.leftChild = x.rightChild;
       if (x.rightChild != null)
           x.rightChild.parent = y;

       x.parent = y.parent;
       if (y.parent == null)
           root = x;
       else if (y == y.parent.rightChild)
           y.parent.rightChild = x;
       else
           y.parent.leftChild = x;

       x.rightChild = y;
       y.parent = x;
       
       
    }

    NodeScript GetSuccessor(NodeScript x)
    {
        if (x == null)
            return x;

        if (x.rightChild != null)
            return GetMinimum(x.rightChild);

        NodeScript y = x.parent;
        while (y != null && x == y.rightChild)
        {
            x = y;
            y = y.parent;
        }
        return y;
    }
    NodeScript GetPredecessor(NodeScript x)
    {
        if (x == null) return x;

        if (x.leftChild != null)
            return GetMaximum(x.leftChild);

        NodeScript y = x.parent;
        while (y != null && x == y.leftChild)
        {
            x = y;
            y = y.parent;
        }
        return y;
    }

    NodeScript GetMinimum(NodeScript x)
    {
        while (x.leftChild != null)
            x = x.leftChild;
        return x;
    }
    NodeScript GetMaximum(NodeScript x)
    {
        while (x.rightChild != null)
            x = x.rightChild;
        return x;
    }


    private void Update()
    {
        UpdateLocalPositionsOfAllNodes(root);
        UpdateTransformOfAllNodes(root);
    }

    void UpdateLocalPositionsOfAllNodes(NodeScript rt, int deep = 0)
    {
        //on levels
        if(rt == null) return;

        if (rt == root)
            rt.rbNodeLocalPosition = transform.position;
        else if (rt.IsLeftChild())
            rt.rbNodeLocalPosition = rt.parent.rbNodeLocalPosition + new Vector3(Mathf.Log(totalNodes) * (-distanceBetweenNodes) * (1f / Mathf.Pow((deep + 1), 1.95f)), -1, 0);
        else rt.rbNodeLocalPosition = rt.parent.rbNodeLocalPosition + new Vector3(Mathf.Log(totalNodes) * distanceBetweenNodes * (1f / Mathf.Pow((deep + 1),1.95f)), -1, 0);

        UpdateLocalPositionsOfAllNodes(rt.leftChild,deep+1);
        UpdateLocalPositionsOfAllNodes(rt.rightChild,deep+1);
    }
    void UpdateTransformOfAllNodes(NodeScript rt)
    {
        //here is applied the smooth transition
        if (rt == null)
            return;

        rt.transform.position = rt.rbNodeLocalPosition;
        UpdateTransformOfAllNodes(rt.leftChild);
        UpdateTransformOfAllNodes(rt.rightChild);
        ColorLinesOfNode(rt);
    }
    void ColorLinesOfNode(NodeScript node)
    {
        //Color
        var line = node.GetComponent<LineRenderer>();
        line.positionCount = 5;
        line.SetPosition(0, node.transform.position);
        int index = 1;
        if (node.leftChild != null)
        {
            line.SetPosition(index++, node.leftChild.transform.position);
            line.SetPosition(index++, node.transform.position);
        }
        if (node.rightChild != null)
        {
            line.SetPosition(index++, node.rightChild.transform.position);
            line.SetPosition(index++, node.transform.position);
        }
        for (int i = index; i < 5; i++)
        {
            line.SetPosition(i, node.transform.position);
        }
    }    
    public void ChangeDistanceBetweenNodes()
    {
        camera.orthographicSize += (slider.value - distanceBetweenNodes)*0.5f;
        distanceBetweenNodes = slider.value;
    }
}
