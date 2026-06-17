using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DesktopWidget
{
    public partial class SettingsWindow : Window
    {
        private AppSettings settings = null!;
        private MainWindow? mainWindow;

        public SettingsWindow()
        {
            InitializeComponent();
            
            // 设置窗口位置在屏幕中央
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // 允许拖动窗口
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            
            // 加载设置
            LoadSettings();
        }

        private void LoadSettings()
        {
            settings = AppSettings.Load();
            
            // 默认显示外观设置面板
            SwitchToPanel(AppearancePanel);
            
            // 获取主窗口引用
            mainWindow = Application.Current.MainWindow as MainWindow;
            
            // 设置控件值
            ThemeColorTextBox.Text = settings.ThemeColor;
            ShowInTaskbarCheckBox.IsChecked = settings.ShowInTaskbar;
            StartWithWindowsCheckBox.IsChecked = settings.StartWithWindows;
            IsTopmostCheckBox.IsChecked = settings.IsTopmost;
            
            // 设置各元素的透明度
            BackgroundOpacitySlider.Value = settings.BackgroundOpacity;
            BackgroundOpacityText.Text = $"{(int)(settings.BackgroundOpacity * 100)}%";
            
            TextOpacitySlider.Value = settings.TextOpacity;
            TextOpacityText.Text = $"{(int)(settings.TextOpacity * 100)}%";
            
            LeftAvatarOpacitySlider.Value = settings.LeftAvatarOpacity;
            LeftAvatarOpacityText.Text = $"{(int)(settings.LeftAvatarOpacity * 100)}%";
            
            RightAvatarOpacitySlider.Value = settings.RightAvatarOpacity;
            RightAvatarOpacityText.Text = $"{(int)(settings.RightAvatarOpacity * 100)}%";
            
            CenterHeartOpacitySlider.Value = settings.CenterHeartOpacity;
            CenterHeartOpacityText.Text = $"{(int)(settings.CenterHeartOpacity * 100)}%";
            
            // 设置圆角角度
            CornerRadiusSlider.Value = settings.CornerRadius;
            CornerRadiusText.Text = $"{(int)settings.CornerRadius}";
            
            // 设置新增控件的值
            SetComboBoxValue(FontSizeComboBox, settings.FontSize);
            SetComboBoxValue(PositionComboBox, settings.Position);
            ShowSecondsCheckBox.IsChecked = settings.ShowSeconds;
            
            // 设置自定义文本
            CustomTextTextBox.Text = settings.CustomText;
            
            // 设置时间控件的值
            StartDatePicker.SelectedDate = settings.StartTime.Date;
            StartTimeTextBox.Text = settings.StartTime.ToString("HH:mm:ss");
            
            // 设置头像路径
            LeftAvatarPathTextBox.Text = settings.LeftAvatarPath;
            RightAvatarPathTextBox.Text = settings.RightAvatarPath;
            
            // 设置头像名称
            LeftAvatarNameTextBox.Text = settings.LeftAvatarName;
            RightAvatarNameTextBox.Text = settings.RightAvatarName;
            
            // 设置头像模式选择
            LeftAvatarLocalModeRadioButton.IsChecked = settings.IsLeftAvatarLocal;
            LeftAvatarOnlineModeRadioButton.IsChecked = !settings.IsLeftAvatarLocal;
            RightAvatarLocalModeRadioButton.IsChecked = settings.IsRightAvatarLocal;
            RightAvatarOnlineModeRadioButton.IsChecked = !settings.IsRightAvatarLocal;
            
            // 添加事件处理
            BackgroundOpacitySlider.ValueChanged += (s, e) => BackgroundOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            TextOpacitySlider.ValueChanged += (s, e) => TextOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            LeftAvatarOpacitySlider.ValueChanged += (s, e) => LeftAvatarOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            RightAvatarOpacitySlider.ValueChanged += (s, e) => RightAvatarOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            CenterHeartOpacitySlider.ValueChanged += (s, e) => CenterHeartOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            CornerRadiusSlider.ValueChanged += (s, e) => CornerRadiusText.Text = $"{(int)e.NewValue}";
            
            // 添加头像模式选择事件处理
            LeftAvatarLocalModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(LeftAvatarPathTextBox, BrowseLeftAvatarButton, LeftAvatarLocalModeRadioButton.IsChecked == true);
            LeftAvatarOnlineModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(LeftAvatarPathTextBox, BrowseLeftAvatarButton, LeftAvatarLocalModeRadioButton.IsChecked == true);
            RightAvatarLocalModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(RightAvatarPathTextBox, BrowseRightAvatarButton, RightAvatarLocalModeRadioButton.IsChecked == true);
            RightAvatarOnlineModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(RightAvatarPathTextBox, BrowseRightAvatarButton, RightAvatarLocalModeRadioButton.IsChecked == true);
            
            // 加载插件列表
            LoadPluginList();
        }

        private void UpdateAvatarPathTextBoxState(TextBox pathTextBox, Button browseButton, bool isLocalMode)
        {
            pathTextBox.IsReadOnly = false; // 始终允许编辑，让用户可以输入本地路径或URL
            browseButton.IsEnabled = isLocalMode; // 本地模式时启用浏览按钮，否则禁用
        }

        private void LoadPluginList()
        {
            PluginListBox.Items.Clear();
            
            // 获取程序安装目录
            var appDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
            var pluginDirectory = System.IO.Path.Combine(appDirectory, "dllmod");
            
            // 如果插件目录不存在，创建它
            if (!System.IO.Directory.Exists(pluginDirectory))
            {
                System.IO.Directory.CreateDirectory(pluginDirectory);
                return;
            }
            
            // 加载所有DLL文件
            try
            {
                var dllFiles = System.IO.Directory.GetFiles(pluginDirectory, "*.dll");
                foreach (var dllFile in dllFiles)
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(dllFile);
                    PluginListBox.Items.Add(new ListBoxItem { Content = fileName, Tag = dllFile });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载插件列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetComboBoxValue(ComboBox comboBox, string value)
        {
            var item = comboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == value);
            if (item != null)
            {
                comboBox.SelectedItem = item;
            }
        }

        private void SwitchToPanel(StackPanel targetPanel)
        {
            // 隐藏所有面板
            AppearancePanel.Visibility = Visibility.Collapsed;
            BehaviorPanel.Visibility = Visibility.Collapsed;
            ContentPanel.Visibility = Visibility.Collapsed;
            AvatarPanel.Visibility = Visibility.Collapsed;
            PluginPanel.Visibility = Visibility.Collapsed;
            
            // 显示目标面板
            targetPanel.Visibility = Visibility.Visible;
        }

        private void AppearanceButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchToPanel(AppearancePanel);
        }

        private void BehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchToPanel(BehaviorPanel);
        }

        private void ContentButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchToPanel(ContentPanel);
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchToPanel(AvatarPanel);
        }

        private void PluginButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchToPanel(PluginPanel);
        }

        

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前设置值
            settings.ThemeColor = ThemeColorTextBox.Text.Trim();
            settings.ShowInTaskbar = ShowInTaskbarCheckBox.IsChecked ?? false;
            settings.StartWithWindows = StartWithWindowsCheckBox.IsChecked ?? false;
            settings.IsTopmost = IsTopmostCheckBox.IsChecked ?? false;
            
            // 获取各元素的透明度设置
            settings.BackgroundOpacity = BackgroundOpacitySlider.Value;
            settings.TextOpacity = TextOpacitySlider.Value;
            settings.LeftAvatarOpacity = LeftAvatarOpacitySlider.Value;
            settings.RightAvatarOpacity = RightAvatarOpacitySlider.Value;
            settings.CenterHeartOpacity = CenterHeartOpacitySlider.Value;
            
            // 获取圆角角度设置
            settings.CornerRadius = CornerRadiusSlider.Value;
            
            // 获取字体大小
            if (FontSizeComboBox.SelectedItem is ComboBoxItem fontItem)
            {
                settings.FontSize = fontItem.Content.ToString() ?? "中";
            }
            
            // 获取位置
            if (PositionComboBox.SelectedItem is ComboBoxItem positionItem)
            {
                settings.Position = positionItem.Content.ToString() ?? "右上角";
            }
            
            // 获取显示秒数设置
            settings.ShowSeconds = ShowSecondsCheckBox.IsChecked ?? true;
            
            // 获取自定义文本
            settings.CustomText = CustomTextTextBox.Text.Trim();
            
            // 获取头像路径
            settings.LeftAvatarPath = LeftAvatarPathTextBox.Text.Trim();
            settings.RightAvatarPath = RightAvatarPathTextBox.Text.Trim();
            
            // 获取头像名称
            settings.LeftAvatarName = LeftAvatarNameTextBox.Text.Trim();
            settings.RightAvatarName = RightAvatarNameTextBox.Text.Trim();
            
            // 获取头像模式
            settings.IsLeftAvatarLocal = LeftAvatarLocalModeRadioButton.IsChecked == true;
            settings.IsRightAvatarLocal = RightAvatarLocalModeRadioButton.IsChecked == true;
            
            // 获取起始时间设置
            if (StartDatePicker.SelectedDate.HasValue)
            {
                var date = StartDatePicker.SelectedDate.Value;
                if (TimeSpan.TryParse(StartTimeTextBox.Text, out TimeSpan time))
                {
                    settings.StartTime = date.Date + time;
                }
                else
                {
                    MessageBox.Show("时间格式错误，请使用 HH:mm:ss 格式", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("请选择起始日期", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // 应用到主窗口
            if (mainWindow != null)
            {
                settings.ApplyToMainWindow(mainWindow);
            }
            
            // 处理开机自启动
            SetStartupWithWindows(settings.StartWithWindows);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 先应用设置
            ApplyButton_Click(sender, e);
            
            // 保存到文件
            settings.Save();
            
            MessageBox.Show("设置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BrowseLeftAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择左下角头像",
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LeftAvatarPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseRightAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择右下角头像",
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RightAvatarPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void SetStartupWithWindows(bool enable)
        {
            try
            {
                var appName = "DesktopWidget";
                var executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
                
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (enable)
                    {
                        key?.SetValue(appName, executablePath);
                    }
                    else
                    {
                        key?.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置开机自启动失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddPluginButton_Click(object sender, RoutedEventArgs e)
        {
            // 打开文件对话框选择插件文件
            var openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择插件文件",
                Filter = "插件文件 (*.dll)|*.dll|所有文件 (*.*)|*.*",
                FilterIndex = 1
            };

            if (openDialog.ShowDialog() == true)
            {
                var sourcePath = openDialog.FileName;
                var pluginName = System.IO.Path.GetFileNameWithoutExtension(sourcePath);
                var fileName = System.IO.Path.GetFileName(sourcePath);
                
                // 获取程序安装目录和插件目录
                var appDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
                var pluginDirectory = System.IO.Path.Combine(appDirectory, "dllmod");
                
                // 确保插件目录存在
                if (!System.IO.Directory.Exists(pluginDirectory))
                {
                    System.IO.Directory.CreateDirectory(pluginDirectory);
                }
                
                var destPath = System.IO.Path.Combine(pluginDirectory, fileName);
                
                try
                {
                    // 检查是否已存在同名插件
                    if (System.IO.File.Exists(destPath))
                    {
                        var result = MessageBox.Show($"插件 {fileName} 已存在，是否覆盖？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                    
                    // 复制插件文件到插件目录
                    System.IO.File.Copy(sourcePath, destPath, true);
                    
                    // 重新加载插件列表
                    LoadPluginList();
                    
                    // 选中新添加的插件
                    foreach (ListBoxItem item in PluginListBox.Items)
                    {
                        if (item.Content.ToString() == pluginName)
                        {
                            PluginListBox.SelectedItem = item;
                            break;
                        }
                    }
                    
                    MessageBox.Show($"插件 {pluginName} 添加成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"添加插件失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemovePluginButton_Click(object sender, RoutedEventArgs e)
        {
            if (PluginListBox.SelectedItem is ListBoxItem selectedItem)
            {
                var pluginName = selectedItem.Content.ToString();
                var pluginPath = selectedItem.Tag as string;
                
                if (string.IsNullOrEmpty(pluginPath))
                {
                    MessageBox.Show("无法获取插件路径信息", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var result = MessageBox.Show($"确定要移除插件 {pluginName} 吗？\n\n注意：这将删除插件文件 {System.IO.Path.GetFileName(pluginPath)}", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 删除插件文件
                        if (System.IO.File.Exists(pluginPath))
                        {
                            System.IO.File.Delete(pluginPath);
                        }
                        
                        // 从列表中移除选中的插件
                        PluginListBox.Items.Remove(selectedItem);
                        
                        // 清空插件信息
                        PluginNameTextBox.Text = "未选择插件";
                        PluginVersionTextBox.Text = "未选择插件";
                        PluginDescriptionTextBox.Text = "未选择插件";
                        
                        MessageBox.Show($"插件 {pluginName} 已成功移除！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"移除插件失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要移除的插件", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ConfigurePluginButton_Click(object sender, RoutedEventArgs e)
        {
            if (PluginListBox.SelectedItem is ListBoxItem selectedItem)
            {
                var pluginName = selectedItem.Content.ToString();
                MessageBox.Show($"配置插件: {pluginName}\n\n此功能将在后续版本中实现", "插件配置", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请先选择要配置的插件", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PluginListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PluginListBox.SelectedItem is ListBoxItem selectedItem)
            {
                var pluginName = selectedItem.Content.ToString();
                var pluginPath = selectedItem.Tag as string;
                
                // 更新插件信息
                PluginNameTextBox.Text = pluginName;
                PluginVersionTextBox.Text = "1.0.0";
                PluginDescriptionTextBox.Text = pluginPath != null ? $"插件路径: {pluginPath}" : "示例插件描述";
            }
            else
            {
                // 清空插件信息
                PluginNameTextBox.Text = "未选择插件";
                PluginVersionTextBox.Text = "未选择插件";
                PluginDescriptionTextBox.Text = "未选择插件";
            }
        }
    }
}