using UnityEngine;

public class LightObject : MonoBehaviour
{
    public enum LIGHT_TYPE
    {
        DIRECTIONAL,
        POINT,
        SPOT
    }
    [Header("Light Type")]
    public LIGHT_TYPE lightType;

    [Header("Light Config")]
    public Color lightColor = Color.white;
    [Range(0f, 1f)]
    public float smoothness = 0.5f;
    [Range(0f, 10f)]
    public float intensity = 2.0f;
    [Range(0f, 360f)]
    public float spotCutoff = 0.0f;

    [Range(0f, 360f)]
    public float spotInnerCutoff;
    [Range(0f, 10f)]
    public float specularStrength = 1.0f;

    public Vector3 attenuation = new Vector3(1, 0.09f, 0.032f);

    private Vector3 direction;

    private void Start()
    {
        LightManagerScript.instance.AddLight(this);
    }

    private void Update()
    {
        direction = transform.rotation * new Vector3(0, -1, 0);
        direction.Normalize();
    }

    public Vector3 GetDirection()
    {
        return this.direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.DrawRay(transform.position, direction * 10.0f);
    }
}
