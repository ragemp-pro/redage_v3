using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NeptuneEvo.Chars;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Database;
using LinqToDB;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;

namespace NeptuneEvo.Core
{
    #region Tattoo
    public enum TattooZones
    {
        Torso = 0,
        Head = 1,
        LeftArm = 2,
        RightArm = 3,
        LeftLeg = 4,
        RightLeg = 5,
    }

    public class Tattoo
    {
        public string Dictionary { get; set; }
        public string Hash { get; set; }
        public List<int> Slots { get; set; }

        public Tattoo(string dictionary, string hash, List<int> slots)
        {
            Dictionary = dictionary;
            Hash = hash;
            Slots = slots;
        }
    }
    #endregion

    #region ParentData
    public class ParentData
    {
        public int Father;
        public int Mother;
        public float Similarity;
        public float SkinSimilarity;

        public ParentData(int father, int mother, float similarity, float skinsimilarity)
        {
            Father = father;
            Mother = mother;
            Similarity = similarity;
            SkinSimilarity = skinsimilarity;
        }
    }
    #endregion

    #region AppearanceItem
    public class AppearanceItem
    {
        public int Value;
        public float Opacity;
        public int Color;

        public AppearanceItem(int value, float opacity, int color)
        {
            Value = value;
            Opacity = opacity;
            Color = color;
        }
    }
    #endregion

    #region HairData
    public class HairData
    {
        public int Hair;
        public int Color;
        public int HighlightColor;

        public HairData(int hair, int color, int highlightcolor)
        {
            Hair = hair;
            Color = color;
            HighlightColor = highlightcolor;
        }
    }
    #endregion

    #region PlayerCustomization Class
    public class PlayerCustomization
    {
        // Player
        public int Gender { get; set; }

        // Parents
        public ParentData Parents { get; set; }

        // Features
        public float[] Features { get; set; } = new float[20];

        // Appearance
        public AppearanceItem[] Appearance { get; set; } = new AppearanceItem[10];

        // Hair & Colors
        public HairData Hair { get; set; }

        public int EyeColor { get; set; }
        
        public Dictionary<int, List<Tattoo>> Tattoos { get; set; } = new Dictionary<int, List<Tattoo>>()
        {
            { 0, new List<Tattoo>() },
            { 1, new List<Tattoo>() },
            { 2, new List<Tattoo>() },
            { 3, new List<Tattoo>() },
            { 4, new List<Tattoo>() },
            { 5, new List<Tattoo>() },
        };
        public PlayerCustomization()
        {
            Gender = 0;
            Parents = new ParentData(0, 0, 1.0f, 1.0f);
            for (int i = 0; i < Features.Length; i++) Features[i] = 0f;
            for (int i = 0; i < Appearance.Length; i++) Appearance[i] = new AppearanceItem(255, 1.0f, 0);
            Hair = new HairData(0, 0, 0);
        }
    }
    #endregion



    #region Clothes Class
    class Clothes
    {
        public Clothes(int variation, List<int> colors, int price, int type = -1)
        {
            Variation = variation;
            Colors = colors;
            Price = price;
            Type = type;
        }

        public int Variation { get; }
        public List<int> Colors { get; }
        public int Price { get; }
        public int Type { get; }
    }
    #endregion

    class Customization : Script
    {
        private static readonly nLog Log = new nLog("Core.Customization");

        public static IReadOnlyDictionary<bool, Dictionary<int, int>> CorrectTorso = new Dictionary<bool, Dictionary<int, int>>()
        {
            {
                true, new Dictionary<int, int>()
                {
                        { 0, 0 },
                        { 1, 0 },
                        { 2, 2 },
                        { 3, 14 },
                        { 4, 14 },
                        { 5, 5 },
                        { 6, 14 },
                        { 7, 14 },
                        { 8, 8 },
                        { 9, 0 },
                        { 10, 14 },
                        { 11, 15 },
                        { 12, 12 },
                        { 13, 11 },
                        { 14, 12 },
                        { 15, 15 },
                        { 16, 0 },
                        { 17, 5 },
                        { 18, 0 },
                        { 19, 14 },
                        { 20, 14 },
                        { 21, 15 },
                        { 22, 0 },
                        { 23, 14 },
                        { 24, 14 },
                        { 25, 15 },
                        { 26, 11 },
                        { 27, 14 },
                        { 28, 14 },
                        { 29, 14 },
                        { 30, 14 },
                        { 31, 14 },
                        { 32, 14 },
                        { 33, 0 },
                        { 34, 0 },
                        { 35, 14 },
                        { 36, 5 },
                        { 37, 14 },
                        { 38, 8 },
                        { 39, 0 },
                        { 40, 15 },
                        { 41, 12 },
                        { 42, 11 },
                        { 43, 11 },
                        { 44, 0 },
                        { 45, 15 },
                        { 46, 14 },
                        { 47, 0 },
                        { 48, 1 },
                        { 49, 1 },
                        { 50, 1 },
                        { 51, 1 },
                        { 52, 2 },
                        { 53, 0 },
                        { 54, 1 },
                        { 55, 0 },
                        { 56, 0 },
                        { 57, 0 },
                        { 58, 14 },
                        { 59, 14 },
                        { 60, 15 },
                        { 61, 0 },
                        { 62, 14 },
                        { 63, 5 },
                        { 64, 14 },
                        { 65, 14 },
                        { 66, 15 },
                        { 67, 1 },
                        { 68, 14 },
                        { 69, 14 },
                        { 70, 14 },
                        { 71, 0 },
                        { 72, 14 },
                        { 73, 0 },
                        { 74, 14 },
                        { 75, 11 },
                        { 76, 14 },
                        { 77, 14 },
                        { 78, 14 },
                        { 79, 14 },
                        { 80, 0 },
                        { 81, 0 },
                        { 82, 0 },
                        { 83, 0 },
                        { 84, 1 },
                        { 85, 1 },
                        { 86, 1 },
                        { 87, 1 },
                        { 88, 14 },
                        { 89, 14 },
                        { 90, 14 },
                        { 91, 15 },
                        { 92, 6 },
                        { 93, 0 },
                        { 94, 0 },
                        { 95, 11 },
                        { 96, 11 },
                        { 97, 0 },
                        { 98, 0 },
                        { 99, 14 },
                        { 100, 14 },
                        { 101, 14 },
                        { 102, 14 },
                        { 103, 14 },
                        { 104, 14 },
                        { 105, 11 },
                        { 106, 14 },
                        { 107, 14 },
                        { 108, 14 },
                        { 109, 5 },
                        { 110, 1 },
                        { 111, 4 },
                        { 112, 14 },
                        { 113, 6 },
                        { 114, 14 },
                        { 115, 14 },
                        { 116, 14 },
                        { 117, 6 },
                        { 118, 14 },
                        { 119, 14 },
                        { 120, 6 },
                        { 121, 14 },
                        { 122, 14 },
                        { 123, 11 },
                        { 124, 14 },
                        { 125, 14 },
                        { 126, 1 },
                        { 127, 14 },
                        { 128, 0 },
                        { 129, 0 },
                        { 130, 14 },
                        { 131, 0 },
                        { 132, 0 },
                        { 133, 0 },
                        { 134, 0 },
                        { 135, 0 },
                        { 136, 14 },
                        { 137, 6 },
                        { 138, 14 },
                        { 139, 12 },
                        { 140, 14 },
                        { 141, 6 },
                        { 142, 14 },
                        { 143, 14 },
                        { 144, 6 },
                        { 145, 14 },
                        { 146, 0 },
                        { 147, 4 },
                        { 148, 4 },
                        { 149, 14 },
                        { 150, 14 },
                        { 151, 14 },
                        { 152, 14 },
                        { 153, 14 },
                        { 154, 14 },
                        { 155, 14 },
                        { 156, 14 },
                        { 157, 15 },
                        { 158, 15 },
                        { 159, 15 },
                        { 160, 15 },
                        { 161, 14 },
                        { 162, 15 },
                        { 163, 14 },
                        { 164, 0 },
                        { 165, 0 },
                        { 166, 14 },
                        { 167, 14 },
                        { 168, 14 },
                        { 169, 14 },
                        { 170, 15 },
                        { 171, 1 },
                        { 172, 14 },
                        { 173, 15 },
                        { 174, 14 },
                        { 175, 15 },
                        { 176, 15 },
                        { 177, 15 },
                        { 178, 1 },
                        { 179, 15 },
                        { 180, 15 },
                        { 181, 15 },
                        { 182, 1 },
                        { 183, 14 },
                        { 184, 14 },
                        { 185, 14 },
                        { 186, 14 },
                        { 187, 14 },
                        { 188, 14 },
                        { 189, 14 },
                        { 190, 14 },
                        { 191, 14 },
                        { 192, 14 },
                        { 193, 0 },
                        { 194, 1 },
                        { 195, 1 },
                        { 196, 1 },
                        { 197, 1 },
                        { 198, 1 },
                        { 199, 1 },
                        { 200, 1 },
                        { 201, 3 },
                        { 202, 4 },
                        { 203, 1 },
                        { 204, 6 },
                        { 205, 5 },
                        { 206, 5 },
                        { 207, 5 },
                        { 208, 0 },
                        { 209, 0 },
                        { 210, 0 },
                        { 211, 0 },
                        { 212, 14 },
                        { 213, 15 },
                        { 214, 14 },
                        { 215, 14 },
                        { 216, 15 },
                        { 217, 14 },
                        { 218, 14 },
                        { 219, 15 },
                        { 220, 14 },
                        { 221, 14 },
                        { 222, 11 },
                        { 223, 5 },
                        { 224, 1 },
                        { 225, 8 },
                        { 226, 0 },
                        { 227, 4 },
                        { 228, 4 },
                        { 229, 14 },
                        { 230, 14 },
                        { 231, 4 },
                        { 232, 14 },
                        { 233, 14 },
                        { 234, 11 },
                        { 235, 0 },
                        { 236, 0 },
                        { 237, 5 },
                        { 238, 2 },
                        { 239, 2 },
                        { 240, 14 },
                        { 241, 2 },
                        { 242, 2 },
                        { 243, 4 },
                        { 244, 6 },
                        { 245, 4 },
                        { 246, 3 },
                        { 247, 2 },
                        { 248, 6 },
                        { 249, 6 },
                        { 250, 0 },
                        { 251, 12 },
                        { 252, 0 },
                        { 253, 12 },
                        { 254, 12 },
                        { 255, 0 },
                        { 256, 0 },
                        { 257, 0 },
                        { 258, 0 },
                        { 259, 0 },
                        { 260, 0 },
                        { 261, 0 },
                        { 262, 14 }
                }
            },
            {
                false, new Dictionary<int, int>()
                {
                    { 0, 0  },
                    { 1, 5  },
                    { 2, 2  },
                    { 3, 3  },
                    { 4, 4 },
                    { 5, 4 },
                    { 6, 5 },
                    { 7, 5 },
                    { 8, 5 },
                    { 9, 0 },
                    { 10, 5 },
                    { 11, 4 },
                    { 12, 12 },
                    { 13, 15 },
                    { 14, 14 },
                    { 15, 15 },
                    { 16, 15 },
                    { 17, 0 },
                    { 18, 15 },
                    { 19, 15 },
                    { 20, 5 },
                    { 21, 4 },
                    { 22, 4 },
                    { 23, 4 },
                    { 24, 5 },
                    { 25, 5 },
                    { 26, 12 },
                    { 27, 0 },
                    { 28, 15 },
                    { 29, 9 },
                    { 30, 2 },
                    { 31, 5 },
                    { 32, 4 },
                    { 33, 4 },
                    { 34, 6 },
                    { 35, 5 },
                    { 36, 4 },
                    { 37, 4 },
                    { 38, 2 },
                    { 39, 1 },
                    { 40, 2 },
                    { 41, 5 },
                    { 42, 5 },
                    { 43, 3 },
                    { 44, 3 },
                    { 45, 3 },
                    { 46, 3 },
                    { 47, 3 },
                    { 48, 14 },
                    { 49, 14 },
                    { 50, 14 },
                    { 51, 6 },
                    { 52, 6 },
                    { 53, 5 },
                    { 54, 5 },
                    { 55, 5 },
                    { 56, 14 },
                    { 57, 5 },
                    { 58, 5 },
                    { 59, 5 },
                    { 60, 14 },
                    { 61, 3 },
                    { 62, 5 },
                    { 63, 5 },
                    { 64, 5 },
                    { 65, 5 },
                    { 66, 6 },
                    { 67, 2 },
                    { 68, 0 },
                    { 69, 0 },
                    { 70, 0 },
                    { 71, 0 },
                    { 72, 0 },
                    { 73, 14 },
                    { 74, 15 },
                    { 75, 9 },
                    { 76, 9 },
                    { 77, 9 },
                    { 78, 9 },
                    { 79, 9 },
                    { 80, 9 },
                    { 81, 9 },
                    { 82, 15 },
                    { 83, 9 },
                    { 84, 14 },
                    { 85, 14 },
                    { 86, 9 },
                    { 87, 9 },
                    { 88, 0 },
                    { 89, 0 },
                    { 90, 6 },
                    { 91, 6 },
                    { 92, 5 },
                    { 93, 5 },
                    { 94, 5 },
                    { 95, 5 },
                    { 96, 4 },
                    { 97, 5 },
                    { 98, 5 },
                    { 99, 5 },
                    { 100, 0 },
                    { 101, 15 },
                    { 102, 3 },
                    { 103, 3 },
                    { 104, 5 },
                    { 105, 4 },
                    { 106, 6 },
                    { 107, 6 },
                    { 108, 6 },
                    { 109, 6 },
                    { 110, 6 },
                    { 111, 4 },
                    { 112, 4 },
                    { 113, 4 },
                    { 114, 4 },
                    { 115, 4 },
                    { 116, 4 },
                    { 117, 11 },
                    { 118, 11 },
                    { 119, 11 },
                    { 120, 6 },
                    { 121, 6 },
                    { 122, 2 },
                    { 123, 2 },
                    { 124, 0 },
                    { 125, 14 },
                    { 126, 14 },
                    { 127, 14 },
                    { 128, 14 },
                    { 129, 14 },
                    { 130, 0 },
                    { 131, 3 },
                    { 132, 2 },
                    { 133, 5 },
                    { 134, 0 },
                    { 135, 3 },
                    { 136, 3 },
                    { 137, 5 },
                    { 138, 6 },
                    { 139, 5 },
                    { 140, 5 },
                    { 141, 14 },
                    { 142, 9 },
                    { 143, 5 },
                    { 144, 3 },
                    { 145, 3 },
                    { 146, 7 },
                    { 147, 1 },
                    { 148, 5 },
                    { 149, 5 },
                    { 150, 0 },
                    { 151, 0 },
                    { 152, 7 },
                    { 153, 5 },
                    { 154, 15 },
                    { 155, 15 },
                    { 156, 15 },
                    { 157, 15 },
                    { 158, 15 },
                    { 159, 15 },
                    { 160, 15 },
                    { 161, 11 },
                    { 162, 0 },
                    { 163, 5 },
                    { 164, 5 },
                    { 165, 5 },
                    { 166, 5 },
                    { 167, 15 },
                    { 168, 15 },
                    { 169, 15 },
                    { 170, 15 },
                    { 171, 15 },
                    { 172, 14 },
                    { 173, 15 },
                    { 174, 15 },
                    { 175, 15 },
                    { 176, 15 },
                    { 177, 15 },
                    { 178, 15 },
                    { 179, 11 },
                    { 180, 3 },
                    { 181, 15 },
                    { 182, 15 },
                    { 183, 15 },
                    { 184, 14 },
                    { 185, 6 },
                    { 186, 6 },
                    { 187, 6 },
                    { 188, 6 },
                    { 189, 6 },
                    { 190, 6 },
                    { 191, 6 },
                    { 192, 5 },
                    { 193, 5 },
                    { 194, 4 },
                    { 195, 4 },
                    { 196, 1 },
                    { 197, 1 },
                    { 198, 1 },
                    { 199, 1 },
                    { 200, 1 },
                    { 201, 1 },
                    { 202, 2 },
                    { 203, 8 },
                    { 204, 4 },
                    { 205, 2 },
                    { 206, 1 },
                    { 207, 4 },
                    { 208, 11 },
                    { 209, 11 },
                    { 210, 11 },
                    { 211, 11 },
                    { 212, 0 },
                    { 213, 1 },
                    { 214, 1 },
                    { 215, 1 },
                    { 216, 5 },
                    { 217, 4 },
                    { 218, 0 },
                    { 219, 5 },
                    { 220, 15 },
                    { 221, 15 },
                    { 222, 15 },
                    { 223, 15 },
                    { 224, 14 },
                    { 225, 15 },
                    { 226, 11 },
                    { 227, 3 },
                    { 228, 3 },
                    { 229, 4 },
                    { 230, 0 },
                    { 231, 0 },
                    { 232, 0 },
                    { 233, 11 },
                    { 234, 6 },
                    { 235, 1 },
                    { 236, 14 },
                    { 237, 3 },
                    { 238, 3 },
                    { 239, 3 },
                    { 240, 5 },
                    { 241, 3 },
                    { 242, 6 },
                    { 243, 6 },
                    { 244, 9 },
                    { 245, 14 },
                    { 246, 14 },
                    { 247, 4 },
                    { 248, 5 },
                    { 249, 14 },
                    { 250, 0 },
                    { 251, 3 },
                    { 252, 1 },
                    { 253, 9 }
                }
            },
        };
        public static IReadOnlyDictionary<bool, Dictionary<int, int>> EmtptySlots = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>() {
                { 1, 0 },
                { 3, 15 },
                { 4, 21 },
                { 5, 0 },
                { 6, 34 },
                { 7, 0 },
                { 8, 15 },
                { 9, 0 },
                { 10, 0 },
                { 11, 15 },
            }},
            { false, new Dictionary<int, int>() {
                { 1, 0 },
                { 3, 15 },
                { 4, 15 },
                { 5, 0 },
                { 6, 35 },
                { 7, 0 },
                { 8, 6 },
                { 9, 0 },
                { 10, 0 },
                { 11, 15 },
            }}
        };

        public static List<Clothes> Masks = new List<Clothes>()
        {
            new Clothes(1, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(2, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(3, new List<int>() { 0 }, 15000),
            new Clothes(4, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(5, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(6, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(7, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(8, new List<int>() { 0,1,2 }, 15000),
            new Clothes(9, new List<int>() { 0 }, 15000),
            new Clothes(10, new List<int>() { 0 }, 15000),
            new Clothes(13, new List<int>() { 0 }, 15000),
            new Clothes(14, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 }, 15000),
            new Clothes(15, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(16, new List<int>() { 0,1,2,3,4,5,6,7,8 }, 15000),
            new Clothes(17, new List<int>() { 0,1 }, 15000),
            new Clothes(18, new List<int>() { 0,1 }, 15000),
            new Clothes(19, new List<int>() { 0,1 }, 15000),
            new Clothes(20, new List<int>() { 0,1 }, 15000),
            new Clothes(21, new List<int>() { 0,1 }, 15000),
            new Clothes(22, new List<int>() { 0,1 }, 15000),
            new Clothes(23, new List<int>() { 0,1 }, 15000),
            new Clothes(24, new List<int>() { 0,1 }, 15000),
            new Clothes(25, new List<int>() { 0,1 }, 15000),
            new Clothes(26, new List<int>() { 0,1 }, 15000),
            new Clothes(28, new List<int>() { 0,1,2,3,4 }, 15000),
            new Clothes(29, new List<int>() { 0,1,2,3,4 }, 15000),
            new Clothes(30, new List<int>() { 0 }, 15000),
            new Clothes(31, new List<int>() { 0 }, 15000),
            new Clothes(32, new List<int>() { 0 }, 15000),
            new Clothes(34, new List<int>() { 0,1,2 }, 15000),
            new Clothes(35, new List<int>() { 0 }, 5000),
            new Clothes(37, new List<int>() { 0 }, 5000),
            new Clothes(38, new List<int>() { 0 }, 15000),
            new Clothes(39, new List<int>() { 0,1 }, 15000),
            new Clothes(40, new List<int>() { 0,1 }, 15000),
            new Clothes(41, new List<int>() { 0,1 }, 15000),
            new Clothes(42, new List<int>() { 0,1 }, 15000),
            new Clothes(46, new List<int>() { 0 }, 15000),
            new Clothes(49, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(50, new List<int>() { 0,1,2,3,4,5,6,7,8,9 }, 15000),
            new Clothes(51, new List<int>() { 0,1,2,3,4,5,6,7,8,9 }, 5500),
            new Clothes(52, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10 }, 5000),
            new Clothes(53, new List<int>() { 0,1,2,3,4,5,6,7,8 }, 5000),
            new Clothes(54, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10 }, 5000),
            new Clothes(56, new List<int>() { 0,1,2,3,4,5,6,7,8 }, 15000),
            new Clothes(57, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21 }, 5500),
            new Clothes(58, new List<int>() { 0,1,2,3,4,5,6,7,8,9 }, 5000),
            new Clothes(59, new List<int>() { 0 }, 15000),
            new Clothes(60, new List<int>() { 0,1,2 }, 15000),
            new Clothes(61, new List<int>() { 0,1,2 }, 15000),
            new Clothes(62, new List<int>() { 0,1,2 }, 15000),
            new Clothes(63, new List<int>() { 0,1,2 }, 15000),
            new Clothes(64, new List<int>() { 0,1,2 }, 15000),
            new Clothes(65, new List<int>() { 0,1,2 }, 15000),
            new Clothes(68, new List<int>() { 0,1,2 }, 15000),
            new Clothes(72, new List<int>() { 0,1,2 }, 15000),
            new Clothes(76, new List<int>() { 0,1,2 }, 15000),
            new Clothes(79, new List<int>() { 0,1,2 }, 15000),
            new Clothes(80, new List<int>() { 0,1,2 }, 15000),
            new Clothes(81, new List<int>() { 0,1,2 }, 15000),
            new Clothes(82, new List<int>() { 0,1,2 }, 15000),
            new Clothes(83, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(84, new List<int>() { 0 }, 15000),
            new Clothes(86, new List<int>() { 0,1,2 }, 15000),
            new Clothes(87, new List<int>() { 0,1 }, 15000),
            new Clothes(88, new List<int>() { 0,1 }, 15000),
            new Clothes(89, new List<int>() { 0,1,2,3,4 }, 15000),
            new Clothes(93, new List<int>() { 0,1,2,3,4,5 }, 15000),
            new Clothes(94, new List<int>() { 0,1,2,3,4,5 }, 15000),
            new Clothes(95, new List<int>() { 0,1,2,3,4,5,6,7 }, 15000),
            new Clothes(96, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(97, new List<int>() { 0,1,2,3,4,5 }, 15000),
            new Clothes(98, new List<int>() { 0 }, 15000),
            new Clothes(99, new List<int>() { 0,1,2,3,4,5 }, 15000),
            new Clothes(100, new List<int>() { 0,1,2,3,4,5 }, 15000),
            new Clothes(101, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 }, 15000),
            new Clothes(103, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(104, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(105, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23 }, 15000),
            new Clothes(106, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(108, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23 }, 15000),
            new Clothes(110, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(111, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 5500),
            new Clothes(112, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(113, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21 }, 5500),
            new Clothes(115, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(117, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20 }, 5500),
            new Clothes(118, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 5500),
            new Clothes(119, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24 }, 4700),
            // 121 - 127 - Beards (Male)
            new Clothes(128, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 }, 15000),
            new Clothes(129, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17 }, 15000),
            new Clothes(130, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18 }, 15000),
            new Clothes(131, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(132, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }, 15000),
            new Clothes(133, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 }, 15000),
            new Clothes(135, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13 }, 15000),
            new Clothes(136, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 }, 15000),
            new Clothes(138, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11 }, 15000),
            new Clothes(137, new List<int>() { 0,1,2,3,4,5,6,7 }, 15000),
            new Clothes(142, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11 }, 15000),
            new Clothes(146, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 }, 15000),
            new Clothes(147, new List<int>() { 0 }, 15000),
            new Clothes(156, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(157, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(158, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(159, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(160, new List<int>() { 0 }, 15000),
            new Clothes(161, new List<int>() { 0 }, 15000),
            new Clothes(162, new List<int>() { 0 }, 15000),
            new Clothes(163, new List<int>() { 0 }, 15000),
            new Clothes(165, new List<int>() { 0 }, 15000),
            new Clothes(167, new List<int>() { 0 }, 15000),
            new Clothes(168, new List<int>() { 0 }, 15000),
            new Clothes(170, new List<int>() { 0 }, 15000),
            new Clothes(171, new List<int>() { 0 }, 15000),
            new Clothes(172, new List<int>() { 0,1,2 }, 15000),
            new Clothes(173, new List<int>() { 0 }, 15000),
            new Clothes(174, new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24 }, 15000),
            new Clothes(176, new List<int>() { 0,1,2,3 }, 15000),
            new Clothes(177, new List<int>() { 0 }, 15000),
            new Clothes(179, new List<int>() { 0,1,2,3,4,5,6,7 }, 15000),

        };


        private static int SpawnIndex = 0;
        
        private static Vector3[] SpawnsPos =
        {
            new Vector3(-477.23682, -307.19675, 34.98841),
            new Vector3(-479.1031, -307.97488, 34.989967),
            /*new Vector3(-1841.6636, -362.8402, 49.387417),
            new Vector3(-1841.6816, -376.71417, 49.32009),
            new Vector3(-1884.7375, -341.20877, 49.272636),
            new Vector3(449.7277, 220.0, 103.165375),
            new Vector3(451.86755, 220.0, 103.165375),
            new Vector3(430.49283, 220.0, 103.165375),
            new Vector3(433.34906, 220.0, 103.165375),
            new Vector3(445.6795, 220.0, 103.165375),*/ //old spawn
        };

        public static Vector3 GetSpawnPos()
        {
            var pos = SpawnsPos[SpawnIndex];

            if (++SpawnIndex >= SpawnsPos.Length)
                SpawnIndex = 0;
            
            return pos;
        }
        
        public static int DimensionID = 1;

        #region Methods
        public static void ApplyCharacter(ExtPlayer player)//Сделать только в нужных моментах
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                var custom = player.GetCustomization();
                if (custom == null)
                    return;
                
                var gender = characterData.Gender;
                
                
                ClothesComponents.ClearClothes(player, gender);
                ClothesComponents.SetSpecialClothes(player, 2, custom.Hair.Hair, 0);
                
                var listTattoo = custom.Tattoos
                    .Values
                    .SelectMany(t => t
                        .Select(t2 => new Decoration
                            {
                                Collection = NAPI.Util.GetHashKey(t2.Dictionary),
                                Overlay = NAPI.Util.GetHashKey(t2.Hash)
                            }
                        ));
                
                player.SetDecoration(listTattoo.ToArray());
                ApplyCharacterFace(player);
                Chars.Repository.LoadAccessories(player);
                //ClothesComponents.SetHat(player, gender);
            }
            catch (Exception e)
            {
                Log.Write($"ApplyCharacter Exception: {e.ToString()}");
            }
        }

        public static void ApplyCharacterFace(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var custom = player.GetCustomization();
                if (custom == null)
                    return;
                
                var parents = custom.Parents;

                NAPI.Player.SetPlayerHeadBlend(player, new HeadBlend
                {
                    ShapeFirst = (byte)parents.Mother,
                    ShapeSecond = (byte)parents.Father,
                    ShapeThird = 0,
                    SkinFirst = (byte)parents.Mother,
                    SkinSecond = (byte)parents.Father,
                    SkinThird = 0,
                    ShapeMix = parents.Similarity,
                    SkinMix = parents.SkinSimilarity,
                    ThirdMix = 0.0f,
                });

                for (int i = 0; i < custom.Features.Count(); i++) 
                    NAPI.Player.SetPlayerFaceFeature(player, i, custom.Features[i]);

                for (int i = 0; i < custom.Appearance.Count(); i++)
                {
                    NAPI.Player.SetPlayerHeadOverlay(player, i, new HeadOverlay
                    {
                        Index = (byte)custom.Appearance[i].Value,
                        Opacity = custom.Appearance[i].Opacity,
                        Color = (byte)custom.Appearance[i].Color,
                        SecondaryColor = 100,
                    });
                }
                NAPI.Player.SetPlayerEyeColor(player, (byte)custom.EyeColor);
            }
            catch (Exception e)
            {
                Log.Write($"ApplyCharacterFace Exception: {e.ToString()}");
            }
            
        }

        public static async Task SaveCharacter(ServerBD db, ExtPlayer player, int uuid, bool UpdateCreate = false)
        {
            try
            {
                if (player == null) 
                    return;
                var custom = player.GetCustomization();
                if (custom == null)
                    return;

                var updateTask = db.Customization
                    .Where(v => v.Uuid == uuid)
                    .AsUpdatable();

                updateTask = updateTask
                    .Set(c => c.Gender, (sbyte)custom.Gender)
                    .Set(c => c.Parents, JsonConvert.SerializeObject(custom.Parents))
                    .Set(c => c.Features, JsonConvert.SerializeObject(custom.Features))
                    .Set(c => c.Appearance, JsonConvert.SerializeObject(custom.Appearance))
                    .Set(c => c.Hair, JsonConvert.SerializeObject(custom.Hair))
                    .Set(c => c.Eyec, custom.EyeColor)
                    .Set(c => c.Tattoos, JsonConvert.SerializeObject(custom.Tattoos));

                if (UpdateCreate)
                {
                    updateTask = updateTask
                        .Set(c => c.Iscreated, (sbyte)1);
                }
                await updateTask.UpdateAsync();
            }
            catch (Exception e)
            {
                Log.Write($"SaveCharacter Exception: {e.ToString()}");
            }
        }

        public static void InsertCustomization(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;
                
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;
                
                var custom = player.GetCustomization();
                
                if (custom != null)
                {
                    var gender =  (sbyte) custom.Gender;
                    var parents = JsonConvert.SerializeObject(custom.Parents);
                    var features = JsonConvert.SerializeObject(custom.Features);
                    var appearance = JsonConvert.SerializeObject(custom.Appearance);
                    var hair = JsonConvert.SerializeObject(custom.Hair);
                    //string Clothes = JsonConvert.SerializeObject(data.Clothes);
                    var tattoos = JsonConvert.SerializeObject(custom.Tattoos);
                    
                    Trigger.SetTask(async () =>
                    {
                        try
                        {
                            await using var db = new ServerBD("MainDB");//В отдельном потоке

                            await db.InsertAsync(new Customizations
                            {
                                Uuid = characterData.UUID,
                                Gender = gender,
                                Parents = parents,
                                Features = features,
                                Appearance = appearance,
                                Hair = hair,
                                Tattoos = tattoos,
                                Eyec = 0,
                                Iscreated = 1
                            });
                        }
                        catch (Exception e)
                        {
                            Debugs.Repository.Exception(e);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"CreateCharacter Exception: {e.ToString()}");
            }
        }

        public static void SendToCreator(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (sessionData.CreatorData.Inside) return;
                
                var custom = player.GetCustomization();

                var isCreate = custom != null;
                
                player.SetCustomization(new PlayerCustomization());

                if (!isCreate)
                {
                    custom = new PlayerCustomization();
                    sessionData.CreatorData.IsCreate = false;
                    InsertCustomization(player);
                }

                sessionData.CreatorData.Inside = true;
                sessionData.CreatorData.Changed = true;
                sessionData.CreatorData.Tattoos = custom.Tattoos;
                //sessionData.CreatorData.Clothes = custom.Clothes;
                
                Trigger.Dimension(player, (uint)(player.Value + 1));
                Trigger.ClientEvent(player, "CreatorCamera", true);
            }
            catch (Exception e)
            {
                Log.Write($"SendToCreator Exception: {e.ToString()}");
            }
        }

        public static void ApplyMaskFace(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;               
                var custom = player.GetCustomization();
                if (custom == null) 
                    return; 
                
                var parents = custom.Parents;
                var headBlend = new HeadBlend();
                headBlend.ShapeFirst = (byte)parents.Mother;
                headBlend.ShapeSecond = (byte)parents.Father;
                headBlend.ShapeThird = 0;

                headBlend.SkinFirst = (byte)parents.Mother;
                headBlend.SkinSecond = (byte)parents.Father;
                headBlend.SkinThird = 0;

                headBlend.ShapeMix = 0.0f;
                headBlend.SkinMix = parents.SkinSimilarity;
                headBlend.ThirdMix = 0.0f;

                NAPI.Player.SetPlayerHeadBlend(player, headBlend);

                NAPI.Player.SetPlayerFaceFeature(player, 0, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 2, 1f);
                NAPI.Player.SetPlayerFaceFeature(player, 9, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 10, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 13, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 14, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 15, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 16, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 17, -1f);
                NAPI.Player.SetPlayerFaceFeature(player, 18, 1f);

                for (int i = 0; i < custom.Appearance.Count(); i++)
                {
                    if (i != 2 && i != 10)
                    {
                        HeadOverlay headOverlay = new HeadOverlay();
                        headOverlay.Index = 255;
                        headOverlay.Opacity = 0;
                        headOverlay.SecondaryColor = 100;
                        NAPI.Player.SetPlayerHeadOverlay(player, i, headOverlay);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"ApplyMaskFace Exception: {e.ToString()}");
            }
        }
        
        #endregion

        #region Events

        [RemoteEvent("CreateCharacter")]
        public void onCreateCharacter(ExtPlayer player, int slot, string firstName, string lastName, bool gender,
            int father, int mother, float similarity, float skinSimilarity, string featur, string appearance,
            string hairAndColor, string clothes)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var accountData = player.GetAccountData();
                    if (accountData == null) 
                        return;
                    
                    var sessionData = player.GetSessionData();
                    if (sessionData == null) 
                        return;
                    
                    var isChanging = sessionData.CreatorData.Changed;
                              
                    var custom = player.GetCustomization();
                    
                    if (!isChanging)
                    {
                        if (accountData.Chars[slot] != -1)
                        {
                            Trigger.ClientEvent(player, "client.characters.create.error", "Слот занят");
                            return;
                        }
                        
                        int result = await Character.Create.Repository.Create(player, firstName, lastName);
                        if (result == -1) return;
                        
                        accountData.Chars[slot] = result;

                        Main.Usernames[accountData.Login] = accountData.Chars;

                        if (custom == null)
                            custom = new PlayerCustomization();
                    }
                    
                    var characterData = player.GetCharacterData();
                    
                    var genderChanged = !(isChanging && sessionData.CreatorData.IsCreate && characterData.Gender == gender);

                    characterData.Gender = gender;
                
                    // parents
                    custom.Parents.Father = father;
                    custom.Parents.Mother = mother;

                    custom.Parents.Similarity = similarity;
                    custom.Parents.SkinSimilarity = skinSimilarity;

                    // features
                    var feature_data = JsonConvert.DeserializeObject<float[]>(featur);
                    custom.Features = feature_data;
                    // appearance
                    var appearance_data = JsonConvert.DeserializeObject<AppearanceItem[]>(appearance);
                    custom.Appearance = appearance_data;
                    // hair & colors
                    var hair_and_color_data = JsonConvert.DeserializeObject<int[]>(hairAndColor);
                    for (int i = 0; i < hair_and_color_data.Length; i++)
                    {
                        switch (i)
                        {
                            // Hair
                            case 0:
                                {
                                    custom.Hair.Hair = hair_and_color_data[i];
                                    break;
                                }

                            // Hair Color
                            case 1:
                                {
                                    custom.Hair.Color = hair_and_color_data[i];
                                    break;
                                }

                            // Hair Highlight Color
                            case 2:
                                {
                                    custom.Hair.HighlightColor = hair_and_color_data[i];
                                    break;
                                }

                            // Eyebrow Color
                            case 3:
                                {
                                    custom.Appearance[(int)ClothesComponents.BarberComponent.Eyebrows].Color = hair_and_color_data[i];
                                    break;
                                }

                            // Beard Color
                            case 4:
                                {
                                    custom.Appearance[(int)ClothesComponents.BarberComponent.Beard].Color = hair_and_color_data[i];
                                    break;
                                }

                            // Eye Color
                            case 5:
                                {
                                    custom.EyeColor = hair_and_color_data[i];
                                    break;
                                }

                            // Blush Color
                            case 6:
                                {
                                    custom.Appearance[(int)ClothesComponents.BarberComponent.Palette].Color = hair_and_color_data[i];
                                    break;
                                }

                            // Lipstick Color
                            case 7:
                                {
                                    custom.Appearance[(int)ClothesComponents.BarberComponent.Lips].Color = hair_and_color_data[i];
                                    break;
                                }

                            // Chest Hair Color
                            case 8:
                                {
                                    custom.Appearance[(int)ClothesComponents.BarberComponent.Body].Color = hair_and_color_data[i];
                                    break;
                                }
                        }
                    }
                    // clothes

                    if (!genderChanged && isChanging)
                        custom.Tattoos = sessionData.CreatorData.Tattoos;
                    
                    player.SetCustomization(custom);
                    
                    NAPI.Task.Run(() =>
                    {
                        Trigger.ClientEvent(player, "client.charStore.Gender", gender);
                        
                        player.SetDefaultSkin();
                        
                        if (genderChanged)
                        {
                            if (isChanging) 
                                Chars.Repository.RemoveAllClothes(player);

                            var clothes_data = JsonConvert.DeserializeObject<ComponentVariation[]>(clothes);

                            var topsData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Tops];
                        
                            if (clothes_data[0].Drawable != -1) 
                                Chars.Repository.ChangeAccessoriesItem(player, 0, $"{clothes_data[0].Drawable}_{clothes_data[0].Texture}_{characterData.Gender}");//Hat
                        
                            if (clothes_data[1].Drawable != -1 && topsData.ContainsKey(clothes_data[1].Drawable))
                            {
                                if (ClothesComponents.IsTopDown(topsData[clothes_data[1].Drawable])) 
                                    Chars.Repository.ChangeAccessoriesItem(player, 6, $"{clothes_data[1].Drawable}_{clothes_data[1].Texture}_{characterData.Gender}");
                                else 
                                    Chars.Repository.ChangeAccessoriesItem(player, 5, $"{clothes_data[1].Drawable}_{clothes_data[1].Texture}_{characterData.Gender}");
                            }
                            
                            if (clothes_data[2].Drawable != -1) 
                                Chars.Repository.ChangeAccessoriesItem(player, 9, $"{clothes_data[2].Drawable}_{clothes_data[2].Texture}_{characterData.Gender}");
                            
                            if (clothes_data[3].Drawable != -1) 
                                Chars.Repository.ChangeAccessoriesItem(player, 13, $"{clothes_data[3].Drawable}_{clothes_data[3].Texture}_{characterData.Gender}");
                        }
                        
                        Trigger.ClientEvent(player, "client.charcreate.close");
                        
                        if (!isChanging)
                        {
                            sessionData.LoggedIn = true;
                            Main.HelloText(player);
                            
                            Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.NewChar, player.Name, player.Value));
                            
                            Trigger.ClientEvent(player, "client:OnOpenHelpMenu");
                            characterData.SpawnPos = GetSpawnPos();
                        }
                        else 
                            characterData.SpawnPos = new Vector3(-385.3705, -195.6715, 36.67242);
                        
                        Main.ClientEvent_Spawn(player, 0);
                    });
                    

                    sessionData.CreatorData = new CreatorData();
                    
                    
                    if (!isChanging)
                    {
                        InsertCustomization(player);
                    }
                    else
                    {
                        
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await SaveCharacter(db, player, characterData.UUID, UpdateCreate: true);
                    }


                }
                catch (Exception e)
                {
                    Log.Write($"ClientEvent_saveCharacter Task Exception: {e.ToString()}");
                }
            });
        }
        #endregion
    }
}
