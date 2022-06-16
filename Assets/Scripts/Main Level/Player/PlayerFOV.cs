using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    Vector3 m_origin = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        

        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;

        

    }

   

    // Update is called once per frame
    void Update()
    {
        Debug.Log(m_origin);

        float fov = 360;
        
        int raycount = 180;
        float angle = 0f;
        float angleIncrease = fov / raycount;
        float viewDistance = 5;


        Vector3[] vertices = new Vector3[raycount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[raycount * 3];

        vertices[0] = m_origin ;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= raycount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast((Vector2)m_origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if(raycastHit2D.collider == null)
            {
                vertex = m_origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;

            
            angle -= angleIncrease;
        }

        


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        

    }
    
    private static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public void setOrigin(Vector3 origin)
    {
        m_origin = origin;
    }

}
