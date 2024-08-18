using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialoguePort : Port
{

    public enum LinkTypes   
    {
        NONE,
        NEED_ITEM,
        NEED_QUEST,
        GIVE_QUEST,
        GIVE_ITEM,
        GIVE_ATTITUDE
    }

    public LinkTypes ThisLink; 

    public string LinkID; 

    public DialoguePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
    {
    }

    public override bool canGrabFocus => base.canGrabFocus;

    public override FocusController focusController => base.focusController;

    public override VisualElement contentContainer => base.contentContainer;

    public override string title { get => base.title; set => base.title = value; }

    public override bool showInMiniMap => base.showInMiniMap;

    public override IEnumerable<Edge> connections => base.connections;

    public override bool connected => base.connected;

    public override bool collapsed => base.collapsed;

    public override void Blur()
    {
        base.Blur();
    }

    public override void Connect(Edge edge)
    {
        base.Connect(edge);
    }

    public override bool ContainsPoint(Vector2 localPoint)
    {
        return base.ContainsPoint(localPoint);
    }

    public override void Disconnect(Edge edge)
    {
        base.Disconnect(edge);
    }

    public override void DisconnectAll()
    {
        base.DisconnectAll();
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override Vector3 GetGlobalCenter()
    {
        return base.GetGlobalCenter();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override Rect GetPosition()
    {
        return base.GetPosition();
    }

/*    public override void HandleEvent(EventBase evt)
    {
        base.HandleEvent(evt);
    }*/

    public override bool HitTest(Vector2 localPoint)
    {
        return base.HitTest(localPoint);
    }

    public override bool IsAscendable()
    {
        return base.IsAscendable();
    }

    public override bool IsCopiable()
    {
        return base.IsCopiable();
    }

    public override bool IsDroppable()
    {
        return base.IsDroppable();
    }

    public override bool IsMovable()
    {
        return base.IsMovable();
    }

    public override bool IsRenamable()
    {
        return base.IsRenamable();
    }

    public override bool IsResizable()
    {
        return base.IsResizable();
    }

    public override bool IsSelectable()
    {
        return base.IsSelectable();
    }

    public override bool IsSelected(VisualElement selectionContainer)
    {
        return base.IsSelected(selectionContainer);
    }

    public override void OnSelected()
    {
        base.OnSelected();
    }

    public override void OnStartEdgeDragging()
    {
        base.OnStartEdgeDragging();
    }

    public override void OnStopEdgeDragging()
    {
        base.OnStopEdgeDragging();
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
    }

    public override bool Overlaps(Rect rectangle)
    {
        return base.Overlaps(rectangle);
    }

    public override void Select(VisualElement selectionContainer, bool additive)
    {
        base.Select(selectionContainer, additive);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void Unselect(VisualElement selectionContainer)
    {
        base.Unselect(selectionContainer);
    }

    public override void UpdatePresenterPosition()
    {
        base.UpdatePresenterPosition();
    }

    protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
    {
        return base.DoMeasure(desiredWidth, widthMode, desiredHeight, heightMode);
    }

    protected override void ExecuteDefaultAction(EventBase evt)
    {
        base.ExecuteDefaultAction(evt);
    }

    protected override void ExecuteDefaultActionAtTarget(EventBase evt)
    {
        base.ExecuteDefaultActionAtTarget(evt);
    }

    protected override void OnCustomStyleResolved(ICustomStyle styles)
    {
        base.OnCustomStyleResolved(styles);
    }
}
