using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject crosshair;
    [SerializeField] float baseTurnSpeed = 3;
    [SerializeField] GameObject gun;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform barrelEnd;
    [SerializeField] LineRenderer line;
    [SerializeField] int maxBounces = 10;
    [SerializeField] float offset = 0.01f;

    List<Vector3> laserPoints = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TrackMouse();
        TurnBase();

        Vector3 startPos = barrelEnd.position;
        Vector3 direction = barrelEnd.forward.normalized;

        laserPoints.Clear();
        laserPoints.Add(startPos);

        for (int i = 0; i < maxBounces; i++)
        {
            if (Physics.Raycast(startPos, direction, out RaycastHit hit, 1000.0f, targetLayer))
            {
                laserPoints.Add(hit.point);

             
                Vector3 ray = direction;
                Vector3 normal = hit.normal.normalized;

                direction = ray - 2f * Vector3.Dot(normal, ray) * normal;

                startPos = hit.point + direction * offset; 
            }
            else
            {
                laserPoints.Add(startPos + direction * 1000.0f);
                break;
            }
        }
        line.positionCount = laserPoints.Count;
        for(int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, laserPoints[i]);
        }
    }

    void TrackMouse()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(Physics.Raycast(cameraRay, out hit, 1000, targetLayer ))
        {
            crosshair.transform.forward = hit.normal;
            crosshair.transform.position = hit.point + hit.normal * 0.1f;
        }
    }

    void TurnBase()
    {
        Vector3 directionToTarget = (crosshair.transform.position - turretBase.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, directionToTarget.y, directionToTarget.z));
        turretBase.transform.rotation = Quaternion.Slerp(turretBase.transform.rotation, lookRotation, Time.deltaTime * baseTurnSpeed);
    }
}
