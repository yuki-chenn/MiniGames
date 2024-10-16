using UnityEngine;


[CreateAssetMenu(menuName = "创建BW资源管理对象")]
public class BWAssetManager : ScriptableObject
{
    private static BWAssetManager _instance;
    public static BWAssetManager Instance
    {
        get
        {
            if (_instance == null) _instance = Resources.Load<BWAssetManager>("AssetManager/BWAssetContainer");
            return _instance;
        }
    }


    public GameObject[] fruitPrefabs;





}
