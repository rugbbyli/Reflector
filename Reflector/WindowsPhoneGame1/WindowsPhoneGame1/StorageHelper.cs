using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;

namespace Reflector
{
    class StorageHelper
    {
        static IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

        static void InitSetting()
        {
            setting.Add("world", byte.MinValue);
            setting.Add("w0", ",");
            setting.Add("w1", ",");
            setting.Add("w2", ",");
            setting.Add("w3", ",");
            setting.Add("w4", ",");
            setting.Add("w5", ",");
            setting.Add("ncp", byte.MinValue);
            setting.Save();
        }

        public static byte GetUnlockWorld()
        {
            if (!setting.Contains("world"))
            {
                InitSetting();
                return 0;
            }
            else
            {
                return (byte)setting["world"];
            }
        }

        public static byte GetNotCompleteLevel()
        {
            if (!setting.Contains("world"))
            {
                InitSetting();
                return 0;
            }
            else
            {
                return (byte)setting["ncp"];
            }
        }

        public static void SetUnlockWorld(byte world)
        {
            setting["world"] = world;
            setting.Save();
        }

        public static string QueryFinishLevel(byte world)
        {
            return (string)setting["w" + world];
        } 

        public static bool IsLevelFinish(byte level)
        {
            return ((string)setting["w" + (level / 20)]).Contains("," + level + ",");
        }

        public static void AddFinishLevel(byte level)
        {
            string world = "w" + level / 20;
            string fl = (string)setting[world];
            if(!fl.Contains("," + level + ","))
            {
                setting[world] = fl + level + ",";

                if (level == (byte)setting["ncp"])
                {
                    byte ncp = (byte)(level + 1);
                    while (StorageHelper.IsLevelFinish(ncp) && ncp < 120)
                    {
                        ncp++;
                    }
                    setting["ncp"] = ncp;
                }

                setting.Save();

                //如果下个世界尚未解锁，则判断能否解锁
                byte cur = StorageHelper.GetUnlockWorld();
                if (cur < 5 && cur == level / 20)
                {
                    byte finished = 0;
                    foreach (char c in fl)
                    {
                        if (c == ',') finished++;
                    }
                    //超过17关已经完成，可以解锁下个世界
                    if (finished >= 18)
                    {
                        StorageHelper.SetUnlockWorld((byte)(cur + 1));
                    }
                }
            }
        }

        public static void SetValue(string key, string value)
        {
            if (key == "world") setting[key] = byte.Parse(value);
            else setting[key] = value;
            setting.Save();
        }

        public static void ClearData()
        {
            setting.Clear();
            setting.Save();
        }
    }
}
