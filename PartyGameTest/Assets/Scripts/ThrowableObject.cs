using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;
    [SerializeField]
    private GameObject m_ObjectOrigin;
    
    void Update()
    {
        float step = m_Speed * Time.deltaTime;
        transform.localPosition += transform.forward * step;
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Ground")
        {
            gameObject.SetActive(false);
            transform.position = m_ObjectOrigin.transform.position;
        }
    }
}
