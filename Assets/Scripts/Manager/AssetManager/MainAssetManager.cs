using UnityEngine;


[CreateAssetMenu(menuName = "创建Main资源管理对象")]
public class MainAssetManager : ScriptableObject
{
    private static MainAssetManager _instance;
    public static MainAssetManager Instance
    {
        get
        {
            if (_instance == null) _instance = Resources.Load<MainAssetManager>("AssetManager/MainAssetContainer");
            return _instance;
        }
    }

    public Sprite bgmOnSprite;
    public Sprite bgmOffSprite;
    public Sprite effectOnSprite;
    public Sprite effectOffSprite;


}
