// 

using System.Numerics;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IPlayerService
{
    MapLocation GetMapLocation();
    Vector3 GetPlayerPos();
    void SetPlayerPos(Vector3 pos);
    Vector3 GetTorrentPos();
    void SetTorrentPos(Vector3 pos);
    void SavePos(int index);
    void RestorePos(int index);
    void MoveToPosition(Position targetPosition);
    nint GetPlayerIns();
    uint GetBlockId();
    void SetHp(int hp);
    int GetCurrentHp();
    int GetMaxHp();
    void SetFullHp();
    void SetRfbs();
    void SetFp(int fp);
    int GetCurrentFp();
    void SetSp(int sp);
    int GetCurrentSp();
    float GetSpeed();
    void SetSpeed(float speed);
    void ToggleInfinitePoise(bool isInfinitePoiseEnabled);
    void ToggleDebugFlag(int offset, bool isEnabled, bool needsReminder = false);
    void ToggleNoDamage(bool isNoDamageEnabled);
    void ToggleNoHit(bool isNoHitEnabled);
    void ToggleNoRuneGain(bool isNoRuneGainEnabled);
    void ToggleNoRuneArcLoss(bool isNoRuneArcLossEnabled);
    void ToggleNoRuneLoss(bool isNoRuneLossEnabled);
    void ToggleNoTimePassOnDeath(bool isNoTimePassOnDeathEnabled);
    void SetNewGame(int value);
    int GetNewGame();
    void GiveRunes(int runes);
    int GetRuneLevel();
    Stats GetStats();
    void SetStat(int offset, int newValue);
    long GetHandle();
    void ToggleNoGravity(bool isEnabled);
    void ToggleTorrentNoDeath(bool isEnabled);
    void SetScadu(int value);
    int GetScadu();
    void SetSpiritAsh(int value);
    int GetSpiritAsh();
    int GetCurrentAnimation();
    void ToggleTorrentAnywhere(bool isEnabled);
    bool IsRiding();
}