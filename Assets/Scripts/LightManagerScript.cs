using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LightManagerScript : MonoBehaviour
{
    public static LightManagerScript instance { private set; get; }

    private List<LightObject> lightScripts = new List<LightObject>();
    private ComputeBuffer lightBuffer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public ComputeBuffer GetLightBufferData()
    {
        int numLights = lightScripts.Count;
        if (numLights == 0)
        {
            if (lightBuffer != null)
            {
                lightBuffer.Release();
                lightBuffer = null;
            }
            return null;
        }

        LightObj[] lightObjects = new LightObj[numLights];

        for (int i = 0; i < numLights; i++)
        {
            lightObjects[i].lightColor = lightScripts[i].lightColor;
            lightObjects[i].Attenuation = lightScripts[i].attenuation;
            lightObjects[i].Smoothness = lightScripts[i].smoothness;
            lightObjects[i].Direction = lightScripts[i].GetDirection();
            lightObjects[i].Intensity = lightScripts[i].intensity;
            lightObjects[i].Position = lightScripts[i].transform.position;
            lightObjects[i].SpotCutoff = lightScripts[i].spotCutoff;
            lightObjects[i].SpotInnerCutoff = lightScripts[i].spotInnerCutoff;
            lightObjects[i].SpecularStrength = lightScripts[i].specularStrength;
            lightObjects[i].LightType = (int)lightScripts[i].lightType;
        }

        int lightStructSize = Marshal.SizeOf(typeof(LightObj));

        if (lightBuffer != null) { lightBuffer.Release(); }
        lightBuffer = new ComputeBuffer(numLights, lightStructSize);
        lightBuffer.SetData(lightObjects);

        return lightBuffer;
    }


    public void AddLight(LightObject light)
    {
        lightScripts.Add(light);
    }

    public int GetNumLights()
    {
        return lightScripts.Count;
    }
}

struct LightObj
{
    public Color lightColor;
    public Vector3 Attenuation;
    public float Smoothness;
    public Vector3 Direction;

    public float Intensity;
    public Vector3 Position;
    public float SpotCutoff;
    public float SpotInnerCutoff;
    public float SpecularStrength;
    public int LightType;
}
