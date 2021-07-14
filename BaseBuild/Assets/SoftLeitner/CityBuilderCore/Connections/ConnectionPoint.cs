namespace CityBuilderCore
{
    public class ConnectionPoint
    {
        public IConnectionPasser Passer { get; }
        public int Value { get; set; }

        public ConnectionPoint(IConnectionPasser passer)
        {
            Passer = passer;
            Value = -1;
        }
    }
}
