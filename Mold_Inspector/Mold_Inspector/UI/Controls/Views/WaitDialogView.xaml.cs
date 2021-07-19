using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mold_Inspector.UI.Controls.Views
{
    /// <summary>
    /// WaitWindowView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WaitDialogView : ModernDialog
    {
        public WaitDialogView() : base()
        {
            InitializeComponent();
        }

        public static bool ShowWait(
            string title,
            Func<bool> check)
        {
            var dlg = new ModernDialog
            {
                Title = title,
                Content = new ModernProgressRing() { IsActive = true },
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = 480,
                MaxWidth = 640,
            };

            dlg.Buttons = new Button[] { dlg.CancelButton };
            var cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (cts.IsCancellationRequested == false)
                {
                    if (check())
                        break;

                    await Task.Delay(100);
                    continue;
                }

                dlg.Dispatcher.Invoke(dlg.Close);
            });

            dlg.ShowDialog();
            cts.Cancel();

            return dlg.MessageBoxResult == MessageBoxResult.None ? true : false;
        }
    }
}
