
using UnityEngine;

public class RotateLightScript : MonoBehaviour
{
    public float rotateSpeed = 0.2f;
    public Transform sun;
    public RectTransform uiElement;

    public VintageShader_Post vintageShader;
    public float dayTimeVintage = 2.0f;
    public float nightTimeVintage = 1.2f;

    private void Update()
    {
        sun.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        uiElement.Rotate(0, 0, -rotateSpeed * Time.deltaTime);

        if (vintageShader == null) { return; }
        float sunRotation = Mathf.Repeat(sun.eulerAngles.z, 360f);
        float t = Mathf.Abs((sunRotation % 360f) - 180f) / 180f;

        vintageShader.vintageScale = Mathf.Lerp(nightTimeVintage, dayTimeVintage, t);
    }
}
