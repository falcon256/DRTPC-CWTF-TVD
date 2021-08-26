using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SigCube : MonoBehaviour
{
    
    public Texture3D volumeTexture = null;
    public int volumeResolution = 256;
    public int[] idata = null;
    public Color[] colorData = null;
    public Shader volumeShader = null;
    private Material volumeMaterial = null;

    private int every100thFrame = 0;
    //start reflection data API

    public void markDetection(int x, int y, int z, int magnitude)
    {

        //Debug.Log(""+x + " " + y + " " + z + " " + magnitude);//debug
        int zOffset = z * volumeResolution * volumeResolution;
        int yOffset = y * volumeResolution;
        //idata[x + yOffset + zOffset] = Math.Min(255, idata[x + yOffset + zOffset] + magnitude);
        idata[x + yOffset + zOffset] = Math.Min(idata[x + yOffset + zOffset]+5,255);
        //Debug.Log("" + idata[x + yOffset + zOffset] + " " + magnitude);//debug
    }

    //end reflection data API




    // Start is called before the first frame update
    void Start()
    {
        initVolumeMaterial();
        //initMesh();
    }

    private void FixedUpdate()
    {
        if (volumeTexture != null)
        {
            fadeTextureOneUnit();
            UpdateColorMapFixedGradient();
            volumeTexture.SetPixels(colorData);
            volumeTexture.Apply();
            if (Input.GetKey(KeyCode.W))
                volumeTexture.filterMode = FilterMode.Point;
            if (Input.GetKey(KeyCode.E))
                volumeTexture.filterMode = FilterMode.Bilinear;
            if (Input.GetKey(KeyCode.T))
                volumeTexture.filterMode = FilterMode.Trilinear;
        }
    }
    /*
    private void initMesh()
    {
        this.GetComponent<MeshFilter>().mesh = generateRenderMesh();
    }
    */
    /*
    private Mesh updateMeshColors(Mesh mesh)
    {
        Color[] colors= new Color[8 * volumeResolution * volumeResolution * volumeResolution];
        float cubeSize = 0.5f / volumeResolution;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            colors[i] = getColorAtPosition(
                (int)(mesh.vertices[i].x * cubeSize) * volumeResolution,
                (int)(mesh.vertices[i].x * cubeSize) * volumeResolution,
                (int)(mesh.vertices[i].x * cubeSize) * volumeResolution);
        }
        mesh.SetColors(colors);
        return mesh;
    }
    */

    //lots of work, turned out to be too slow to rebuild the mesh. Surprisingly this worked, and was written from memory.
    /*
    private Mesh generateRenderMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "3d grid render mesh.";

        float cubeSize = 0.5f / volumeResolution;

        Vector3[] vertices = new Vector3[8 * volumeResolution * volumeResolution * volumeResolution];
        int[] triangles = new int[12 * 3 * volumeResolution * volumeResolution * volumeResolution];
        int nv = 0;
        int nt = 0;
        for (int x = 0; x < volumeResolution; x++)
        {
            for (int y = 0; y < volumeResolution; y++)
            {
                for (int z = 0; z < volumeResolution; z++)
                {
                    float xoff = x * cubeSize;
                    float yoff = y * cubeSize;
                    float zoff = z * cubeSize;
                    vertices[nv + 0] = new Vector3(xoff, yoff, zoff);
                    vertices[nv + 1] = new Vector3(xoff+cubeSize, yoff, zoff);
                    vertices[nv + 2] = new Vector3(xoff + cubeSize, yoff + cubeSize, zoff);
                    vertices[nv + 3] = new Vector3(xoff, yoff + cubeSize, zoff);
                    vertices[nv + 4] = new Vector3(xoff, yoff + cubeSize, zoff + cubeSize);
                    vertices[nv + 5] = new Vector3(xoff + cubeSize, yoff + cubeSize, zoff + cubeSize);
                    vertices[nv + 6] = new Vector3(xoff + cubeSize, yoff, zoff + cubeSize);
                    vertices[nv + 7] = new Vector3(xoff, yoff, zoff + cubeSize);
                    triangles[((nt + 0) * 3) + 0] = nv + 0; triangles[((nt + 0) * 3) + 1] = nv + 2; triangles[((nt + 0) * 3) + 2] = nv + 1; //face front
                    triangles[((nt + 1) * 3) + 0] = nv + 0; triangles[((nt + 1) * 3) + 1] = nv + 3; triangles[((nt + 1) * 3) + 2] = nv + 2;
                    triangles[((nt + 2) * 3) + 0] = nv + 2; triangles[((nt + 2) * 3) + 1] = nv + 3; triangles[((nt + 2) * 3) + 2] = nv + 4;//face top
                    triangles[((nt + 3) * 3) + 0] = nv + 2; triangles[((nt + 3) * 3) + 1] = nv + 4; triangles[((nt + 3) * 3) + 2] = nv + 5;
                    triangles[((nt + 4) * 3) + 0] = nv + 1; triangles[((nt + 4) * 3) + 1] = nv + 2; triangles[((nt + 4) * 3) + 2] = nv + 5;//face right
                    triangles[((nt + 5) * 3) + 0] = nv + 1; triangles[((nt + 5) * 3) + 1] = nv + 5; triangles[((nt + 5) * 3) + 2] = nv + 6;
                    triangles[((nt + 6) * 3) + 0] = nv + 0; triangles[((nt + 6) * 3) + 1] = nv + 7; triangles[((nt + 6) * 3) + 2] = nv + 4;//face left
                    triangles[((nt + 7) * 3) + 0] = nv + 0; triangles[((nt + 7) * 3) + 1] = nv + 4; triangles[((nt + 7) * 3) + 2] = nv + 3;
                    triangles[((nt + 8) * 3) + 0] = nv + 5; triangles[((nt + 8) * 3) + 1] = nv + 4; triangles[((nt + 8) * 3) + 2] = nv + 7;//face back
                    triangles[((nt + 9) * 3) + 0] = nv + 5; triangles[((nt + 9) * 3) + 1] = nv + 7; triangles[((nt + 9) * 3) + 2] = nv + 6;
                    triangles[((nt + 10) * 3) + 0] = nv + 0; triangles[((nt + 10) * 3) + 1] = nv + 6; triangles[((nt + 10) * 3) + 2] = nv + 7;//face bottom
                    triangles[((nt + 11) * 3) + 0] = nv + 0; triangles[((nt + 11) * 3) + 1] = nv + 1; triangles[((nt + 11) * 3) + 2] = nv + 6;
                    nv += 8;
                    nt += 12;
                }
            } 
        }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles,0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.Optimize();
        return mesh;
    }
    */

    public void initVolumeMaterial()
    {
        this.volumeTexture = GenerateInitialTexture3D();
        Debug.Log("Generating Rendering Material");
        volumeMaterial = new Material(volumeShader);
        volumeMaterial.SetFloat("_Threshold", 0.2f);
        volumeMaterial.SetFloat("_Intensity", 5.0f);
        volumeMaterial.SetVector("_SliceMin", new Vector3(0, 0, 0));
        volumeMaterial.SetVector("_SliceMax", new Vector3(1, 1, 1));
        volumeMaterial.SetMatrix("_AxisRotationMatrix", Matrix4x4.Rotate(Quaternion.identity));
        volumeMaterial.SetTexture("_Volume", volumeTexture);
        this.GetComponent<MeshRenderer>().sharedMaterial = volumeMaterial;
        //volumeMaterial.SetTexture("_MainTex", volumeTexture);
 
    }

    //from unity docs, modified
    private Texture3D GenerateInitialTexture3D()
    {
        Debug.Log("Generating Initial 3D Volumetric Data.");
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        Texture3D texture = new Texture3D(volumeResolution, volumeResolution, volumeResolution, format, false);
        texture.wrapMode = wrapMode;

        colorData = new Color[volumeResolution * volumeResolution * volumeResolution];
        idata = new int[volumeResolution * volumeResolution * volumeResolution];

        float inverseResolution = 1.0f / (volumeResolution - 1.0f);
        for (int z = 0; z < volumeResolution; z++)
        {
            int zOffset = z * volumeResolution * volumeResolution;
            for (int y = 0; y < volumeResolution; y++)
            {
                int yOffset = y * volumeResolution;
                for (int x = 0; x < volumeResolution; x++)
                {
                    //colorData[x + yOffset + zOffset]= new Color(x * inverseResolution, y * inverseResolution, z * inverseResolution, 32);
                    //idata[x + yOffset + zOffset] = (int)((colorData[x + yOffset + zOffset].r+colorData[x + yOffset + zOffset].g+colorData[x + yOffset + zOffset].b)*16.0f);
                    idata[x + yOffset + zOffset] = 0;
                    colorData[x + yOffset + zOffset] = new Color(0,0,0,0);
                }
            }
        }
        
        // Copy the color values to the texture
        texture.SetPixels(colorData);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        return texture;      
    }

    private void fadeTextureOneUnit()
    {
        if (Input.GetKey(KeyCode.A))
            return;
        every100thFrame++;
        if (every100thFrame >= 10||Input.GetKey(KeyCode.Q))
        {
            for (int i = 0; i < idata.Length; i++)
                idata[i] = Mathf.Max(idata[i] - 1, 0);
            every100thFrame = 0;
        }
    }


    private void UpdateColorMapFixedGradient()
    {
        for (int z = 0; z < volumeResolution; z++)
        {
            int zOffset = z * volumeResolution * volumeResolution;
            for (int y = 0; y < volumeResolution; y++)
            {
                int yOffset = y * volumeResolution;
                for (int x = 0; x < volumeResolution; x++)
                {                   
                    colorData[x + yOffset + zOffset] = Color.HSVToRGB((((float)idata[x + yOffset + zOffset] / 128.0f)+0.5f)%1.0f, 1.0f, 1.0f);
                    colorData[x + yOffset + zOffset].a = (float)idata[x + yOffset + zOffset]/1024.0f;
                }
            }
        }
    }

    private Color getColorAtPosition(int x, int y, int z)
    {
        int yOffset = y * volumeResolution;
        int zOffset = z * volumeResolution * volumeResolution;
        return colorData[x + yOffset + zOffset];
    }
}
