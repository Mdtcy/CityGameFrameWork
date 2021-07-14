using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore.Editor
{
    [Serializable]
    public class SetupModel
    {
        public enum CityDisplayMode { Mesh, Sprite }
        public enum MapDisplayMode { Mesh, Sprite }
        public enum MapLayoutMode { Rectangle, Isometric }
        public enum MapAxisMode { XY, XZ }

        public string Directory = "MyCityBuilder";
        public CityDisplayMode CityDisplay = CityDisplayMode.Mesh;
        public MapDisplayMode MapDisplay = MapDisplayMode.Mesh;
        public MapLayoutMode MapLayout = MapLayoutMode.Rectangle;
        public MapAxisMode MapAxis = MapAxisMode.XZ;
        public Vector2Int MapSize = new Vector2Int(32, 32);
        public float Scale = 1f;

        public GridLayout.CellLayout CellLayout => MapLayout == MapLayoutMode.Rectangle ? GridLayout.CellLayout.Rectangle : GridLayout.CellLayout.Isometric;
        public GridLayout.CellSwizzle CellSwizzle => MapAxis == MapAxisMode.XY ? GridLayout.CellSwizzle.XYZ : GridLayout.CellSwizzle.XZY;
        public Vector3 CellSize => MapLayout == MapLayoutMode.Rectangle ? Vector3.one * Scale : new Vector3(Scale, Scale / 2f, Scale);
        public Tilemap.Orientation TilemapOrientation => MapAxis == MapAxisMode.XY ? Tilemap.Orientation.XY : Tilemap.Orientation.XZ;

        public string AssetDirectory => string.IsNullOrWhiteSpace(Directory) ? "Assets/" : "Assets/" + Directory;
        public string GetAssetPath(string asset, string folder = null)
        {
            if (string.IsNullOrWhiteSpace(folder))
                return AssetDirectory + "/" + asset;
            else
                return AssetDirectory + "/" + folder + "/" + asset;
        }
    }
}
