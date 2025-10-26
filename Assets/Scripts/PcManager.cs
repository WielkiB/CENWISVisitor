using UnityEngine;

public class PcManager : MonoBehaviour
{
    public int minCondiiton = 0;
    public int maxCondiiton = 100;

    public MissionManager missionManager;

    public void UpdateMissionCondition()
    {
        if (missionManager)
            missionManager.CheckWinState();
    }
}
