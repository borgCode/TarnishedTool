// 

using System.Numerics;
using SilkyRing.Models;

namespace SilkyRing.Interfaces;

public interface IPlayerService
{
    Vector3 GetPlayerPos();
    void SavePos(int index);
    void RestorePos(int index);
    PosWithHurtbox GetPosWithHurtbox();
    long GetPlayerIns();
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
    void ToggleDebugFlag(int offset, bool isEnabled);
    void ToggleNoDamage(bool isFreezeHealthEnabled);
    void ToggleNoRuneGain(bool isNoRuneGainEnabled);
    void ToggleNoRuneArcLoss(bool isNoRuneArcLossEnabled);
    void ToggleNoRuneLoss(bool isNoRuneLossEnabled);
    void SetNewGame(int value);
    int GetNewGame();
    void GiveRunes(int runes);
    int GetRuneLevel();
    Stats GetStats();
    void SetStat(int offset, int newValue);
    long GetHandle();
    void EnableGravity();
    void ToggleTorrentNoDeath(bool isEnabled);
    void SetScadu(int value);
    int GetScadu();
    void SetSpiritAsh(int value);
    int GetSpiritAsh();
}