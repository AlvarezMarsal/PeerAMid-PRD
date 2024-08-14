using PeerAMid.Core;
using PeerAMid.DataAccess;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace YardStickPortal;

public static class UnityConfig
{
    public static void RegisterComponents()
    {
        var container = new UnityContainer();

        // register all your components with the container here
        // it is NOT necessary to register your controllers

        // e.g. container.RegisterType<ITestService, TestService>();
        container.RegisterType<IHomeDataAccess, HomeDataAccess>();
        container.RegisterType<IHomeCore, HomeCore>();
        container.RegisterType<IActualDataCollectionDataAccess, ActualDataCollectionDataAccess>();
        container.RegisterType<IActualDataCollectionCore, ActualDataCollectionCore>();
        container.RegisterType<IPeerAMidDataAccess, PeerAMidDataAccess>();
        container.RegisterType<IPeerAMidCore, PeerAMidCore>();

        DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
    }
}
