using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public static class SaveHelper
    {
        [Serializable]
        private class StageData
        {
            public string Version;
            public bool IsFinished;
            public List<string> Saves;

            public bool AddSave(string name)
            {
                if (Saves == null)
                    Saves = new List<string>();
                if (Saves.Contains(name))
                    return false;
                Saves.Add(name);
                return true;
            }
            public bool RemoveSave(string name)
            {
                if (Saves == null)
                    return false;
                if (!Saves.Contains(name))
                    return false;
                Saves.Remove(name);
                return true;
            }
        }

        private static string getStageDataName(string key) => $"SAVE_{key}";
        private static string getSaveDataName(string key, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return $"SAVE_{key}_QUICK";
            else
                return $"SAVE_{key}_{name}";

        }

        private static StageData getStageData(string key)
        {
            string json = PlayerPrefs.GetString(getStageDataName(key));
            if (string.IsNullOrWhiteSpace(json))
                return new StageData() { Version = Application.version };
            return JsonUtility.FromJson<StageData>(json);
        }
        private static void setStageData(string key, StageData data)
        {
            PlayerPrefs.SetString(getStageDataName(key), JsonUtility.ToJson(data));
        }

        public static bool HasSave(string key, string name)
        {
            return PlayerPrefs.HasKey(getSaveDataName(key, name));
        }

        public static void Save(string key, string name, string data)
        {
            var stage = getStageData(key);
            if (stage.AddSave(name))
                setStageData(key, stage);

            PlayerPrefs.SetString(getSaveDataName(key, name), data);
            PlayerPrefs.Save();
        }

        public static string Load(string key, string name)
        {
            return PlayerPrefs.GetString(getSaveDataName(key, name));
        }

        public static void Remove(string key, string name)
        {
            var stage = getStageData(key);
            if (!stage.RemoveSave(name))
                return;

            setStageData(key, stage);
            PlayerPrefs.Save();
        }

        public static bool GetFinished(string key)
        {
            return getStageData(key)?.IsFinished ?? false;
        }
        public static void Finish(string key)
        {
            if (key == "DEBUG")
                return;

            var stage = getStageData(key);
            if (stage.IsFinished)
                return;
            stage.IsFinished = true;
            setStageData(key, stage);
            PlayerPrefs.Save();
        }
    }
}