using System;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class Connection
{
    public ConnectionPoint OutPoint;
    public ConnectionPoint InPoint;
    public Action<Connection> OnClickRemoveConnection;

    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        this.OutPoint = inPoint;
        this.InPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            OutPoint.rect.center,
            InPoint.rect.center,
            OutPoint.rect.center + Vector2.left * 50f,
            InPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        if (Handles.Button((OutPoint.rect.center + InPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}

#endif