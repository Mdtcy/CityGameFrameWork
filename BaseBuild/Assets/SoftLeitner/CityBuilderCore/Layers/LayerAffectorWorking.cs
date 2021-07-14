namespace CityBuilderCore
{
    /// <summary>
    /// special layeraffector that only affects as long as its building is working
    /// </summary>
    public class LayerAffectorWorking : LayerAffector
    {
        public override bool IsAffecting => _isWorking;

        public IBuilding Building { get; private set; }

        private bool _isWorking;

        protected override void Start()
        {
            base.Start();

            Building = GetComponent<IBuilding>() ?? GetComponentInParent<IBuilding>();
        }

        private void Update()
        {
            if (_isWorking != Building.IsWorking)
            {
                _isWorking = Building.IsWorking;
                checkAffector();
            }
        }
    }
}