   M S B A M L   `   `   `  ���� L  HFindAllHudgeFiles, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:FindAllHudgeFilesFindAllHudgeFiles  "    FindAllHudgeFiles.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     1localclr-namespace:FindAllHudgeFiles     -     � WindowStartupLocation$  CenterScreen=� � Title$> 8Hi，Big Files, where are you?-[金旭亮开发 ver 1.0]��$	��571��$	��501�� � 	Resources '  !FindAllHudgeFiles.NumberConverter   FileSizeConverter��%   &	          .��]� $��Stretch=�$��Stretch=�$��10q�.+��� -    ��Name$ 	statusBar��$��Stretch=�$��Bottom=�.@��� -    ��Name$ lblInfo����  �� Orientation$ 
Horizontal=�$��10q�$��Bottom=�$��Center=�.+��� -    ��Name$ 	btnDelete��$��删除��$��10q�$
��Left=�$	��Top=�$	��100���� -   $ btnMove��$��移动��$��10q�$
��Left=�$	��Top=�$	��100���� -   $ btnOpen��$��打开��$��10q�$
��Left=�$	��Top=�$	��100���� -   $ btnOpenFolder��$��打开所在文件夹��$��10q�$
��Left=�$	��Top=�$	��100���� $��设置搜索参数��$	��Top=�$��10q�$��Stretch=�$	��Top=�$	��119��.���  � ColumnDefinitions �� �� $
��421* �.+��� $
��Left=�$��	25,22,0,0q�$
��Wrap=�$��我要查找大于��$	��Top=�$��1���� -    ��Name$ txtFileSize��$	��100��$
��Left=�$��23��$��
102,19,0,0q�	 �� TextWrapping$
	 Wrap=�$	��Top=�$	��120��$��1���� $
��Left=�$��
238,22,0,0q�$
��Wrap=�$��MB的文件。��$	��Top=�$��1���� -	   $ btnBeginSearch��$��开始搜索��$
��Left=�$��
310,18,0,0q�$	��Top=�$��75��$��23��$��1���� -
   $ btnCancelSearch��$��取消搜索��$
��Left=�$��23��$��
410,19,0,0q�$	��Top=�$��75��$��1���� $
��Left=�$��18��$��	25,59,0,0q�$
��Wrap=�$��搜索位置：��$	��Top=�$��74��$��1���� -   $ txtLocation��$
��Left=�$��27��$��
102,56,0,0q�$
	 Wrap=�$	��C:\��$	��Top=�$	��308��$��1���� -   $ btnChooseLocation��$	��...��$
��Left=�$��430,59,-51,0q�$	��Top=�$��42��$��1��&   System.Windows.Controls.DataGrid  -   
  Name$
 dgFiles��$��Stretch=�$��10q�$��Stretch=�   AutoGenerateColumns .     Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 	文件名��   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Name+  $ 大小��$ *  �� *Length+ �� 	Converter# [    $ 位置��$ 2*  �� *
Location+