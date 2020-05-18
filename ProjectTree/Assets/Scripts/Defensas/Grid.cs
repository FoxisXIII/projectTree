using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]private int _width;
    [SerializeField]private int _height;
    [SerializeField]private float _cellSize;
    [SerializeField] private LayerMask _obstacleLayer;

    public Vector3 GetNearestpointOnGrid(Vector3 position)
    {
        int xCount = Mathf.RoundToInt(position.x / _cellSize);
        int zCount = Mathf.RoundToInt(position.z / _cellSize);
        
        Vector3 possibleResult = new Vector3(xCount * _cellSize, 0, zCount * _cellSize);
        
        if (!Physics.CheckSphere(possibleResult, 1f, _obstacleLayer))
        {
            return possibleResult;
        }
        
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (float x = 0; x < _width; x+=_cellSize)
        {
            for (float y = 0; y < _height; y+=_cellSize)
            {
                var point = GetNearestpointOnGrid(new Vector3(transform.position.x+x, 0, transform.position.z+y));
                if (!Physics.CheckSphere(point, 1f, _obstacleLayer))
                {
                    Gizmos.DrawCube(point, Vector3.one/1.5f);
                }
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,y)*_cellSize;
    }
}
