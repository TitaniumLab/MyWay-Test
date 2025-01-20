using MyWay;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private DataLoader _dataLoader;

    public override void InstallBindings()
    {
        Container.Bind<DataLoader>().FromInstance(_dataLoader).AsSingle();
    }
}