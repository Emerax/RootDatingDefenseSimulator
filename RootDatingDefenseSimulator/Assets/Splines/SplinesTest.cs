using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinesTest : MonoBehaviour
{
    [SerializeField] private Transform A;
    [SerializeField] private Transform B;
    [SerializeField] private Transform C;
    [SerializeField] private Transform D;
    [SerializeField] private Transform AB;
    [SerializeField] private Transform BC;
    [SerializeField] private Transform CD;
    [SerializeField] private Transform AB_BC;
    [SerializeField] private Transform BC_CD;
    [SerializeField] private Transform ABBC_BCCD;


    private float interpolateAmount;


    

    // Update is called once per frame
    void Update(){
        interpolateAmount = (interpolateAmount + Time.deltaTime) % 1f;
        
        /*AB.position = Vector3.Lerp(A.position, B.position, interpolateAmount);
        BC.position = Vector3.Lerp(B.position, C.position, interpolateAmount);
        CD.position = Vector3.Lerp(C.position, D.position, interpolateAmount);
        AB_BC.position = Vector3.Lerp(AB.position, BC.position, interpolateAmount);
        BC_CD.position = Vector3.Lerp(BC.position, CD.position, interpolateAmount);*/

        ABBC_BCCD.position = CubicLerp(A.position, B.position, C.position, D.position, interpolateAmount);
    }
    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);
    }

}
