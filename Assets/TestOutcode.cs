using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutcode : MonoBehaviour
{
    const int ResX = 1024;
    const int ResY = 768;
    Texture2D screen;
    Vector3[] cube = new Vector3[8];
    Vector2[] dividedPoints;
    Vector3[] finalImage;
    private float myAngle;

    // Start is called before the first frame update
    void Start()
    {
        screen = new Texture2D(ResX, ResY);
        GetComponent<Renderer>().material.mainTexture = screen;
        GetComponent<Renderer>().material.mainTexture = screen;

        Vector3 startingAxis = new Vector3(14, -3, -3);
        startingAxis.Normalize();

        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        //final image
        finalImage = MatrixTransform(cube, GettingAllMatrix(cube));

        dividedPoints = divide_by_z(finalImage);

        DrawTheSquare(dividedPoints);
        
        
    }

    //Gets the allMatrix
    private Matrix4x4 GettingAllMatrix(Vector3[] cube)
    {
        //Camera Setup
        Vector3 lookAt = new Vector3(0, 0, 0);
        Vector3 position = new Vector3(0, 0, 10);

        Vector3 direction = (lookAt - position);
        direction.Normalize();

        Vector3 up = new Vector3(0, 1, 0);
        up.Normalize();

        Quaternion rotationCamera = Quaternion.LookRotation(direction, up);

        //Getting the vewing matrix
        Matrix4x4 viewingMatrix = Matrix4x4.TRS(-position,
                            rotationCamera,
                            Vector3.one);


        //Getting a perspectve matrix
        Matrix4x4 perspectiveMatrix = Matrix4x4.Perspective(45, 1.6f, 1, 800);
        //printMatrix(perspectiveMatrix);

        return perspectiveMatrix * viewingMatrix;

    }

    //Draws the square onto the plane
    private void DrawTheSquare(Vector2[] dividedPoints)
    {
        Vector2 start = dividedPoints[0];  
        Vector2 finish = dividedPoints[1];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));


        start = dividedPoints[1];
        finish = dividedPoints[2];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[2];
        finish = dividedPoints[3];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[3];
        finish = dividedPoints[0];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[1];
        finish = dividedPoints[5];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[0];
        finish = dividedPoints[4];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[2];
        finish = dividedPoints[6];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[3];
        finish = dividedPoints[7];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[5];
        finish = dividedPoints[4];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[4];
        finish = dividedPoints[7];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[7];
        finish = dividedPoints[6];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = dividedPoints[6];
        finish = dividedPoints[5];

        if (lineClip(ref start, ref finish))
            plot(Breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));


        screen.Apply();
    }

    //Plots the pixels onto the screen
    private void plot(List<Vector2Int> list)
    {
        foreach (Vector2Int pixel in list)
            screen.SetPixel(pixel.x, pixel.y,Color.red);
    }

    //Makes the screen size smaller
    private Vector2Int convertToScreenSpace(Vector2 v)
    {
        int x = (int)Math.Round(((v.x + 1) / 2) * (Screen.width - 1));

        int y = (int)Math.Round(((1 - v.y) / 2) * (Screen.height - 1));

        return new Vector2Int(x, y);
    }

    private Vector2[] divide_by_z(Vector3[] list_of_vertices)
    {
        List<Vector2> output_list = new List<Vector2>();

        foreach (Vector3 v in list_of_vertices)
            output_list.Add(new Vector2(v.x / v.z, v.y / v.z));

        return output_list.ToArray();
    }

    private Vector3[] MatrixTransform(Vector3[] meshVertices,Matrix4x4 transformMatrix)
    {
        Vector3[] output = new Vector3[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
            output[i] = transformMatrix *
                new Vector4(
                meshVertices[i].x,
                meshVertices[i].y,
                meshVertices[i].z,
                    1);

        return output;

    }
     
    public static bool lineClip(ref Vector2 v, ref Vector2 u)
    {
        Outcode inViewPort = new Outcode();
        Outcode vO = new Outcode(v);
        Outcode uO = new Outcode(u);
        //detect trivial acceptance
        if ((vO + uO) == inViewPort)
            return true;

        //detect trivial rejection
        if ((vO * uO) != inViewPort)
            return false;

        if (vO == inViewPort)                   
            return lineClip(ref u,ref v);

        Vector2 v2 = v;
        if (vO.up)
        {
            v = intercept(u, v, 0);
            return false;      
        }
        if (vO.down)
        {
            v = intercept(u, v, 1);
            return false;
        }
        if (vO.left)
        {
            v = intercept(u, v, 2);
            return false;
        }
            v = intercept(u, v, 3);
            return false;
    }

    private static Vector2 intercept(Vector2 p1, Vector2 p2, int v2)
    {
        float slope = (p2.y - p1.y) / (p2.x - p1.x);
        if (v2 == 0)
            return new Vector2(p1.x + (1/slope) * (1- p1.y), 1);

        if (v2 == 1)
            return new Vector2(p1.x + (1 / slope) * (-1 - p1.y), -1);

        if (v2 == 2)
            return new Vector2( -1, p1.y + (slope) * (-1 - p1.x));

        return new Vector2(1, p1.y + (slope) * (1 - p1.x));

    }

    private static List<Vector2Int> Breshenham(Vector2Int start, Vector2Int finish)
    {
        int dx = finish.x - start.x;
        List<Vector2Int> list = new List<Vector2Int>();
        if(dx<0)
        {
            return Breshenham(finish, start);
        }

        int dy = finish.y - start.y;

        if(dy<0)//negative shape
        {
            return NegativeY(Breshenham(NegativeY(start), NegativeY(finish)));
        }

        if(dy > dx)//slope > 1
        {
            return SwapXY(Breshenham(SwapXY(start), SwapXY(finish)));
        }

        int a = 2 * dy;
        int b = 2 * (dy - dx);
        int p = (2 * dy) - dx;
        int y = start.y;


        for (int x = start.x; x <= finish.x; x++)
        {
            list.Add(new Vector2Int(x, y));

            if (p > 0)
            {
                y++;
                p += b;
            }
            else
            {
                p += a;
            }
        }

        return list;
    }

    private static List<Vector2Int> SwapXY(List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach (Vector2Int v in list)
        {
            outputList.Add(SwapXY(v));
        }

        return outputList;
    }

    private static Vector2Int SwapXY(Vector2Int vec)
    {
     
        return new Vector2Int(vec.y, vec.x);
    }

    private static Vector2Int NegativeY(Vector2Int vec)
    {
        return new Vector2Int(vec.x, -vec.y);
    }

    private static List<Vector2Int> NegativeY(List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();
        foreach (Vector2Int v in list)
            outputList.Add(NegativeY(v));

        return outputList;
    }

    private Matrix4x4 GetMatrixOfTranformations()
    {
        Vector3 startingAxis = new Vector3(14, -2, -2);
        startingAxis.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(-41, startingAxis);
        Matrix4x4 rotationMatrix =
            Matrix4x4.TRS(new Vector3(0, 0, 0),
                            rotation,
                            Vector3.one);

        Matrix4x4 scaleMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0),
                            Quaternion.identity,
                            new Vector3(14, 5, 2));

        Matrix4x4 translateMatrix =
            Matrix4x4.TRS(new Vector3(-4, 4, 1),
                            Quaternion.identity,
                            Vector3.one);

        return translateMatrix * scaleMatrix * rotationMatrix;
    }

    Matrix4x4 translationMatrix()
    {
        return Matrix4x4.TRS(new Vector3(-4, 4, 1),
                            Quaternion.identity,
                            Vector3.one);
    }

    Matrix4x4 viewingMatrix (Vector3 position, Vector3 target, Vector3 up)

    {
        Quaternion rotationCamera = Quaternion.LookRotation(target - position, up);
        return  Matrix4x4.TRS(-position,
                    rotationCamera,
                    Vector3.one);
    }

    Matrix4x4 rotationMatrix (float angle, Vector3 startingAxis )

    {
        
        Quaternion rotation = Quaternion.AngleAxis(angle, startingAxis.normalized);
        return
            Matrix4x4.TRS(new Vector3(0, 0, 0),
                            rotation,
                            Vector3.one);
    }

    private bool IsBackFace(Vector2 a, Vector2 b, Vector2 c)
    {
        if(((b.x - a.x) * (c.y - b.y)) - ((b.y - a.y) * (c.x - b.x)) > 0)
        {
            return true;
        }

        return false;
    }

    void Update()
    {
        myAngle += 1;
        Destroy(screen);
        screen = new Texture2D(ResX, ResY);

        GetComponent<Renderer>().material.mainTexture = screen;
        Matrix4x4 overall =
                    Matrix4x4.Perspective(45, 1.6f, 1, 800) *
                                viewingMatrix(new Vector3(0, 0, 10), Vector3.zero, Vector3.up) *
                                        (rotationMatrix(myAngle, new Vector3(0, 1, 1)) * translationMatrix());

        DrawTheSquare(divide_by_z(MatrixTransform(cube, overall)));
    }
}
