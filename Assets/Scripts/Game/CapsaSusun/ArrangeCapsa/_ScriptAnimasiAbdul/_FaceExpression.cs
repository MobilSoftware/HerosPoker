using UnityEngine;

public class _FaceExpression : MonoBehaviour
{
    public SkinnedMeshRenderer skinRender;
    Transform avatarObj;

    [SerializeField]
    Material[] matExpressions;

    [SerializeField]
    Material[] matExpressionsFrank;

    [SerializeField]
    Material[] matExpressionsNormal;

    public void GetSkinMesh (Transform avatar)
    {
        avatarObj = avatar;

        Debug.Log(avatarObj.name);

        for (var i = 0; i < avatarObj.childCount; i++)
        {
            if (avatarObj.GetChild(i).name.Contains("Face"))
            {
                skinRender = avatarObj.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            }
        }
        
        matExpressions[0] = skinRender.materials[0];
    }

    public void ChangeBaseMat(int avatarId)
    {
        if (avatarId == 251)
            matExpressions = matExpressionsFrank;
        else
            matExpressions = matExpressionsNormal;
    }

    public void ChangeExpression(int type)
    {
        if (skinRender == null)
            return;

        switch (type)
        {
            case 9:
                SwitchFaceMaterial(0);
                break;
            case 101:
                SwitchFaceMaterial(1);
                break;
            case 102:
                SwitchFaceMaterial(2);
                break;
            case 18:case 103:
                SwitchFaceMaterial(3);
                break;
            case 12: case 20: case 1: case 104:
                SwitchFaceMaterial(4);
                break;
            case 15: case 19: case 16: case 8: case 105:
                SwitchFaceMaterial(5);
                break;
            case 13:
                SwitchFaceMaterial(6);
                break;
            case 17:
                SwitchFaceMaterial(7);
                break;
            case 10:
                SwitchFaceMaterial(8);
                break;
            case 21:
                SwitchFaceMaterial(9);
                break;
            case 11: case 14:
                SwitchFaceMaterial(10);
                break;
        }
    }

    public void ChangeExpressionHit(int type)
    {
        if (skinRender == null)
            return;

        switch (type)
        {
            case 0:case 3:case 4:
                SwitchFaceMaterial(4);
                break;
            case 1:case 6:
                SwitchFaceMaterial(1);
                break;
            case 5:
                SwitchFaceMaterial(3);
                break;
            default:
                SwitchFaceMaterial(6);
                break;
        }

    }

    void SwitchFaceMaterial(int faceIdx)
    {
        Material[] tempMat = skinRender.materials;

        tempMat[0] = matExpressions[faceIdx];

        skinRender.materials = tempMat;
    }
}
