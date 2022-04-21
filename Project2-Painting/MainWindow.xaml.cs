using IContract;
using Newtonsoft.Json;
using Project2_Painting.MyAdorner;
using Project2_Painting.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project2_Painting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        // save all shape entity
        private List<IShapeEntity> _allShape = new List<IShapeEntity>();
        // use to implement undo&redo method
        private Stack<IShapeEntity> _temp = new Stack<IShapeEntity>();

        // State
        bool _isDrawing = false;
        bool _isSelecting = false;
        string _currentType = "";
        IShapeEntity _preview = null;
        Point _start;
        List<IShapeEntity> _drawnShapes = new List<IShapeEntity>();
        StyleState _currentStyle = new StyleState();
        private Matrix _originalMatrix;

        // Config
        Dictionary<string, IPaintBusiness> _painterPrototypes = new Dictionary<string, IPaintBusiness>();
        Dictionary<string, IShapeEntity> _shapesPrototypes = new Dictionary<string, IShapeEntity>();


        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* Nạp tất cả dll, tìm kiếm entity và business */
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var folderInfo = new DirectoryInfo(exeFolder);
            var dllFiles = folderInfo.GetFiles("*.dll");

            foreach (var dll in dllFiles)
            {
                Assembly assembly = Assembly.LoadFrom(dll.FullName);

                Type[] types = assembly.GetTypes();

                // Giả định: 1 dll chỉ có 1 entity và 1 business tương ứng
                IShapeEntity? entity = null;
                IPaintBusiness? business = null;

                foreach (Type type in types)
                {
                    if (type.IsClass)
                    {
                        if (typeof(IShapeEntity).IsAssignableFrom(type))
                        {
                            entity = (Activator.CreateInstance(type) as IShapeEntity)!;
                        }

                        if (typeof(IPaintBusiness).IsAssignableFrom(type))
                        {
                            business = (Activator.CreateInstance(type) as IPaintBusiness)!;
                        }
                    }
                }

                //var img = new Bitmap
                if (entity != null)
                {
                    _shapesPrototypes.Add(entity!.Name, entity);
                    _painterPrototypes.Add(entity!.Name, business!);
                }
            }

            foreach (var item in _shapesPrototypes)
            {
                var shape = item.Value as IShapeEntity;
                _allShape.Add(shape);
            }

            // Tạo ra các nút bấm tương ứng
            //foreach (var (name, entity) in _shapesPrototypes)
            //{
            //    var button = new Button();
            //    button.Content = name;
            //    button.Tag = entity;
            //    button.Width = 80;
            //    button.Height = 35;
            //    button.Click += Button_Click;

            //    //TODO: thêm các nút bấm vào giao diện
            //    actionsStackPanel.Children.Add(button);
            //}

            iconListView.ItemsSource = _allShape;

            if (_shapesPrototypes.Count > 0)
            {
                //Lựa chọn nút bấm đầu tiên
                var (key, shape) = _shapesPrototypes.ElementAt(0);
                _currentType = key;
                _preview = (shape.Clone() as IShapeEntity)!;
            }


            var matrixTransform = content.RenderTransform as MatrixTransform;
            if (matrixTransform != null) _originalMatrix = matrixTransform.Matrix;

        }

        //// Đổi lựa chọn
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var button = sender as Button;
        //    var entity = button!.Tag as IShapeEntity;

        //    _currentType = entity!.Name;
        //    _preview = (_shapesPrototypes[entity.Name].Clone() as IShapeEntity)!;
        //}

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (_isSelecting)
            {
                if (_drawnShapes.Count > 0)
                {
                    HitTestResult hitTestResult = VisualTreeHelper.HitTest(content, e.GetPosition(content));
                    if (hitTestResult != null && hitTestResult.VisualHit is UIElement element)
                    {
                        AdornerLayer myAdornerLayer = ClearAllAdorner();
                        if (myAdornerLayer != null)
                        {
                            if (element is Line)
                            {
                                myAdornerLayer.Add(new MyLineAdorner(element));
                            }
                            else
                            {
                                myAdornerLayer.Add(new MyRectangleAdorner(element));
                            }
                        }
                    }
                }
            }
            else
            {
                _isDrawing = true;
                _start = e.GetPosition(content);

                _preview.HandleStart(_start);
            }
            
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                var end = e.GetPosition(content);
                _preview.HandleEnd(end);

                // Xóa đi tất cả bản vẽ cũ và vẽ lại những đường thẳng trước đó
                content.Children.Clear(); // Xóa đi toàn bộ

                // Vẽ lại những hình đã vẽ trước đó
                foreach (var item in _drawnShapes)
                {
                    var painter = _painterPrototypes[item.Name];
                    var shape = painter.Draw(item, item.Thickness, item.Brush, item.StrokeDash, item.Fill);

                    content.Children.Add(shape);
                }

                var previewPainter = _painterPrototypes[_preview.Name];
                var previewElement = previewPainter.Draw(_preview, _currentStyle.Thickness, 
                    _currentStyle.Brush, _currentStyle.StrokeDash, _currentStyle.fill);
                content.Children.Add(previewElement);
            }
        }
        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;

            var end = e.GetPosition(content); // Điểm kết thúc

            _preview.HandleEnd(end);
            _preview.Thickness = _currentStyle.Thickness;
            _preview.Brush = _currentStyle.Brush;
            _preview.StrokeDash = _currentStyle.StrokeDash;
            _preview.Fill = _currentStyle.fill;

            _drawnShapes.Add(_preview.Clone() as IShapeEntity);
        }

        private void shapeSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (this._allShape.Count == 0)
                return;

            var index = iconListView.SelectedIndex;

            _currentType = _allShape[index].Name;
            _preview = (_shapesPrototypes[_currentType].Clone() as IShapeEntity)!;

        }
        private void editColorBtn(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorPicker = new System.Windows.Forms.ColorDialog();

            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentStyle.Brush = new SolidColorBrush(Color.FromRgb(colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B));
            }
        }
        private AdornerLayer ClearAllAdorner()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(content);
            if (myAdornerLayer != null)
            {
                foreach (UIElement canvasChild in content.Children)
                {
                    var adorners = myAdornerLayer.GetAdorners(canvasChild);
                    if (adorners != null)
                    {
                        foreach (var adorner in adorners)
                        {
                            myAdornerLayer.Remove(adorner);
                        }
                    }
                }

                return myAdornerLayer;
            }

            return null;
        }

        private void btnBasicGray_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Gray);
        }
        private void btnBasicBlack_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Black);
        }
        private void btnBasicRed_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Red);
        }
        private void btnBasicOrange_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Orange);
        }
        private void btnBasicYellow_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Yellow);
        }
        private void btnBasicBlue_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Blue);
        }
        private void btnBasicGreen_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Green);
        }
        private void btnBasicPurple_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Purple);
        }
        private void btnBasicPink_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Pink);
        }
        private void btnBasicBrown_Click(object sender, RoutedEventArgs e)
        {
            _currentStyle.Brush = new SolidColorBrush(Colors.Brown);
        }

        private void sizeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sizeComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentStyle.Thickness = 1;
                    break;
                case 1:
                    _currentStyle.Thickness = 2;
                    break;
                case 2:
                    _currentStyle.Thickness = 3;
                    break;
                case 3:
                    _currentStyle.Thickness = 4;
                    break;
                default:
                    break;
            }
        }
        private void outlineSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = outlineCombobox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentStyle.StrokeDash = null;
                    break;
                case 1:
                    _currentStyle.StrokeDash = new DoubleCollection() { 6, 1 };
                    break;
                case 2:
                    _currentStyle.StrokeDash = new DoubleCollection() { 1, 1 };
                    break;
                case 3:
                    _currentStyle.StrokeDash = new DoubleCollection() { 4, 1, 1, 1, 1, 1 };
                    break;
                default:
                    break;
            }
        }

        private void undoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_drawnShapes.Count == 0)
            {
                undoBtn.Background = Brushes.LightGray;
                return;
            }

            undoBtn.Background = null;

            int lastIndex = _drawnShapes.Count - 1;
            _temp.Push(_drawnShapes[lastIndex]);
            _drawnShapes.RemoveAt(lastIndex);

            content.Children.Clear(); // delete all shape previous

            // re draw new shape list 
            foreach (var item in _drawnShapes)
            {
                var painter = _painterPrototypes[item.Name];
                var shape = painter.Draw(item, item.Thickness, item.Brush, item.StrokeDash, item.Fill);

                content.Children.Add(shape);
            }

        }
        private void redoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_temp.Count == 0)
            {
                redoBtn.Background = Brushes.LightGray;
                return;
            }
            redoBtn.Background = null;

            // Last item undo push in temp and then pop (LIFO mechanism)
            _drawnShapes.Add(_temp.Pop());

            content.Children.Clear(); // delete all shape previous

            // re draw new shape list 
            foreach (var item in _drawnShapes)
            {
                var painter = _painterPrototypes[item.Name];
                var shape = painter.Draw(item, item.Thickness, item.Brush, item.StrokeDash, item.Fill);

                content.Children.Add(shape);
            }
        }
        private void fillColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = new SolidColorBrush(Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                _currentStyle.fill = color;
                fillColor.Background = color;
            }
        }

        private void zoominBtn_Click(object sender, RoutedEventArgs e)
        {
            Point center = content.TransformToAncestor(gridView).Transform(new Point(content.ActualWidth / 2, content.ActualHeight / 2));

            var matTrans = content.RenderTransform as MatrixTransform;
            var mat = matTrans.Matrix;
            var scale = 1.1;
            mat.ScaleAt(scale, scale, center.X, center.Y);
            matTrans.Matrix = mat;
            e.Handled = true;
        }
        private void zoomoutBtn_Click(object sender, RoutedEventArgs e)
        {
            Point center = content.TransformToAncestor(gridView).Transform(
                new Point(content.ActualWidth / 2, content.ActualHeight / 2));

            var matTrans = content.RenderTransform as MatrixTransform;
            var mat = matTrans.Matrix;
            var scale = 1 / 1.1;
            mat.ScaleAt(scale, scale, center.X, center.Y);
            matTrans.Matrix = mat;
            e.Handled = true;
        }
        private void normalView_Click(object sender, RoutedEventArgs e)
        {
            var matTrans = content.RenderTransform as MatrixTransform;

            matTrans.Matrix = _originalMatrix;
            e.Handled = true;
        }
        private void gridView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var matTrans = content.RenderTransform as MatrixTransform;
            var pos1 = e.GetPosition(gridView);

            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;

            var mat = matTrans.Matrix;
            mat.ScaleAt(scale, scale, pos1.X, pos1.Y);
            matTrans.Matrix = mat;
            e.Handled = true;
        }
        private void selectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = selectionComboBox.SelectedIndex;
            _isSelecting = index == 0 ? false : true;
        }

        private void openFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();

            dialog.Filter = "JSON (*.json)|*.json";

            
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                string[] contents = File.ReadAllLines(path);

                string background = "";
                string json = contents[0];
                if (contents.Length > 1)
                    background = contents[1];

                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                _drawnShapes.Clear();
                content.Children.Clear();

                List<IShapeEntity> containers = JsonConvert.DeserializeObject<List<IShapeEntity>>(json, settings);

                foreach (var item in containers)
                    _drawnShapes.Add(item);

            }

            // re draw new shape list 
            foreach (var item in _drawnShapes)
            {
                var painter = _painterPrototypes[item.Name];
                var shape = painter.Draw(item, item.Thickness, item.Brush, item.StrokeDash, item.Fill);

                content.Children.Add(shape);
            }
        }
        private void saveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            var serializedShapeList = JsonConvert.SerializeObject(_drawnShapes, settings);

            // experience 
            StringBuilder builder = new StringBuilder();
            builder.Append(serializedShapeList);
            string content = builder.ToString();


            var dialog = new System.Windows.Forms.SaveFileDialog();

            dialog.Filter = "JSON (*.json)|*.json";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                File.WriteAllText(path, content);
            }
        }
        private void importFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG (*.png)|*.png";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;


                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
                content.Background = brush;
            }
        }
        private void exportFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Rect rect = new Rect(content.RenderSize);
                RenderTargetBitmap renderTargetBitmap =
                    new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, 96d, 96d, PixelFormats.Default);
                renderTargetBitmap.Render(content);

                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                MemoryStream memoryStream = new MemoryStream();

                pngEncoder.Save(memoryStream);
                memoryStream.Close();

                File.WriteAllBytes(dialog.FileName, memoryStream.ToArray());
            }


        }
    }
}
