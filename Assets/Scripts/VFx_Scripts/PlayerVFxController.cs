using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFxController : MonoBehaviour
{
    [SerializeField]
    private GameObject BeanStart;
    [SerializeField]
    private GameObject BeanEnd;
    [SerializeField]
    private GameObject Bean;
    [SerializeField]
    private GameObject LinkCircle;

    public ParticleSystem Flash;
    public ParticleSystem Smoke;


    private SpriteRenderer beanSpriteRenderer;
    [SerializeField]
    private Material linkMaterial;

    private float distance;
    private float angle;


    private bool isPossing = false;
    private float fillAmountLink = -0.1f;

    

    void Start()
    {
        BeanStart = gameObject;
        Bean = transform.Find("ConnectionBean").gameObject;
        LinkCircle = transform.Find("VFx_Connection").gameObject;
        Flash = transform.Find("Flash_VFx").GetComponent<ParticleSystem>();
        Smoke = transform.Find("Smoke_VFx").GetComponent<ParticleSystem>();

        beanSpriteRenderer = Bean.GetComponent<SpriteRenderer>();
        linkMaterial.SetFloat("_PossesProgression", fillAmountLink);
        LinkCircle.SetActive(false);

        StartLink(BeanEnd);
    }

    // Update is called once per frame
    void Update()
    {
        if (BeanEnd)
        {
            UpdateLinkTransform();
            if (isPossing)
            {
                Possessing();
            }
        }
    }

    public void UpdateLinkTransform()
    {
       

        distance = Vector2.Distance(BeanStart.transform.position, BeanEnd.transform.position);

        if (distance == 0) return;

        angle = Mathf.Atan((-BeanStart.transform.position.y + BeanEnd.transform.position.y)/ (-BeanStart.transform.position.x + BeanEnd.transform.position.x));
        angle = angle * Mathf.Rad2Deg;

        Vector2 position = new Vector2((-BeanStart.transform.position.x + BeanEnd.transform.position.x) * 0.5f, (-BeanStart.transform.position.y + BeanEnd.transform.position.y)*0.5f);


        Bean.transform.localPosition = position;

        Bean.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);

        //half of the distance because the transforme is 2,1,1;
        beanSpriteRenderer.size = new Vector2(distance * 0.5f, beanSpriteRenderer.size.y);

    }

    public void StartLink(GameObject possesedItem)
    {
        BeanEnd = possesedItem;
        isPossing = true;
    }

    public void Possessing()
    {
        fillAmountLink += 1 * Time.deltaTime;
        linkMaterial.SetFloat("_PossesProgression", fillAmountLink);

        if (fillAmountLink >= 1)
        {
            fillAmountLink = 1;
            isPossing = false;

            Flash.Play();
            Smoke.Play();

            BeanEnd.GetComponent<ItemVFxController>().PlayVFx();
            Invoke("ShowCircle", 0.2f);
        }
    }

    private void ShowCircle()
    {
        LinkCircle.SetActive(true);
    }
}
