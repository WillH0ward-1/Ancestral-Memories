using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class WaveManager : MonoBehaviour
{
    public ComputeShader waveCompute;
    public RenderTexture NState, Nm1State, Np1State;

    public Vector2Int resolution;

    public Material waveMaterial;
    public Texture2D waveTexture;

    public float dispersion;

    public Camera camera;

    float[][] waveN, waveNm1, waveNp1; // state info

    [SerializeField] float Lx = 10; //width
    [SerializeField] float Ly = 10; // height

    [SerializeField] float dx = 0.1f; 
    float dy { get => dx; }

    int nx, ny; // resoluton

    public float CFL = 0.5f;
    public float c = 1;
    float dt;

    public RenderTexture obstaclesTex;

    [SerializeField] private float elasticity = 0.98f;

    public Vector3 effect; 

    void Start()
    {
        nx = Mathf.FloorToInt(Lx / dx);
        ny = Mathf.FloorToInt(Ly / dy);
        
        waveTexture = new Texture2D(nx, ny, TextureFormat.RGBA32, false);

        waveN = new float[nx][];
        waveNm1 = new float[nx][];
        waveNp1 = new float[nx][];

        for (int i = 0; i < nx; i++)
        {
            waveN[i] = new float[ny];
            waveNp1[i] = new float[ny];
            waveNm1[i] = new float[ny];
        }

        waveMaterial.SetTexture("_MainTex", waveTexture);
        waveMaterial.SetTexture("_DisplacementTex", waveTexture);
    }

    float t;

    public bool reflectiveBoundary;


    private Vector2Int reso;
    /*
    void Start()
    {
        InitializeTexture(ref NState);
        InitializeTexture(ref Nm1State);
        InitializeTexture(ref Np1State);
        obstaclesTex.enableRandomWrite = true;
         
        Debug.Assert(obstaclesTex.width == resolution.x && obstaclesTex.height == resolution.y);
        waveMaterial.mainTexture = NState;

    }
    */
    void InitializeTexture(ref RenderTexture tex)
    {

        tex = new RenderTexture(resolution.x, resolution.y, 1, GraphicsFormat.R16G16B16A16_SNorm);

        tex.enableRandomWrite = true;
 
        tex.Create();

    }

    void WaveStep()
    {
        dt = CFL * dx / c;
        t += dx; // increment

        if (reflectiveBoundary)
            ApplyReflectiveBoundary();
            else ApplyAbsorptiveBoundary();
        
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                waveNm1[i][j] = waveN[i][j];
                waveN[i][j] = waveNp1[i][j];
            }
        }


        waveN[pulsePosition.x][pulsePosition.y] = dt * dt * 20 *  pulseMagnitude * Mathf.Cos(t*Mathf.Rad2Deg * pulseFrequency);

        for (int i = 1; i < nx-1; i++)
        {
            for (int j = 1; j < ny-1; j++)
            {
                float n_ij = waveN[i][j];
                float n_ip1j = waveN[i+1][j];
                float n_im1j = waveN[i-1][j];
                float n_ijp1 = waveN[i][j+1];
                float n_ijm1 = waveN[i][j-1];
                float nm1_ij = waveNm1[i][j];

                waveNp1[i][j] = 2f * n_ij - nm1_ij + CFL * CFL * (n_ijm1 + n_ijp1 + n_im1j + n_ip1j - 4f * n_ij);
                waveNp1[i][j] *= elasticity;
            }
        }
    }

    [SerializeField]float floatToColorMultiplier = 2f;
    [SerializeField] float pulseFrequency = 1;
    [SerializeField] float pulseMagnitude = 1f;
    [SerializeField] Vector2Int pulsePosition = new Vector2Int(50, 50);

    void ApplyMatrixToTexture(float[][] state, ref Texture2D tex, float floatToColorMultiplier)
    {
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                float val = state[i][j] * floatToColorMultiplier;
                tex.SetPixel(i,j,new Color(val + 0.5f, val + 0.5f, val + 0.5f, 1f)); 
            }
        }
        tex.Apply();
    }

    void ApplyReflectiveBoundary()
    {

        for (int i = 0; i < nx; i++)
        {
            waveN[i][0] = 0f;
            waveN[i][ny - 1] = 0f;
        }

        for (int j = 0; j < ny; j++)
        {
            waveN[0][j] = 0f;
            waveN[ny-1][j] = 0f;
        }
    }

    void ApplyAbsorptiveBoundary()
    {
        float v = (CFL - 1f) / (CFL + 1f);

        for (int i = 0; i < nx; i++)
        {
            waveNp1[i][0] = waveN[i][1] + v * (waveNp1[i][1] - waveN[i][0]);
            waveNp1[i][ny - 1] = waveN[i][ny - 2] + v * (waveNp1[i][ny - 2] - waveN[i][ny - 1]);
        } 

        for (int j = 0; j < ny; j++)
        {
            waveNp1[0][j] = waveN[1][j] + v * (waveNp1[1][j] - waveN[0][j]);
            waveNp1[ny - 1][j] = waveN[ny - 2][j] + v * (waveNp1[ny - 2][j] - waveN[ny - 1][j]);
        }
    } 

    void Update()
    {
        /*
        Graphics.CopyTexture(NState, Nm1State);
        Graphics.CopyTexture(Np1State, NState);

        waveCompute.SetTexture(0, "NState", NState);
        waveCompute.SetTexture(0, "Nm1State", Nm1State);
        waveCompute.SetTexture(0, "Np1State", Np1State);
        waveCompute.SetVector("effect", effect);
        waveCompute.SetVector("resolution", new Vector2 (resolution.x, resolution.y));

        waveCompute.SetFloat("dispersion", dispersion);
        waveCompute.SetTexture(0, "obstaclesTex", obstaclesTex);
        waveCompute.Dispatch(0, resolution.x / 8, resolution.y / 8, 1);
        */

        pulsePosition = GetWaveOriginPosition(ref pulsePosition);
        WaveStep();
        ApplyMatrixToTexture(waveN, ref waveTexture, floatToColorMultiplier);

    }

    public GameObject rayOrigin;
    public GameObject rippleOrigin;
    public bool shouldRipple;
    public LayerMask waterCheckMask;

    Vector2Int GetWaveOriginPosition(ref Vector2Int pos)
    {
        int rippleOriginX = (int)rippleOrigin.transform.position.x;
        int rippleOriginY = (int)rippleOrigin.transform.position.y;
        int rippleOriginZ = (int)rippleOrigin.transform.position.z;


        if (Physics.Raycast(rayOrigin.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, waterCheckMask)
            && hit.collider.transform.CompareTag("Water"))
        {
            return new Vector2Int((int)(hit.textureCoord.x * nx), (int)(hit.textureCoord.y * ny));
            //return new Vector2Int(rippleOriginX * nx, rippleOriginZ * ny);
        }

        else return new Vector2Int(0, 0);

    }

}


















    //https://www.youtube.com/watch?v=5EyL0WpjdI8&t=0s
