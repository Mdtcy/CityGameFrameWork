namespace CityBuilderCore
{
    public class StructureReference : StructureReferenceBase<IStructure>
    {
        public StructureReference(IStructure structure) : base(structure) { }

        public override void Replace(IStructure replacement)
        {
            base.Replace(replacement);

            replacement.StructureReference = this;
        }
    }
}