using NUnit.Framework;
using System.Linq;

namespace CityBuilderCore.Tests
{
    public class ItemRecipientChecker : CheckerBase
    {
        public EvolutionComponent EvolutionComponent;
        public Item Item;
        public int ExpectedQuantity;
        public int ActualQuantity => EvolutionComponent.GetItems()?.FirstOrDefault(i => i.Item == Item)?.Quantity ?? 0;

        public override void Check()
        {
            Assert.AreEqual(ExpectedQuantity, ActualQuantity, name);
        }
    }
}