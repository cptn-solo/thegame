﻿using UnityEngine;

namespace Assets.Scripts.Services.App
{
    public struct PlayerInfo
    {
        public string NickName;
        public Color BodyTintColor;
        public int Score;

        public PlayerInfo(string nickName, Color bodyTintColor, int score)
        {
            NickName = nickName;
            BodyTintColor = bodyTintColor;
            Score = score;
        }
        public string Serialize()
        {
            var colorHex = HexStringFromColor(BodyTintColor);
            return $"{NickName}:{colorHex}:{Score}";
        }

        public static PlayerInfo Deserialize(string playerInfo)
        {
            var props = playerInfo.Split(":");

            PlayerInfo info = default;
            info.NickName = props[0];
            info.BodyTintColor = ColorFromHexString(props[1]);
            info.Score = int.Parse(props[2]);
            return info;
        }
        public static string HexStringFromColor(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
        public static Color ColorFromHexString(string hexString)
        {
            if (ColorUtility.TryParseHtmlString($"#{hexString}", out var color))
                return color;
            return Color.black;
        }
    }
}
