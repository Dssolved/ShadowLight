using System;
using System.Collections.Generic;

[Serializable]
public class LevelRecord
{
    public int levelIndex;
    public float bestTimeSeconds;
    public int bestFlamesLeft;
}

[Serializable]
public class SaveData
{
    public const int CurrentVersion = 1;
    public const float DefaultVolume = 0.8f;
    public const float MinVolume = 0.0001f;

    public int version = CurrentVersion;
    public List<int> unlockedLevels = new List<int> { 0 };
    public List<LevelRecord> records = new List<LevelRecord>();
    public float musicVolume = DefaultVolume;
    public float sfxVolume = DefaultVolume;

    public bool IsUnlocked(int levelIndex)
    {
        return unlockedLevels.Contains(levelIndex);
    }

    public LevelRecord FindRecord(int levelIndex)
    {
        foreach (LevelRecord record in records)
        {
            if (record.levelIndex == levelIndex) return record;
        }
        return null;
    }

    public void CompleteLevel(int levelIndex, float timeSeconds, int flamesLeft)
    {
        if (!IsUnlocked(levelIndex + 1)) unlockedLevels.Add(levelIndex + 1);

        LevelRecord record = FindRecord(levelIndex);
        if (record == null)
        {
            records.Add(new LevelRecord { levelIndex = levelIndex, bestTimeSeconds = timeSeconds, bestFlamesLeft = flamesLeft });
            return;
        }

        if (timeSeconds < record.bestTimeSeconds) record.bestTimeSeconds = timeSeconds;
        if (flamesLeft > record.bestFlamesLeft) record.bestFlamesLeft = flamesLeft;
    }

    public void Sanitize()
    {
        if (unlockedLevels == null) unlockedLevels = new List<int>();
        if (records == null) records = new List<LevelRecord>();
        if (!unlockedLevels.Contains(0)) unlockedLevels.Add(0);

        musicVolume = Math.Clamp(musicVolume, MinVolume, 1f);
        sfxVolume = Math.Clamp(sfxVolume, MinVolume, 1f);
    }
}
