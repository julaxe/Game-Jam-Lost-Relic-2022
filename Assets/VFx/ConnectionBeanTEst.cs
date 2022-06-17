using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionBeanTEst : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject BeanStart;
    public GameObject BeanEnd;
    public GameObject Bean;
    private float distance;
    private float angle;

    private SpriteRenderer beanSpriteRenderer;
    public Material linkMaterial;

    public GameObject LinkCircle;

    bool isPossing = false;
    float fillAmountLink = -0.1f;

    public ParticleSystem Flash;
    public ParticleSystem Smoke;

    void Start()
    {
        beanSpriteRenderer = Bean.GetComponent<SpriteRenderer>();
        linkMaterial.SetFloat("_PossesProgression", fillAmountLink);
        LinkCircle.SetActive(false);

        Invoke("StartLink", 2.0f);
    }

    // Update is called once per frame
    void Update()
    {

        UpdateLinkTransform();

        if (isPossing)
        {
            Possessing();
        }
    }

    public void UpdateLinkTransform()
    {
        distance = Vector2.Distance(BeanStart.transform.position, BeanEnd.transform.position);

        angle = Mathf.Atan((-BeanStart.transform.position.y + BeanEnd.transform.position.y)/ (-BeanStart.transform.position.x + BeanEnd.transform.position.x));
        angle = angle * Mathf.Rad2Deg;

        Vector2 position = new Vector2((-BeanStart.transform.position.x + BeanEnd.transform.position.x) * 0.5f, (-BeanStart.transform.position.y + BeanEnd.transform.position.y)*0.5f);

        Bean.transform.localPosition = position;
        Bean.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);

        //half of the distance because the transforme is 2,1,1;
        beanSpriteRenderer.size = new Vector2(distance * 0.5f, beanSpriteRenderer.size.y);

    }

    public void StartLink()
    {
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

            Invoke("ShowCircle", 0.2f);
        }
    }

    private void ShowCircle()
    {
        LinkCircle.SetActive(true);
    }
}
