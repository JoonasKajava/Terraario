using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;

    List<Vector3> Vertices = new List<Vector3>();
    List<int> Triangles = new List<int>();


    public void GenerateMesh(int[,] map, float SquareSize)
    {
        squareGrid = new SquareGrid(map, SquareSize);

        for (int x = 0; x < squareGrid.Squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.Squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.Squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.RecalculateNormals();

    }

    void TriangulateSquare(Square square)
    {
        switch (square.Configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.CenterBottom, square.BottomLeft, square.CenterLeft);
                break;
            case 2:
                MeshFromPoints(square.CenterRight, square.BottomRight, square.CenterBottom);
                break;
            case 4:
                MeshFromPoints(square.CenterTop, square.TopRight, square.CenterRight);
                break;
            case 8:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.CenterRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
                break;
            case 6:
                MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.CenterBottom);
                break;
            case 9:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
                break;
            case 12:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterLeft);
                break;
            case 5:
                MeshFromPoints(square.CenterTop, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft, square.CenterLeft);
                break;
            case 10:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
                break;
            case 11:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.BottomLeft);
                break;
            case 13:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
                break;
            case 14:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
                break;
        }
    }
    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3) CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4) CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5) CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6) CreateTriangle(points[0], points[4], points[5]);
    }

    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].VertexIndex == -1)
            {
                points[i].VertexIndex = Vertices.Count;
                Vertices.Add(points[i].Position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        Triangles.Add(a.VertexIndex);
        Triangles.Add(b.VertexIndex);
        Triangles.Add(c.VertexIndex);
    }

    void OnDrawGizmos()
    {
        return;
        for (int x = 0; x < squareGrid.Squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.Squares.GetLength(1); y++)
            {
                Gizmos.color = squareGrid.Squares[x, y].TopLeft.active ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.Squares[x, y].TopLeft.Position, Vector3.one * 0.4f);

                Gizmos.color = squareGrid.Squares[x, y].TopRight.active ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.Squares[x, y].TopRight.Position, Vector3.one * 0.4f);

                Gizmos.color = squareGrid.Squares[x, y].BottomRight.active ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.Squares[x, y].BottomRight.Position, Vector3.one * 0.4f);

                Gizmos.color = squareGrid.Squares[x, y].BottomLeft.active ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.Squares[x, y].BottomLeft.Position, Vector3.one * 0.4f);

                Gizmos.color = Color.red;
                Gizmos.DrawCube(squareGrid.Squares[x, y].CenterTop.Position, Vector3.one * .15f);
                Gizmos.DrawCube(squareGrid.Squares[x, y].CenterRight.Position, Vector3.one * .15f);
                Gizmos.DrawCube(squareGrid.Squares[x, y].CenterBottom.Position, Vector3.one * .15f);
                Gizmos.DrawCube(squareGrid.Squares[x, y].CenterLeft.Position, Vector3.one * .15f);
            }
        }
    }

    public class SquareGrid
    {
        public Square[,] Squares;

        public SquareGrid(int[,] map, float SquareSize)
        {
            int NodeCountX = map.GetLength(0);
            int NodeCountY = map.GetLength(1);
            float MapWidth = NodeCountX * SquareSize;
            float MapHeight = NodeCountY * SquareSize;


            ControlNode[,] controlNodes = new ControlNode[NodeCountX, NodeCountY];

            for (int x = 0; x < NodeCountX; x++)
            {
                for (int y = 0; y < NodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-MapWidth / 2 + x * SquareSize + SquareSize / 2, 0, -MapHeight / 2 + y * SquareSize + SquareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, SquareSize);
                }
            }

            Squares = new Square[NodeCountX - 1, NodeCountY - 1];

            for (int x = 0; x < NodeCountX - 1; x++)
            {
                for (int y = 0; y < NodeCountY - 1; y++)
                {
                    Squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }


    public class Square
    {
        public ControlNode TopLeft, TopRight, BottomLeft, BottomRight;
        public Node CenterTop, CenterRight, CenterBottom, CenterLeft;
        public int Configuration;

        public Square(ControlNode _TopLeft, ControlNode _TopRight, ControlNode _BottomRight, ControlNode  _BottomLeft)
        {
            TopLeft = _TopLeft;
            TopRight = _TopRight;
            BottomLeft = _BottomLeft;
            BottomRight = _BottomRight;

            CenterTop = TopLeft.right;
            CenterRight = BottomRight.above;
            CenterBottom = BottomLeft.right;
            CenterLeft = BottomLeft.above;

            if (TopLeft.active) Configuration += 8;
            if (TopRight.active) Configuration += 4;
            if (BottomRight.active) Configuration += 2;
            if (BottomLeft.active) Configuration += 1;
        }
    }

    public class Node
    {
        public Vector3 Position;
        public int VertexIndex = -1;
        public Node(Vector3 _pos)
        {
            Position = _pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(Position + Vector3.forward * squareSize / 2f);
            right = new Node(Position + Vector3.right * squareSize / 2f);
        }

    }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(-90, 0, 0); ;
    }

}
