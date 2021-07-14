using UnityEngine;

namespace CityBuilderCore.Tests
{
    public class DebugBuilding : Building
    {
        public override Vector2Int Size => Info == null ? Vector2Int.one : base.Size;
        public override Vector2Int Point => Info == null ? Dependencies.Get<IGridPositions>().GetGridPosition(transform.position) : base.Point;

        public override bool IsWalkable => true;

        protected override void Awake()
        {
            Components.ForEach(c => c.Building = this);
        }

        protected override void Start()
        {
            if (StructureReference == null)
                Initialize();
        }

        public override void Initialize()
        {
            StructureReference = new StructureReference(this);
            BuildingReference = new BuildingReference(this);

            Dependencies.GetOptional<IStructureManager>()?.RegisterStructure(this);
            Dependencies.GetOptional<IBuildingManager>()?.RegisterBuilding(this);

            Components.ForEach(c => c.InitializeComponent());
        }

        public override void Terminate()
        {
            Components.ForEach(c => c.TerminateComponent());

            Destroy(gameObject);
        }

        public override Vector2Int? GetAccessPoint(PathType type, object tag = null) => Info == null ? Point : base.GetAccessPoint(type, tag);
    }
}