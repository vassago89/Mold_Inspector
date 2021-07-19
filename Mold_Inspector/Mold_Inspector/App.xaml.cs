using Prism.Ioc;
using Prism.Unity;
using Serilog;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;
using WPFLocalizeExtension.Extensions;
using Mold_Inspector.UI.Shell.Views;
using Mold_Inspector.Store;
using Mold_Inspector.Device;
using Mold_Inspector.Config;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store.Process;
using Mold_Inspector.Service;
using Mold_Inspector.Device.IO;
using Mold_Inspector.Device.IO.Serial;

namespace Mold_Inspector
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : PrismApplication
    {
        private CancellationTokenSource _cts;
        //private ILogger _logger;

        protected override void OnExit(ExitEventArgs e)
        {
            //Container.Resolve<CameraStore>().Save();
            //Container.Resolve<RecipeStore>().Save();
            //Container.Resolve<CommonStore>().Save();

            Container.Resolve<CameraServiceMediator>().Disconnect();
            Container.Resolve<IOService>().Release();
            
            //File.WriteAllText("CoreConfig.json", JsonConvert.SerializeObject(Container.Resolve<CoreConfig>()));

            //Container.Resolve<InspectService>().Disconnect();
            //Container.Resolve<GrabService>().Disconnect();

            //_logger.Info("Exit");
            //base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.File(path: "MyApp.log")
                .CreateLogger();

            var provider = LocalizeDictionary.Instance.DefaultProvider as ResxLocalizationProvider;
            ResxLocalizationProvider.SetDefaultAssembly(ResxLocalizationProvider.Instance, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            ResxLocalizationProvider.SetDefaultDictionary(ResxLocalizationProvider.Instance, nameof(Mold_Inspector.Properties.Resources));

            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            //LocalizeDictionary.Instance.Culture = new CultureInfo("ko-KR");
            LocalizeDictionary.Instance.Culture = new CultureInfo("en-US");

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry
                .RegisterSingleton<IOController, KM6050IOController>();

            containerRegistry
                .RegisterSingleton<IOService>()
                .RegisterSingleton<CameraServiceMediator>();

            containerRegistry
                .RegisterSingleton<StateStore>()
                .RegisterSingleton<PageStore>()
                .RegisterSingleton<CameraStore>()
                .RegisterSingleton<RecipeStore>()
                .RegisterSingleton<ImageStore>()
                .RegisterSingleton<TeachingStore>()
                .RegisterSingleton<IOStore>()
                .RegisterSingleton<TeachingStore>()
                .RegisterSingleton<MouseStore>()
                .RegisterSingleton<CommonStore>()
                .RegisterSingleton<ResultStore>()
                .RegisterSingleton<InspectStore>()
                .RegisterSingleton<ManualProcessStore>()
                .RegisterSingleton<CommandStack>()
                .RegisterSingleton<DefaultStore>();

            var controller = Container.Resolve<IOController>() as KM6050IOController;
            if (controller != null)
            {
                var ioStore = Container.Resolve<IOStore>();
                controller.Connect(ioStore.PortName, ioStore.BaudRate, ioStore.Parity, ioStore.DataBits, ioStore.StopBits);
                controller.StartMonitoring(100);
            }
        }

        protected override Window CreateShell()
        {
            return new ShellView();
        }
    }
}
