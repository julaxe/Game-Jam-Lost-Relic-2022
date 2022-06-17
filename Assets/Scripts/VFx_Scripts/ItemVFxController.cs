using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVFxController : MonoBehaviour
{
    ParticleSystem smoke;
    ParticleSystem flash;
    GameObject LinkCircle;

    void Start()
    {
        flash = transform.Find("Flash_VFx").GetComponent<ParticleSystem>();
        smoke = transform.Find("Smoke_VFx").GetComponent<ParticleSystem>();
        LinkCircle = transform.Find("VFx_Connection").gameObject;

        LinkCircle.SetActive(false);
    }

    public void PlayVFx()
    {
        flash.Play();
        smoke.Play();

        Invoke("ShowCircle", 0.2f);
    }

    private void ShowCircle()
    {
        LinkCircle.SetActive(true);
    }
}
