using Build.Map;
using Zenject;

public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
{
    public IsometricMap map;

    public override void InstallBindings()
    {
        // base.InstallBindings();

        Container.Bind<IMap>().FromInstance(map).AsSingle();
        Container.Bind<IGridPositions>().FromInstance(map).AsSingle();
        Container.Bind<IGridRotations>().FromInstance(map).AsSingle();
    }
}
