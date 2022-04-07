using System;
using System.Collections.Generic;
using mattatz.Triangulation2DSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class CPULevelGenerator
{
    private float _padding;
    private Vector2 _lineLengthRange;
    private Vector2 _lineAngleRange;
    private float _width;
    private Vector2 _planeSize;
    
    private List<Vector2> _pathPoints;
    
    private MeshFilter meshFilterL;
    private MeshFilter meshFilterR;

    internal static List<Vector2> CreateLevel(LevelModel model, MeshFilter leftMeshFilter, MeshFilter rightMeshFilter)
    {
        CPULevelGenerator generator = new CPULevelGenerator();
        return generator.GenerateLevel(model, leftMeshFilter, rightMeshFilter);
    }

    private List<Vector2> GenerateLevel(LevelModel model, MeshFilter leftMeshFilter, MeshFilter rightMeshFilter)
    {
        _padding = model.Padding;
        _lineLengthRange = model.LineLengthRange;
        _lineAngleRange = model.LineAngleRange;
        _width = model.LineWidth;
        _planeSize = model.PlaneSize;
        
        meshFilterL = leftMeshFilter;
        meshFilterR = rightMeshFilter;

        GeneratePath();
        CreateMesh();

        return _pathPoints;
    }

    private void GeneratePath()
    {
        try
        {
            _pathPoints = new List<Vector2>();
            Vector2 minMeshPoint = new Vector2(-_planeSize.x * .5f, -_planeSize.y * .5f);
            Vector2 maxMeshPoint = new Vector2(_planeSize.x * .5f, _planeSize.y * .5f);
            if (_planeSize.x < _padding * 2 + _width)
            {
                throw new ArgumentException(
                    $"Padding and Width values are too big for this mesh: Padding = {_padding} Width = {_width}");
            }

            Vector2 workingWidth = new Vector2(minMeshPoint.x + _padding + _width * .5f,
                maxMeshPoint.x - _padding - _width * .5f);
            Vector2 newPoint =
                new Vector2(Random.Range(workingWidth.x, workingWidth.y), minMeshPoint.y);
            _pathPoints.Add(newPoint);
            
            float projLength = 0f;
            while (projLength < _planeSize.y)
            {
                Vector2 lastPoint = _pathPoints[_pathPoints.Count - 1];
                float dir = Mathf.Pow(-1f, _pathPoints.Count);
                float setAngle = Random.Range(_lineAngleRange.x, _lineAngleRange.y) * Mathf.Deg2Rad;

                float maxLength;
                CalcLength();

                void CalcLength()
                {
                    if (dir > 0)
                    {
                        maxLength = Mathf.Abs(workingWidth.y - lastPoint.x) / Mathf.Cos(setAngle);
                    }
                    else
                    {
                        maxLength = Mathf.Abs(lastPoint.x - workingWidth.x) / Mathf.Cos(setAngle);
                    }
                }

                if (Mathf.Min(maxLength, _lineLengthRange.y) < _lineLengthRange.x)
                {
                    dir *= -1;
                    CalcLength();
                }

                float lineLength = Random.Range(_lineLengthRange.x, Mathf.Min(maxLength, _lineLengthRange.y));
                maxLength = (maxMeshPoint.y - lastPoint.y) / Mathf.Cos(setAngle);
                lineLength = Mathf.Min(lineLength, maxLength);

                Quaternion angleRot = Quaternion.AngleAxis(setAngle * Mathf.Rad2Deg * dir, Vector3.up);
                Vector3 point3 = angleRot * new Vector3(0f, 0f, lineLength);
                newPoint = new Vector2(point3.x, point3.z);

                Vector2 setPoint = new Vector2(lastPoint.x + newPoint.x, lastPoint.y + newPoint.y);

                _pathPoints.Add(setPoint);

                projLength = Mathf.Abs(setPoint.y - _pathPoints[0].y);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Can't generate path: {e.Message}");
        }
    }

    private void CreateMesh()
    {
        CreateVertices(out List<Vector2> leftV, out List<Vector2> rightV);
        Polygon2D leftSide = Polygon2D.Contour(leftV.ToArray());
        Polygon2D rightSide = Polygon2D.Contour(rightV.ToArray());

        List<Vector2> keyPoints = new List<Vector2>();
        keyPoints.Add(leftV[1]);
        keyPoints.Add(leftV[0]);
        keyPoints.Add(rightV[rightV.Count - 1]);
        keyPoints.Add(rightV[rightV.Count - 2]);
        
        Triangulation2D triangulationLeft = new Triangulation2D(leftSide, 22.5f);
        Triangulation2D triangulationRight = new Triangulation2D(rightSide, 22.5f);

        Mesh meshLeft = triangulationLeft.Build(keyPoints);
        Mesh meshRight = triangulationRight.Build(keyPoints);
        meshFilterL.mesh = meshLeft;
        meshFilterR.mesh = meshRight;
    }

    private void CreateVertices(out List<Vector2> leftSide, out List<Vector2> rightSide)
    {
        leftSide = new List<Vector2>();
        rightSide = new List<Vector2>();
        
        leftSide.Add(new Vector2(-_planeSize.x * .5f, _planeSize.y * .5f));
        leftSide.Add(new Vector2(-_planeSize.x * .5f, -_planeSize.y * .5f));

        for (int i = 0; i < _pathPoints.Count; i++)
        {
            Vector2 point = _pathPoints[i];
            leftSide.Add(new Vector2(point.x - _width * .5f, point.y));
            rightSide.Add(new Vector2(point.x + _width * .5f, point.y));
        }
        
        rightSide.Add(new Vector2(_planeSize.x * .5f, _planeSize.y * .5f));
        rightSide.Add(new Vector2(_planeSize.x * .5f, -_planeSize.y * .5f));
    }
}
