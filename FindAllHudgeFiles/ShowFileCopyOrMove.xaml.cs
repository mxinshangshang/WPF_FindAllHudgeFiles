using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FindAllHudgeFiles
{
    /// <summary>
    /// Interaction logic for ShowFileCopyOrMove.xaml
    /// </summary>
    public partial class ShowFileCopyOrMove : Window
    {
        public ShowFileCopyOrMove()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 线程安全的显示字串属性
        /// </summary>
        public string Information
        {
            set
            {
                Action<string> del = (info) =>
                    {
                        lblInfo.Text = info;
                    };
                Dispatcher.Invoke(del, value);
            }
        }

       
    }
}
