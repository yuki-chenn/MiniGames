using UnityEngine;


[CreateAssetMenu(menuName = "创建SAS资源管理对象")]
public class SASAssetManager : ScriptableObject
{
    private static SASAssetManager _instance;
    public static SASAssetManager Instance
    {
        get
        {
            if (_instance == null) _instance = Resources.Load<SASAssetManager>("AssetManager/SASAssetContainer");
            return _instance;
        }
    }

    public GameObject blockPrefab;
    public Sprite blockBg;
    public Sprite blockBgGrey;
    public Sprite[] patternSprites;
}
