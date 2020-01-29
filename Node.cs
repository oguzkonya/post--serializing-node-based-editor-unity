using System;
using UnityEditor;
using UnityEngine;

public class Node
{
    public Rect rect;
    
    [XmlIgnore] public string title;
	[XmlIgnore] public bool isDragged;
	[XmlIgnore] public bool isSelected;

	[XmlIgnore] public ConnectionPoint inPoint;
	[XmlIgnore] public ConnectionPoint outPoint;

	[XmlIgnore] public GUIStyle style;
	[XmlIgnore] public GUIStyle defaultNodeStyle;
	[XmlIgnore] public GUIStyle selectedNodeStyle;

	[XmlIgnore] public Action<Node> OnRemoveNode;
    
    public Node() { }

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, 
    Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode, string inPointID, string outPointID)
    {
        rect = new Rect(position.x, position.y, width, height);
		style = nodeStyle;
		inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, inPointID);
		outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint, outPointID);
		defaultNodeStyle = nodeStyle;
		selectedNodeStyle = selectedStyle;
		OnRemoveNode = OnClickRemoveNode;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 &amp;&amp; isSelected &amp;&amp; rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 &amp;&amp; isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}
