using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RedBlackTree : MonoBehaviour
{
    //modification on transplant(last if)
    //132 one more condition inside if
    //138 one more if

    //bad positioning when adding more nodes of the same key
    [SerializeField] NodeScript root;
    [SerializeField] int totalNodes = 0;
    [SerializeField] float horizontalDistance = 10f;
    [SerializeField] float verticalDistance = 1f;
    [SerializeField] float smoothSpeed = 0.5f;

    public NodeScript Nil;

    [Header("Attributes")]
    [SerializeField] GameObject NodePrefab;
    [SerializeField] Camera camera;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text warningText;
    [SerializeField] Slider horizontalSlider;
    [SerializeField] Slider verticalSlider;
    [SerializeField] TMP_Text statistics;
    [SerializeField] Toggle showNILL;
    bool nillHidden;
    int zeroMask = int.MaxValue;
    [SerializeField]LayerMask hidNillMask;


    bool isSearching = false;
    private void Awake()
    {
        warningText.text = "";
        root = Nil;
    }
    public void InsertRandomNode()
    {
        ResetWarningText();

        int k = UnityEngine.Random.Range(0, 100);
        
        GameObject newNode = Instantiate(NodePrefab, this.transform);
        NodeScript script = newNode.GetComponent<NodeScript>();
        script.Init(k);

        warningText.text += k.ToString() + " inserted (random).\n Steps:\n";
        Insert(script);
        

    }
    public void InsertNode()
    {
        ResetWarningText();
        int key = 0;
        try
        {
            key = int.Parse(inputField.text);
            inputField.text = "";    
        }catch {
            warningText.text = "Error: Invalid input!\n";
                return; 
        }
        
        GameObject newNode = Instantiate(NodePrefab, this.transform);
        NodeScript script = newNode.GetComponent<NodeScript>();
        script.Init(key);
        warningText.text += key.ToString() + " inserted.\n Steps:\n";
        Insert(script);
        
    }
    public void DeleteNode()
    {
        ResetWarningText();
        int key = 0;
        try
        {
            key = int.Parse(inputField.text);
            inputField.text = "";
            warningText.text = key.ToString() + " deleted.\nSteps:\n";
        }
        catch
        {
            warningText.text = "Error: Invalid input!\n";
            return;
        }
        NodeScript node = Search(key);
        if(node == null)
        {
            warningText.text = key.ToString() + " not found.\n";
            return;

        }
        
        Delete(node);
        //GameObject.Destroy(node.gameObject);
       
    }
    public void SearchNode()
    {
        ResetWarningText();
        if (isSearching)
            return;
        //get key
        int key = 0;
        try
        {
            key = int.Parse(inputField.text);
        }
        catch
        {
            warningText.text = "Error: Invalid input!\n";
            return;
        }
        if (root == Nil)
            return;

        isSearching = true;
        StartCoroutine(SearchVisualization(key));
    }
    public void MinNode()
    {
        if (isSearching)
            return;
        isSearching = true;
        StartCoroutine(MinVisualization());
    }
    public void MaxNode()
    {
        if (isSearching)
            return;
        StartCoroutine(MaxVisualization());

    }
    //Tree operations
    #region tree operations
    void Insert(NodeScript z)
    {
        totalNodes++;
        camera.orthographicSize += Mathf.Log(1 + 1/camera.orthographicSize);

        z.leftChild = Nil;
        z.rightChild = Nil;
        z.SetColor(Color.red);

        NodeScript y = null;
        NodeScript x = this.root;
        while (x != Nil)
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

        if (z.parent == null)
        {
            z.SetColor(Color.black);
            warningText.text += "Node " + z.GetKey() + " colored to black.\n";
            return;
        }
        if (z.parent.parent == null)
            return;

        InsertFixup(z);
        //set root position
    }
    void Delete(NodeScript z)
    {
        NodeScript y = z;
        NodeScript x;
        Color yOriginalColor = y.GetColor();
        if(z.leftChild == Nil)
        {
            x = z.rightChild;
            Transplant(z, z.rightChild);
        }
        else if( z.rightChild == Nil)
        {
            x = z.leftChild;
            Transplant(z, z.leftChild);
        }
        else
        {
            y = GetMinimum(z.rightChild);
            yOriginalColor = y.GetColor();
            x = y.rightChild;
            if (y.parent == z)
                x.parent = y;
            else
            {
                Transplant(y, y.rightChild);
                y.rightChild = z.rightChild;
                y.rightChild.parent = y;
            }

            Transplant(z, y);
            y.leftChild = z.leftChild;
            y.leftChild.parent = y;
            y.SetColor(z.GetColor());
        }
        
        if (yOriginalColor == Color.black)
            DeleteFixup(x);

        Destroy(z.gameObject);

    }
    IEnumerator SearchVisualization(int key)
    {
        bool found = false;

        //start from root
        var node = this.root;
        var originalColor = node.GetColor();
        node.SetColor(Color.green, smoothSpeed);
        
        while (true)
        {
            //how we do it
            /* start color the node to green
             * advance to next node
             * pause 0.5s
             * start color the node to green
             * pause 0.5s
             * start color back the prev node
             * 
             */
            yield return new WaitForSeconds(1f);
            var previousNode = node;
            //go to next node
            if (node.GetKey() == key)
            {
                warningText.text = key + " found!\n";
                found = true;
            }

            if (node.GetKey() > key)
                node = node.leftChild;
            else
                node = node.rightChild;
            

            previousNode.SetColor(originalColor, smoothSpeed);

            if (node == Nil)
                break;
            originalColor = node.GetColor();
            node.SetColor(Color.green, smoothSpeed);
            
        }

        if(found == false)
        {
            warningText.text = key + " not found!\n";
        }
        isSearching = false;
        yield return null;
    }
    IEnumerator MinVisualization()
    {
        //start from root
        var node = this.root;
        var originalColor = node.GetColor();
        node.SetColor(Color.green, smoothSpeed);

        while (true)
        {
            //how we do it
            /* start color the node to green
             * advance to next node
             * pause 0.5s
             * start color the node to green
             * pause 0.5s
             * start color back the prev node
             * 
             */
            yield return new WaitForSeconds(1f);
            var previousNode = node;
            //go to next node


            previousNode.SetColor(originalColor, smoothSpeed);
            if (node.leftChild == Nil)
                break;

            node = node.leftChild;

            originalColor = node.GetColor();
            node.SetColor(Color.green, smoothSpeed);
 
        }


        warningText.text = node.GetKey() + " is min key!\n";

        isSearching = false;
        yield return null;
    }
    IEnumerator MaxVisualization()
    {

        //start from root
        var node = this.root;
        var originalColor = node.GetColor();
        node.SetColor(Color.green, smoothSpeed);

        while (true)
        {
            //how we do it
            /* start color the node to green
             * advance to next node
             * pause 0.5s
             * start color the node to green
             * pause 0.5s
             * start color back the prev node
             * 
             */
            yield return new WaitForSeconds(1f);
            var previousNode = node;
            //go to next node


            previousNode.SetColor(originalColor, smoothSpeed);
            if (node.rightChild == Nil)
                break;

            node = node.rightChild;

            originalColor = node.GetColor();
            node.SetColor(Color.green, smoothSpeed);
        }

      
        warningText.text = node.GetKey() + " is max key!\n";
        
        isSearching = false;
        yield return null;
    }


    void InsertFixup(NodeScript z)
    {
        try
        {
            while (z.parent.GetColor() == Color.red)
            {
                if (z.parent == z.parent.parent.leftChild)
                {

                    NodeScript y = z.parent.parent.rightChild;

                    if (y.GetColor() == Color.red)
                    {
                        z.parent.SetColor(Color.black); warningText.text += "Node " + z.parent.GetKey() + " colored to black.\n";
                        y.SetColor(Color.black); warningText.text += "Node " + y.GetKey() + " colored to black.\n";
                        z.parent.parent.SetColor(Color.red); warningText.text += "Node " + z.parent.parent.GetKey() + " colored to red.\n";
                        z = z.parent.parent;
                    }
                    else
                    {
                        if (z == z.parent.rightChild)
                        {
                            z = z.parent;
                            LeftRotate(z);
                        }
                        z.parent.SetColor(Color.black); warningText.text += "Node " + z.parent.GetKey() + " colored to black.\n";
                        z.parent.parent.SetColor(Color.red); warningText.text += "Node " + z.parent.parent.GetKey() + " colored to red.\n";
                        RightRotate(z.parent.parent);
                    }
                }
                else
                {
                    NodeScript y = z.parent.parent.leftChild;
                    if (y.GetColor() == Color.red)
                    {
                        z.parent.SetColor(Color.black); warningText.text += "Node " + z.parent.GetKey() + " colored to black.\n";
                        y.SetColor(Color.black); warningText.text += "Node " + y.GetKey() + " colored to black.\n";
                        z.parent.parent.SetColor(Color.red); warningText.text += "Node " + z.parent.parent.GetKey() + " colored to red.\n";
                        z = z.parent.parent;
                    }
                    else
                    {
                        if (z == z.parent.leftChild)
                        {
                            z = z.parent;
                            RightRotate(z);
                        }
                        z.parent.SetColor(Color.black); warningText.text += "Node " + z.parent.GetKey() + " colored to black.\n";

                        z.parent.parent.SetColor(Color.red); warningText.text += "Node " + z.parent.parent.GetKey() + " colored to red.\n";
                        LeftRotate(z.parent.parent);
                    }
                }
            }
        }
        catch { }

        if(root.GetColor() != Color.black)
            warningText.text += "Root (" + root.GetKey() + ") colored to black.\n";
        root.SetColor(Color.black); 
    }
    void DeleteFixup(NodeScript x)
    {
            while (x != root && x.GetColor() == Color.black)
            {
                 if (x.parent == null)
                     break;
                if (x == x.parent.leftChild)
                {
                    NodeScript w = x.parent.rightChild;
                    if (w.GetColor() == Color.red)
                    {
                        w.SetColor(Color.black);        warningText.text += "Node " + w.GetKey() + " colored to black.\n";
                        x.parent.SetColor(Color.red);   warningText.text += "Node " + x.parent.GetKey() + " colored to black.\n";
                        LeftRotate(x.parent);
                        w = x.parent.rightChild;
                    }
                    if (w.leftChild.GetColor() == Color.black && w.rightChild.GetColor() == Color.black)
                    {
                        w.SetColor(Color.red);                   warningText.text += "Node " + w.GetKey() + " colored to red.\n";
                        x = x.parent;
                    }
                    else
                    {
                        if (w.rightChild.GetColor() == Color.black)
                        {
                            w.leftChild.SetColor(Color.black);      warningText.text += "Node " + w.leftChild.GetKey() + " colored to black.\n";
                            w.SetColor(Color.red);                  warningText.text += "Node " + w.GetKey() + " colored to red.\n";
                            RightRotate(w);
                            w = x.parent.rightChild;
                        }
                        w.SetColor(x.parent.GetColor());        warningText.text += "Node " + w.GetKey() + " colored to " + x.parent.GetColor().ToString() + ".\n";
                        x.parent.SetColor(Color.black);          warningText.text += "Node " + x.parent.GetKey() + " colored to black.\n";
                        w.rightChild.SetColor(Color.black);      warningText.text += "Node " + w.rightChild.GetKey() + " colored to black.\n";
                        LeftRotate(x.parent);
                        x = root;
                    }
                }
                else
                {
                    NodeScript w = x.parent.leftChild;
                    if (w.GetColor() == Color.red)
                    {
                        w.SetColor(Color.black);              warningText.text += "Node " + w.GetKey() + " colored to black.\n";
                        x.parent.SetColor(Color.red);         warningText.text += "Node " + x.parent.GetKey() + " colored to black.\n";
                        RightRotate(x.parent);
                        w = x.parent.leftChild;
                    }
                    if (w.rightChild.GetColor() == Color.black && w.leftChild.GetColor() == Color.black)
                    {
                        w.SetColor(Color.red);              warningText.text += "Node " + w.GetKey() + " colored to red.\n";
                        x = x.parent;
                    }
                    else
                    {
                        if (w.leftChild.GetColor() == Color.black)
                        {
                            w.rightChild.SetColor(Color.black);        warningText.text += "Node " + w.rightChild.GetKey() + " colored to black.\n";
                            w.SetColor(Color.red);                      warningText.text += "Node " + w.GetKey() + " colored to red.\n";
                            LeftRotate(w);
                            w = x.parent.leftChild;
                        }
                        w.SetColor(x.parent.GetColor());                 warningText.text += "Node " + w.GetKey() + " colored to " + x.parent.GetColor().ToString() + ".\n";
                        x.parent.SetColor(Color.black);                   warningText.text += "Node " + x.parent.GetKey() + " colored to black.\n";
                        w.leftChild.SetColor(Color.black);              warningText.text += "Node " + w.leftChild.GetKey() + " colored to black.\n";
                        RightRotate(x.parent);
                        x = root;
                    }
                }


            }
      
        x.SetColor(Color.black); warningText.text += "Node " + x.GetKey() + " colored to black.\n";
    }

    void LeftRotate(NodeScript x)
    {
        warningText.text += "Node " + x.GetKey() + " rotated to left.\n";
        NodeScript y = x.rightChild;

        x.rightChild = y.leftChild;
        if (y.leftChild != Nil)
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
        warningText.text += "Node " + y.GetKey() + " rotated to right.\n";
        NodeScript x = y.leftChild;
       y.leftChild = x.rightChild;
       if (x.rightChild != Nil)
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

    NodeScript GetMinimum(NodeScript x)
    {
        while (x.leftChild != Nil)
            x = x.leftChild;
        return x;
    }

    NodeScript Search(int k)
    {
        NodeScript node = root;
        while(node != Nil)
        {
            if (node.GetKey() == k)
                return node;
            else if (node.GetKey() > k)
                node = node.leftChild;
            else if(node.GetKey() < k)
                node = node.rightChild;
        }
        return null;
    }
    void Transplant(NodeScript u, NodeScript v)
    {
        if (u.parent == null)
            root = v;
        else if (u == u.parent.leftChild)
            u.parent.leftChild = v;
        else
            u.parent.rightChild = v;
        if(v != null)
            v.parent = u.parent;
    }
    int GetBlackHeight()
    {
        int height = 0;
        NodeScript node = root;
        while (node != Nil)
        {
            node = node.leftChild;
            if(node.GetColor() == Color.black)
                height++;
        }
        return height;
    }
    public void Clear()
    {
        foreach (Transform child in transform)
        {
            if(child.CompareTag("Node"))
                GameObject.Destroy(child.gameObject);
        }
        root = Nil;
        totalNodes = 0;
        verticalSlider.value = 1f;
        horizontalSlider.value = 10f;
        camera.orthographicSize = 6f;
    }

    #endregion


    private void Update()
    {
        UpdateLocalPositionsOfAllNodes(root);
        UpdateTransformOfAllNodes(root);
        UpdateTransformOfNill();
        MakeStatistic();
    }

    void UpdateLocalPositionsOfAllNodes(NodeScript rt, int deep = 0)
    {
        //on levels
        if(rt == null || rt == Nil) return;

        if (rt == root)
            rt.rbNodeLocalPosition = transform.position;
        else if (rt.IsLeftChild())
            rt.rbNodeLocalPosition = rt.parent.rbNodeLocalPosition + new Vector3(Mathf.Log(totalNodes) * (-horizontalDistance) * (1f / Mathf.Pow((deep + 1), 1.95f)), -verticalDistance, 0);
        else rt.rbNodeLocalPosition = rt.parent.rbNodeLocalPosition + new Vector3(Mathf.Log(totalNodes) * horizontalDistance * (1f / Mathf.Pow((deep + 1),1.95f)), -verticalDistance, 0);

        UpdateLocalPositionsOfAllNodes(rt.leftChild,deep+1);
        UpdateLocalPositionsOfAllNodes(rt.rightChild,deep+1);
    }
  
    void UpdateTransformOfAllNodes(NodeScript rt)
    {
        //here is applied the smooth transition
        if (rt == null || rt == Nil) return;

        rt.transform.position = Vector3.Lerp(rt.transform.position, rt.rbNodeLocalPosition, Time.deltaTime * smoothSpeed);
        UpdateTransformOfAllNodes(rt.leftChild);
        UpdateTransformOfAllNodes(rt.rightChild);
        ColorLinesOfNode(rt);
    }
    void UpdateTransformOfNill()
    {
        Nil.transform.localPosition = new Vector3(Nil.transform.localPosition.x, (-8f * (1+Mathf.Log(verticalDistance,2f))) - Mathf.Log(totalNodes + 1) * verticalDistance, Nil.transform.localPosition.z);
    }
    public void ShowNILL()
    {
        if(showNILL.isOn)
        {
            camera.cullingMask = zeroMask;
        }
        else
        {
            camera.cullingMask = hidNillMask;
        }
        //UnityEngine.Debug.Log(camera.cullingMask);
    }
    void MakeStatistic()
    {
        string stat = "Developed by kbRadu\n";
        stat += "Total nodes: " + totalNodes;
        stat += "\nBlack height: " + GetBlackHeight().ToString();
        statistics.text = stat;
    }

    //Other
    void ColorLinesOfNode(NodeScript node)
    {
        //Color
        var line = node.GetComponent<LineRenderer>();
        line.positionCount = 5;
        line.SetPosition(0, node.transform.position);
        int index = 1;
        if (node.leftChild != null)
        {
            if (node.leftChild != Nil || (node.leftChild == Nil && showNILL.isOn))
            {
                line.SetPosition(index++, node.leftChild.transform.position);
                line.SetPosition(index++, node.transform.position);
            }
        }
        if (node.rightChild != null)
        {
            if (node.rightChild != Nil || (node.rightChild == Nil && showNILL.isOn))
            {
                line.SetPosition(index++, node.rightChild.transform.position);
                line.SetPosition(index++, node.transform.position);
            }
        }
        for (int i = index; i < 5; i++)
        {
            line.SetPosition(i, node.transform.position);
        }
    }    
    public void ChangeHorizontalDistance()
    {
        camera.orthographicSize += (horizontalSlider.value - horizontalDistance)*0.5f;
        horizontalDistance = horizontalSlider.value;
    }
    public void ChangeVerticalDistance()
    {
        camera.orthographicSize += (verticalSlider.value - verticalDistance) * 5f;
        verticalDistance = verticalSlider.value;
    }
    void ResetWarningText()
    {
        warningText.text = string.Empty;
    }
}
