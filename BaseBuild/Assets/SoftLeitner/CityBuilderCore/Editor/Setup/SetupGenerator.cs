using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace CityBuilderCore.Editor
{
    [CreateAssetMenu()]
    public class SetupGenerator : ScriptableObject
    {
        public const string FOLDER_CITY = "City";
        public const string FOLDER_ROADS = "Roads";
        public const string FOLDER_BUILDINGS = "Buildings";
        public const string FOLDER_WALKERS = "Walkers";
        public const string FOLDER_ITEMS = "Items";
        public const string FOLDER_SCENES = "Scenes";
        public const string FOLDER_PALETTE = "Palette";
        public const string FOLDER_MODELS = "Models";
        public const string FOLDER_SPRITES = "Sprites";
        public const string FOLDER_MATERIALS = "Materials";
        public const string FOLDER_UI = "UI";

        public static string FOLDER_CITY_ROADS => Path.Combine(FOLDER_CITY, FOLDER_ROADS);
        public static string FOLDER_CITY_BUILDINGS => Path.Combine(FOLDER_CITY, FOLDER_BUILDINGS);
        public static string FOLDER_CITY_WALKERS => Path.Combine(FOLDER_CITY, FOLDER_WALKERS);
        public static string FOLDER_CITY_ITEMS => Path.Combine(FOLDER_CITY, FOLDER_ITEMS);

        private static SetupGenerator _instance;
        public static SetupGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    var script = Resources.FindObjectsOfTypeAll<MonoScript>().Single(s => s.GetClass() == typeof(SetupGenerator));
                    var directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));

                    InternalEditorUtility.LoadSerializedFileAndForget(Path.Combine(directory, "SetupGenerator.asset"));
                }

                return _instance;
            }
        }

        public SceneAsset Scene;

        [Header("Palette")]
        public Grid GroundPalette;
        public Sprite RectBlank;
        public Sprite IsoBlank;
        public TileBase InfoTile;
        public TileBase ValidTile;
        public TileBase InvalidTile;
        public TileBase GroundTile;
        public TileBase GroundBlockedTile;
        public TileBase RoadTile;
        public ObjectTile RoadObjectTile;
        [Header("Building")]
        public Building Building;
        public BuildingInfo BuildingInfo;
        public Mesh BuildingMesh;
        public Sprite BuildingSprite;
        public Sprite BuildingSpriteIso;
        [Header("Walker")]
        public Walker Walker;
        public WalkerInfo WalkerInfo;
        public Mesh WalkerMesh;
        public Sprite WalkerSprite;
        public Sprite WalkerSpriteIso;
        [Header("UI")]
        public RenderTexture MinimapTexture;
        public TMPro.TMP_FontAsset Font;
        [Header("Materials")]
        public Material GroundMaterial;
        public Material BlockedMaterial;
        public Material OverlayMaterial;

        private SetupModel _model;

        public SetupGenerator()
        {
            _instance = this;
        }

        public void Execute(SetupModel model)
        {
            _model = model;

            setupFolders();

            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(copyAsset(Scene, FOLDER_SCENES, "Main.unity")));

            setupUI();
            setupCamera();
            var roadTile = setupGrid();
            setupMap();
            var item = setupItem();
            var road = setupRoad(item, roadTile);
            var walker = setupWalker();
            var building = setupBuilding(walker, item);

            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(GameObject.Find("Grid").scene);
        }

        private void setupFolders()
        {
            if (!string.IsNullOrWhiteSpace(_model.Directory))
                createFolder("Assets", _model.Directory);
            createFolder(_model.AssetDirectory, FOLDER_SCENES);

            createFolder(_model.AssetDirectory, FOLDER_CITY);
            createFolder(_model.AssetDirectory, FOLDER_CITY, FOLDER_ROADS);
            createFolder(_model.AssetDirectory, FOLDER_CITY, FOLDER_BUILDINGS);
            createFolder(_model.AssetDirectory, FOLDER_CITY, FOLDER_WALKERS);
            createFolder(_model.AssetDirectory, FOLDER_CITY, FOLDER_ITEMS);

            createFolder(_model.AssetDirectory, FOLDER_PALETTE);
            createFolder(_model.AssetDirectory, FOLDER_MODELS);
            createFolder(_model.AssetDirectory, FOLDER_SPRITES);
            createFolder(_model.AssetDirectory, FOLDER_MATERIALS);
            createFolder(_model.AssetDirectory, FOLDER_UI);
        }

        private void setupCamera()
        {
            var two = GameObject.Find("2DCamera");
            var tre = GameObject.Find("3DCamera");

            var twoPos = two.transform.GetChild(0);

            var trePiv = tre.transform.GetChild(0);
            var trePos = trePiv.transform.GetChild(0);

            var twoCam = twoPos.GetComponent<Camera>();
            var treCam = trePos.GetComponent<Camera>();

            var twoCon = twoCam.GetComponent<CameraController>();
            var treCon = treCam.GetComponent<CameraController>();

            if (_model.MapAxis == SetupModel.MapAxisMode.XY)
            {
                two.transform.rotation = Quaternion.identity;
                tre.transform.rotation = Quaternion.Euler(270, 0, 0);
            }
            else
            {
                two.transform.rotation = Quaternion.Euler(90, 0, 0);
                tre.transform.rotation = Quaternion.identity;
            }

            var x = _model.MapSize.x / 2f * _model.Scale;
            var y = _model.MapSize.y / 2f * _model.Scale;

            if (_model.MapLayout == SetupModel.MapLayoutMode.Isometric)
            {
                x = 0;
                y /= 2f;
            }

            twoCon.MaxZoom = y;
            treCon.MaxZoom = y;

            twoCon.ZoomSpeed = twoCon.MaxZoom;
            treCon.ZoomSpeed = treCon.MaxZoom;

            twoPos.localPosition = new Vector3(x, y);
            twoCam.orthographicSize = y;

            trePiv.localPosition = new Vector3(x, 0, y);
            trePos.localPosition = new Vector3(0, 0, -y);

            if (_model.CityDisplay == SetupModel.CityDisplayMode.Mesh)
            {
                DestroyImmediate(two.gameObject);
                tre.SetActive(true);
                FindObjectOfType<CameraArea>().Camera = treCam;
            }
            else
            {
                two.SetActive(true);
                DestroyImmediate(tre.gameObject);
                FindObjectOfType<CameraArea>().Camera = twoCam;
            }

            var miniTex = copyAsset(MinimapTexture, FOLDER_UI, "MinimapTexture.renderTexture");
            var miniMap = FindObjectOfType<Minimap>();
            var miniCam = GameObject.Find("MinimapCamera").GetComponent<Camera>();
            miniCam.targetTexture = miniTex;
            miniMap.Image.texture = miniCam.targetTexture;
            miniMap.Camera = miniCam;
            miniMap.Camera.orthographic = _model.MapDisplay == SetupModel.MapDisplayMode.Sprite;

            if (_model.MapLayout == SetupModel.MapLayoutMode.Isometric && _model.CityDisplay == SetupModel.CityDisplayMode.Sprite)
            {
                if (_model.MapAxis == SetupModel.MapAxisMode.XY)
                    twoCon.SortAxis = new Vector3(0, 1, 0);
                else
                    twoCon.SortAxis = new Vector3(0, 0, 1);
            }
        }

        private TileBase setupGrid()
        {
            Sprite sprite;
            if (_model.MapLayout == SetupModel.MapLayoutMode.Rectangle)
            {
                sprite = copyAsset(RectBlank, FOLDER_PALETTE);
                if (_model.Scale != 1)
                    changePixelsPerUnit(sprite, 4 / _model.Scale);
            }
            else
            {
                sprite = copyAsset(IsoBlank, FOLDER_PALETTE);
                if (_model.Scale != 1)
                    changePixelsPerUnit(sprite, 128 / _model.Scale);
            }

            var infoTile = copyAsset(InfoTile, FOLDER_PALETTE);
            var validTile = copyAsset(ValidTile, FOLDER_PALETTE);
            var invalidTile = copyAsset(InvalidTile, FOLDER_PALETTE);
            var groundTile = copyAsset(GroundTile, FOLDER_PALETTE);
            var groundBlockedTile = copyAsset(GroundBlockedTile, FOLDER_PALETTE);

            TileBase roadTile;
            if (_model.MapDisplay == SetupModel.MapDisplayMode.Sprite)
            {
                roadTile = copyAsset(RoadTile, FOLDER_PALETTE);
            }
            else
            {
                var roadObjectTile = copyAsset(RoadObjectTile, FOLDER_PALETTE);
                roadObjectTile.Prefab = copyAsset(RoadObjectTile.Prefab, FOLDER_PALETTE);
                var scale = _model.CellSize;
                if (_model.MapAxis == SetupModel.MapAxisMode.XY)
                    scale = new Vector3(scale.x, scale.y, scale.z / 100f);
                else
                    scale = new Vector3(scale.x, scale.y / 100f, scale.z);
                roadObjectTile.Prefab.transform.localScale = scale;
                roadTile = roadObjectTile;
            }

            var tiles = new[] { InfoTile, validTile, invalidTile, groundTile, groundBlockedTile, roadTile };

            foreach (var tile in tiles.OfType<Tile>())
            {
                tile.sprite = sprite;
            }

            var highlightManager = FindObjectOfType<DefaultHighlightManager>();
            highlightManager.InfoTile = infoTile;
            highlightManager.ValidTile = validTile;
            highlightManager.InvalidTile = invalidTile;

            var grid = FindObjectOfType<Grid>();

            var ground = GameObject.Find("Ground");
            var groundTiles = ground.GetComponent<Tilemap>();

            grid.cellLayout = _model.CellLayout;
            grid.cellSwizzle = _model.CellSwizzle;
            grid.cellSize = _model.CellSize;

            if (_model.CellLayout == GridLayout.CellLayout.Rectangle)
            {
                var map = grid.gameObject.AddComponent<DefaultMap>();
                map.Size = _model.MapSize;
                map.Ground = groundTiles;
                map.WalkingBlockingTiles = new TileBase[] { groundBlockedTile };
                map.BuildingBlockingTiles = new BlockingTile[] { new BlockingTile() { Level = new StructureLevelMask(), Tile = groundBlockedTile } };
            }
            else
            {
                var map = grid.gameObject.AddComponent<IsometricMap>();
                map.Size = _model.MapSize;
                map.Ground = groundTiles;
                map.WalkingBlockingTiles = new TileBase[] { groundBlockedTile };
                map.BuildingBlockingTiles = new BlockingTile[] { new BlockingTile() { Level = new StructureLevelMask(), Tile = groundBlockedTile } };
            }

            for (int x = 0; x < _model.MapSize.x; x++)
            {
                for (int y = 0; y < _model.MapSize.y; y++)
                {
                    groundTiles.SetTile(new Vector3Int(x, y, 0), y < _model.MapSize.y / 4 ? groundBlockedTile : groundTile);
                }
            }

            foreach (var tilemap in FindObjectsOfType<Tilemap>())
            {
                tilemap.orientation = _model.TilemapOrientation;
            }

            var palette = copyAsset(GroundPalette, FOLDER_PALETTE, "Ground.prefab");

            var paletteMap = palette.GetComponentInChildren<Tilemap>();

            paletteMap.SetTile(new Vector3Int(0, 0, 0), groundTile);
            paletteMap.SetTile(new Vector3Int(1, 0, 0), groundBlockedTile);

            return roadTile;
        }

        private void setupMap()
        {
            var ground = GameObject.Find("Ground");
            var map = GameObject.Find("Map");

            var walkable = map.transform.Find("Walkable");
            var blocked = map.transform.Find("Blocked");

            var groundTiles = ground.GetComponent<TilemapRenderer>();

            var groundMaterial = copyAsset(GroundMaterial, FOLDER_MATERIALS);
            var blockedMaterial = copyAsset(BlockedMaterial, FOLDER_MATERIALS);
            var overlayMaterial = copyAsset(OverlayMaterial, FOLDER_MATERIALS);

            walkable.GetComponent<MeshRenderer>().material = groundMaterial;
            blocked.GetComponent<MeshRenderer>().material = blockedMaterial;
            FindObjectOfType<CameraArea>().GetComponent<LineRenderer>().material = overlayMaterial;
            GameObject.Find("Highlights").GetComponent<TilemapRenderer>().material = overlayMaterial;

            groundTiles.enabled = _model.MapDisplay == SetupModel.MapDisplayMode.Sprite;
            map.SetActive(_model.MapDisplay != SetupModel.MapDisplayMode.Sprite);

            map.transform.rotation = Quaternion.Euler(_model.MapAxis == SetupModel.MapAxisMode.XY ? 0 : 90, 0, 0);

            walkable.localPosition = new Vector3(_model.MapSize.x * _model.Scale / 2f, _model.MapSize.y * _model.Scale / 2f, 0);
            walkable.localScale = new Vector3(_model.MapSize.x * _model.Scale / 10f, 1, _model.MapSize.y * _model.Scale / 10f);

            blocked.localPosition = new Vector3(walkable.localPosition.x, walkable.localPosition.y / 4f, -_model.MapSize.y * _model.Scale / 32f);
            blocked.localScale = new Vector3(_model.MapSize.x * _model.Scale, _model.MapSize.y * _model.Scale / 4f, _model.MapSize.y * _model.Scale / 16f);
        }

        private Road setupRoad(Item item, TileBase roadTile)
        {
            var road = new Road() { Key = "ROD", Name = "Road", Cost = new[] { new ItemQuantity(item, 1) } };
            road.Stages = new[] { new RoadStage() { Tile = roadTile } };
            AssetDatabase.CreateAsset(road, _model.GetAssetPath("Road.asset", FOLDER_CITY_ROADS));

            var roads = new RoadSet() { Objects = new[] { road } };
            AssetDatabase.CreateAsset(roads, _model.GetAssetPath("Roads.asset", FOLDER_CITY_ROADS));
            FindObjectOfType<ObjectRepository>().Roads = roads;

            FindObjectOfType<RoadBuilder>().Road = road;

            return road;
        }

        private Item setupItem()
        {
            var item = new Item() { Key = "ITM", Name = "Item" };
            AssetDatabase.CreateAsset(item, _model.GetAssetPath("Item.asset", FOLDER_CITY_ITEMS));

            var items = new ItemSet() { Objects = new[] { item } };
            AssetDatabase.CreateAsset(items, _model.GetAssetPath("Items.asset", FOLDER_CITY_ITEMS));
            FindObjectOfType<ObjectRepository>().Items = items;

            FindObjectOfType<DefaultItemManager>().StartItems = new[] { new ItemQuantity(item, 100) };
            FindObjectOfType<InventoryVisualizer>().Item = item;

            return item;
        }

        private Building setupBuilding(Walker walker, Item item)
        {
            var building = copyAsset(Building, FOLDER_CITY_BUILDINGS, "Building.prefab");
            var buildingInfo = copyAsset(BuildingInfo, FOLDER_CITY_BUILDINGS, "BuildingInfo.asset");

            building.Info = buildingInfo;
            building.GetBuildingComponent<WalkerComponent>().CyclicRoamingWalkers.Prefab = (RoamingWalker)walker;
            buildingInfo.Prefab = building;
            buildingInfo.Cost = new[] { new ItemQuantity(item, 10) };

            var pivotXY = building.transform.Find("PivotXY");
            var pivotXZ = building.transform.Find("PivotXZ");

            var buildingSprite = copyAsset(_model.MapLayout == SetupModel.MapLayoutMode.Rectangle ? BuildingSprite : BuildingSpriteIso, FOLDER_SPRITES, "building.png");
            if (_model.Scale != 1)
                changePixelsPerUnit(buildingSprite, 16 / _model.Scale);

            if (_model.CityDisplay == SetupModel.CityDisplayMode.Mesh)
            {
                var buildingMesh = copyAsset(BuildingMesh, FOLDER_MODELS, "building.fbx");
                foreach (var sprite in building.GetComponentsInChildren<SpriteRenderer>())
                {
                    DestroyImmediate(sprite.gameObject, true);
                }
                foreach (var mesh in building.GetComponentsInChildren<MeshFilter>())
                {
                    mesh.mesh = buildingMesh;
                }
            }
            else
            {
                foreach (var sprite in building.GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.sprite = buildingSprite;
                }
                foreach (var mesh in building.GetComponentsInChildren<MeshFilter>())
                {
                    DestroyImmediate(mesh.gameObject, true);
                }
            }

            if (_model.MapAxis == SetupModel.MapAxisMode.XY)
            {
                DestroyImmediate(pivotXZ.gameObject, true);
                building.Pivot = pivotXY;
            }
            else
            {
                DestroyImmediate(pivotXY.gameObject, true);
                building.Pivot = pivotXZ;
            }

            building.Pivot.localPosition *= _model.Scale;
            if (_model.MapLayout == SetupModel.MapLayoutMode.Isometric)
                building.Pivot.localPosition = new Vector3(0, building.Pivot.localPosition.y * 0.5f, building.Pivot.localPosition.z * 0.5f);
            if (_model.CityDisplay == SetupModel.CityDisplayMode.Mesh)
                building.Pivot.localScale *= _model.Scale;

            var buildings = new BuildingInfoSet() { Objects = new[] { buildingInfo } };
            AssetDatabase.CreateAsset(buildings, _model.GetAssetPath("Buildings.asset", FOLDER_CITY_BUILDINGS));
            FindObjectOfType<ObjectRepository>().Buildings = buildings;

            FindObjectOfType<BuildingBuilder>().BuildingInfo = buildingInfo;
            FindObjectOfType<BuildingBuilder>().GetComponentInChildren<Image>().sprite = buildingSprite;
            FindObjectOfType<DemolishTool>().GetComponentInChildren<Image>().sprite = buildingSprite;

            return building;
        }

        private Walker setupWalker()
        {
            var walker = copyAsset(Walker, FOLDER_CITY_WALKERS, "Walker.prefab");
            var walkerInfo = copyAsset(WalkerInfo, FOLDER_CITY_WALKERS, "WalkerInfo.asset");

            walker.Speed *= _model.Scale;
            walker.Info = walkerInfo;

            var pivotXY = walker.transform.Find("PivotXY");
            var pivotXZ = walker.transform.Find("PivotXZ");

            var walkerSprite = copyAsset(_model.MapLayout == SetupModel.MapLayoutMode.Rectangle ? WalkerSprite : WalkerSpriteIso, FOLDER_SPRITES, "walker.png");
            if (_model.Scale != 1)
                changePixelsPerUnit(walkerSprite, 16 / _model.Scale);

            if (_model.CityDisplay == SetupModel.CityDisplayMode.Mesh)
            {
                var walkerMesh = copyAsset(WalkerMesh, FOLDER_MODELS, "walker.fbx");
                foreach (var sprite in walker.GetComponentsInChildren<SpriteRenderer>())
                {
                    DestroyImmediate(sprite.gameObject, true);
                }
                foreach (var mesh in walker.GetComponentsInChildren<MeshFilter>())
                {
                    mesh.mesh = walkerMesh;
                }
            }
            else
            {
                foreach (var sprite in walker.GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.sprite = walkerSprite;
                }
                foreach (var mesh in walker.GetComponentsInChildren<MeshFilter>())
                {
                    DestroyImmediate(mesh.gameObject, true);
                }
            }

            if (_model.MapAxis == SetupModel.MapAxisMode.XY)
            {
                DestroyImmediate(pivotXZ.gameObject, true);
                walker.Pivot = pivotXY;
            }
            else
            {
                DestroyImmediate(pivotXY.gameObject, true);
                walker.Pivot = pivotXZ;
            }

            walker.Pivot.localPosition *= _model.Scale;
            if (_model.MapLayout == SetupModel.MapLayoutMode.Isometric)
                walker.Pivot.localPosition = new Vector3(0, walker.Pivot.localPosition.y * 0.5f, walker.Pivot.localPosition.z * 0.5f);
            if (_model.CityDisplay == SetupModel.CityDisplayMode.Mesh)
                walker.Pivot.localScale *= _model.Scale;

            FindObjectOfType<RoadBuilder>().GetComponentInChildren<Image>().sprite = walkerSprite;

            return walker;
        }

        private void setupUI()
        {
            var font = copyAsset(Font, FOLDER_UI, "DefaultFont.asset");

            foreach (var text in FindObjectsOfType<TMPro.TMP_Text>())
            {
                text.font = font;
            }
        }

        private void createFolder(string parent, string name)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(parent, name)))
                return;
            AssetDatabase.CreateFolder(parent, name);
        }

        private void createFolder(string parent, string subfolder, string name)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(parent, subfolder, name)))
                return;
            AssetDatabase.CreateFolder(Path.Combine(parent, subfolder), name);
        }

        private T copyAsset<T>(T asset, string folder, string name = null) where T : Object
        {
            string originalPath = AssetDatabase.GetAssetPath(asset);
            string copyPath = _model.GetAssetPath(name ?? Path.GetFileName(originalPath), folder);
            AssetDatabase.CopyAsset(originalPath, copyPath);
            return AssetDatabase.LoadAssetAtPath<T>(copyPath);
        }

        private void changePixelsPerUnit(Sprite sprite, float value)
        {
            string path = AssetDatabase.GetAssetPath(sprite.texture);
            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
            TextureImporterSettings texSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(texSettings);
            texSettings.spritePixelsPerUnit = value;
            ti.SetTextureSettings(texSettings);
            ti.SaveAndReimport();
        }
    }
}
