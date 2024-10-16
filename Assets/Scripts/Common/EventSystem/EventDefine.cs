
public enum EventDefine
{
    #region SAS
    // 根据数据初始化block
    SAS_InitBlockMap,
    // 刷新所有
    SAS_RefreshBlockMap,
    // 更新block层次变化
    SAS_RefreshOccusion,
    // 更新block偏移变化
    SAS_RefreshOffsetPosition,
    // 更新block位置和图案
    SAS_RefreshAllBlocks,
    // 更新道具数量
    SAS_RefreshSkillCount,
    // 显示设置界面
    SAS_ShowSettingPanel,
    // 显示复活界面
    SAS_ShowRevivePanel,
    // 显示获取道具界面
    SAS_ShowAddSkillPanel,
    // 显示游戏结束界面
    SAS_ShowGameoverPanel,
    // 复活
    SAS_Revive,

    #endregion

    #region GC
    GC_StartGame,
    GC_ShowSettingPanel,
    GC_ShowGameoverPanel,

    #endregion

    #region BW
    BW_StartGame,
    BW_ShowSettingPanel,
    BW_RefreshScore,
    BW_ShowGameoverPanel,
    #endregion

    #region DJ
    DJ_ShowSettingPanel,
    DJ_StartGame,
    DJ_RefreshScore,
    DJ_Pause,
    DJ_Resume,
    DJ_Gameover,
    #endregion

    #region CP
    CP_UpdateAll,
    CP_UpdateLast,

    #endregion

}


