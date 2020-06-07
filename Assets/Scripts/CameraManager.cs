using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float m_moveSpeed = 5.0f;
    public float m_moveRate = 0.01f;
    public float m_scaleSpeed = 100.0f;

    private float currentValue = 0;

    void Update()
    {
        float hValue = Input.GetAxis("Horizontal") * m_moveSpeed;
        float vValue = Input.GetAxis("Vertical") * m_moveSpeed;
        float scale = -m_moveRate * currentValue + 1;
        scale = scale <= 0.5f ? 0.5f : scale;
        transform.Translate(new Vector3(hValue, 0, vValue) * scale * Time.deltaTime, Space.World);

        float mValue = Input.GetAxis("Mouse ScrollWheel") * m_scaleSpeed;
        if (mValue > 0)
        {
            if(transform.position.y >= 1.5f)
            {
                currentValue += mValue;
                transform.Translate(mValue * Vector3.forward * Time.deltaTime);
            }
        }
        else if(mValue < 0)
        {
            if (transform.position.y <= 10.0f)
            {
                currentValue += mValue;
                transform.Translate(mValue * Vector3.forward * Time.deltaTime);
            }
        }
    }
}

