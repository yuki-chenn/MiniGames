using UnityEngine;


[CreateAssetMenu(menuName = "创建GC资源管理对象")]
public class GCAssetManager : ScriptableObject
{
    private static GCAssetManager _instance;
    public static GCAssetManager Instance
    {
        get
        {
            if (_instance == null) _instance = Resources.Load<GCAssetManager>("AssetManager/GCAssetContainer");
            return _instance;
        }
    }

    public GameObject colorSelectItemPrefab;
    public GameObject ansItemPrefab;
    public GameObject attemptItemPrefab;
    public GameObject colorItemPrefab;
    public GameObject resultItemPrefab;

    public Sprite attempColorItemUnselected;
    public Sprite attempColorItemselected;





}
