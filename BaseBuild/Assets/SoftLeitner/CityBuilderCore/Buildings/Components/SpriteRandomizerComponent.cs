using UnityEngine;

namespace CityBuilderCore
{
    public class SpriteRandomizerComponent : BuildingComponent
    {
        public override string Key => "SRA";

        public SpriteRenderer Renderer;
        public Sprite[] Sprites;

        private int _index;

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            _index = Random.Range(0, Sprites.Length);
            Renderer.sprite = Sprites[_index];
        }
        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            var replacementComponent = replacement.GetBuildingComponent<SpriteRandomizerComponent>();

            if (replacementComponent != null && replacementComponent.Sprites.Length == Sprites.Length)
            {
                replacementComponent._index = _index;
                replacementComponent.Renderer.sprite = replacementComponent.Sprites[_index];
            }

        }

        public override string SaveData()
        {
            return _index.ToString();
        }
        public override void LoadData(string json)
        {
            _index = int.Parse(json);
            Renderer.sprite = Sprites[_index];
        }
    }
}
