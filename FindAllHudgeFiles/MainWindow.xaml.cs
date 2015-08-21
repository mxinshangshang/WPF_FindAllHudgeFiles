using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FindAllHudgeFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //用于选择起始文件夹
        private FolderBrowserDialog folderBrowserDialog = null;


        private ObservableCollection<FoundFile> FoundFiles = null;

        private CancellationTokenSource cts = null;
        private ShowFileCopyOrMove win = null;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        Action<String> showInfo = null;
        Action<bool> EnableSearchButton = null;
        private void Init()
        {
            folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "选择要搜索的位置";

            FoundFiles = new ObservableCollection<FoundFile>();
            dgFiles.ItemsSource = FoundFiles;

            showInfo = (info) =>
              {
                  lblInfo.Text = info;

              };
            EnableSearchButton = (Enabled) =>
                {
                    btnBeginSearch.IsEnabled = Enabled;
                };

        }
        /// <summary>
        /// 用于启动查找文件的过程
        /// </summary>
        private void searchFiles(String Location, long fileLength)
        {
            DirectoryInfo dir = null;
            List<FileInfo> files = null;
            FoundFile foundFile = null;
            try
            {
                dir = new DirectoryInfo(Location);
                Dispatcher.Invoke(showInfo, Location);
                if (cts.IsCancellationRequested)
                {
                    //Dispatcher.Invoke(EnableSearchButton, true);
                    return;
                }
                //启动并行查找
                var query = (from file in dir.GetFiles("*.*", SearchOption.TopDirectoryOnly).AsParallel()
                             where file.Length >= fileLength
                             select file).WithCancellation(cts.Token);

                files = query.ToList();


                //更新显示
                if (files != null)
                {
                    foreach (var item in files)
                    {
                        //如果收到取消请求
                        if (cts.IsCancellationRequested)
                        {
                            //Dispatcher.Invoke(showInfo, "搜索已取消");
                            //Dispatcher.Invoke(EnableSearchButton, true);
                            return;
                        }
                        foundFile = new FoundFile()
                        {
                            Name = item.Name,
                            Length = item.Length,
                            Location = item.DirectoryName
                        };
                        Action<FoundFile> addFileDelegate = (file) =>
                        {
                            FoundFiles.Add(file);
                        };

                        Dispatcher.BeginInvoke(addFileDelegate, foundFile);
                    }
                }
                //递归查找每个子文件夹
                foreach (var directory in dir.GetDirectories())
                {
                    searchFiles(directory.FullName, fileLength);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Dispatcher.Invoke(showInfo, dir.Name + "无权限访问");

            }


        }



        private void btnChooseLocation_Click(object sender, RoutedEventArgs e)
        {
            ChooseLocation();
        }
        /// <summary>
        /// 选择搜索起始路径
        /// </summary>
        private void ChooseLocation()
        {
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnBeginSearch_Click(object sender, RoutedEventArgs e)
        {
            lblInfo.Text = "正在查找";
            FoundFiles.Clear();
            cts = new CancellationTokenSource();
            //禁止启动第二个搜索
            btnBeginSearch.IsEnabled = false;
            String location = txtLocation.Text;
            long length = Convert.ToInt32(txtFileSize.Text) * 1024 * 1024;
            Task tsk = new Task(() =>
            {

                //开始搜索
                searchFiles(location, length);
                if (!cts.IsCancellationRequested)

                    Dispatcher.Invoke(showInfo, "搜索完成");


                else
                    Dispatcher.Invoke(showInfo, "搜索被取消");
                Dispatcher.Invoke(EnableSearchButton, true);
            });
            try
            {
                tsk.Start();

            }

            catch (Exception ex)
            {
                lblInfo.Text = ex.Message;
                //如果收到取消请求
                if (cts.IsCancellationRequested)
                {
                    Dispatcher.Invoke(showInfo, "搜索已取消");
                    Dispatcher.Invoke(EnableSearchButton, true);
                    return;
                }
                else
                    Dispatcher.Invoke(EnableSearchButton, true);
            }
        }

        private void btnCancelSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
                cts.Cancel();

            EnableSearchButton(true);
            showInfo("搜索己取消");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedFile();
        }
        /// <summary>
        /// 删除网格中选中的文件
        /// </summary>
        private void DeleteSelectedFile()
        {
            int index = dgFiles.SelectedIndex;
            if (index != -1 && index < FoundFiles.Count)
            {
                try
                {
                    String FileName = FoundFiles[index].Name;
                    String FileToBeDelete = FoundFiles[index].Location + @"\\" + FileName;
                    File.Delete(FileToBeDelete);
                    FoundFiles.RemoveAt(index);
                    lblInfo.Text = "文件\"" + FileName + "\"已删除";
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            MoveFile();
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        private void MoveFile()
        {
            int index = dgFiles.SelectedIndex;
            if (index == -1 || index >= FoundFiles.Count)
            {
                return;
            }
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    String FileName = FoundFiles[index].Name;
                    String ChoosedFile = FoundFiles[index].Location + @"\\" + FileName;
                    String toDir = folderBrowserDialog.SelectedPath;
                    String toDirFile = toDir.EndsWith("\\") ? toDir + FileName : toDir + @"\\" + FileName;

                    //不允许再进行文件操作
                    //EnableButton(false);
                    //异步执行文件移动
                    Task tsk = new Task(() =>
                    {

                        Action del = () =>
                        {
                            win = new ShowFileCopyOrMove();
                            win.Information = "\"" + FileName + "\"正在移动中...";
                            win.Show();
                        };
                        Dispatcher.Invoke(del);
                        File.Move(ChoosedFile, toDirFile);
                        FoundFiles[index].Location = toDir;
                        // Dispatcher.Invoke(showInfo, "\"" + FileName + "\"文件移动完成");
                        del = () =>
                        {

                            win.Close();
                            win = null;
                        };
                        //Action<bool> del = EnableButton;
                        //Dispatcher.Invoke(del, true);
                        Dispatcher.Invoke(del);
                    });
                    tsk.Start();
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.ToString());
                }
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }
        /// <summary>
        /// 打开选中的文件
        /// </summary>
        private void OpenFile()
        {
            int index = dgFiles.SelectedIndex;
            if (index != -1 && index < FoundFiles.Count)
            {
                try
                {

                    String ChoosedFile = FoundFiles[index].Location + @"\\" + FoundFiles[index].Name;
                    Process.Start(ChoosedFile);

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 当文件操作正在进行时，激活或屏蔽掉按钮
        /// </summary>
        /// <param name="IsEnabled"></param>
        //private void EnableButton(bool IsEnabled)
        //{
        //    btnOpen.IsEnabled = IsEnabled;
        //    btnDelete.IsEnabled = IsEnabled;
        //    btnMove.IsEnabled = IsEnabled;
        //    btnOpenFolder.IsEnabled = IsEnabled;
        //}

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder();
        }

        /// <summary>
        /// 打开选中的文件所在的文件夹
        /// </summary>
        private void OpenFolder()
        {
            int index = dgFiles.SelectedIndex;
            if (index != -1 && index < FoundFiles.Count)
            {
                try
                {
                    String ChoosedFile = FoundFiles[index].Location;
                    Process.Start(ChoosedFile);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (win != null)
                win.Close();
        }
    }
}
