using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

namespace CV_Nodes.Views;

public partial class CreateROIView : UserControl
{
    private Point _startPoint;
    private Rectangle _currentRectangle;
    public List<Rectangle> _rectangles = new List<Rectangle>(); // 存储所有矩形框
    private List<Rectangle> _selectedRectangles = new List<Rectangle>(); // 存储选中的矩形框
    private Dictionary<Rectangle, Point> _initialPositions = new Dictionary<Rectangle, Point>(); // 存储选中矩形的初始位置

    public CreateROIView()
    {
        InitializeComponent();
        myCanvas.PointerPressed += MyCanvas_PointerPressed;
        myCanvas.PointerReleased += MyCanvas_PointerReleased;
        myCanvas.PointerMoved += MyCanvas_PointerMoved; // 添加 PointerMoved 事件
        this.KeyDown += MainWindow_KeyDown; // 添加 KeyDown 事件
    }
    private void MyCanvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        _startPoint = e.GetPosition(myCanvas);

        // 检查是否点击了某个矩形框
        var clickedRectangle = GetRectangleAtPoint(_startPoint);

        if (clickedRectangle != null)
        {
            // 如果按住 Ctrl 键，则支持多选
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                if (_selectedRectangles.Contains(clickedRectangle))
                {
                    // 如果已经选中，则取消选中
                    _selectedRectangles.Remove(clickedRectangle);
                    clickedRectangle.Stroke = Brushes.Black; // 恢复颜色
                }
                else
                {
                    // 如果未选中，则添加到选中列表
                    _selectedRectangles.Add(clickedRectangle);
                    clickedRectangle.Stroke = Brushes.HotPink; // 设置为粉红色
                }
            }
            else
            {
                // 如果没有按住 Ctrl 键，则只选中当前矩形
                ClearSelectedRectangles(); // 清空之前的选中
                _selectedRectangles.Add(clickedRectangle);
                clickedRectangle.Stroke = Brushes.HotPink; // 设置为粉红色
            }

            // 记录选中矩形的初始位置
            _initialPositions.Clear();
            foreach (var rect in _selectedRectangles)
            {
                _initialPositions[rect] = new Point(Canvas.GetLeft(rect), Canvas.GetTop(rect));
            }
        }
        else
        {
            // 如果没有点击矩形框，则创建新的矩形框
            ClearSelectedRectangles(); // 清空之前的选中
            _currentRectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(_currentRectangle, _startPoint.X);
            Canvas.SetTop(_currentRectangle, _startPoint.Y);
            myCanvas.Children.Add(_currentRectangle);
        }
    }

    private void MyCanvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (_currentRectangle != null)
        {
            var endPoint = e.GetPosition(myCanvas);
            double width = endPoint.X - _startPoint.X;
            double height = endPoint.Y - _startPoint.Y;
            _currentRectangle.Width = Math.Abs(width);
            _currentRectangle.Height = Math.Abs(height);
            Canvas.SetLeft(_currentRectangle, width < 0 ? endPoint.X : _startPoint.X);
            Canvas.SetTop(_currentRectangle, height < 0 ? endPoint.Y : _startPoint.Y);

            _rectangles.Add(_currentRectangle); // 将完成的矩形框添加到列表中
            _currentRectangle = null;
        }
    }

    private void MyCanvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (_currentRectangle != null && e.Pointer.Captured == image)
        {
            var currentPoint = e.GetPosition(myCanvas);
            double width = currentPoint.X - _startPoint.X;
            double height = currentPoint.Y - _startPoint.Y;
            _currentRectangle.Width = Math.Abs(width);
            _currentRectangle.Height = Math.Abs(height);
            Canvas.SetLeft(_currentRectangle, width < 0 ? currentPoint.X : _startPoint.X);
            Canvas.SetTop(_currentRectangle, height < 0 ? currentPoint.Y : _startPoint.Y);
        }
        else if (_selectedRectangles.Count > 0 && e.Pointer.Captured == image)
        {
            var currentPoint = e.GetPosition(myCanvas);
            double offsetX = currentPoint.X - _startPoint.X;
            double offsetY = currentPoint.Y - _startPoint.Y;

            // 根据鼠标偏移量更新每个选中矩形的位置
            foreach (var rect in _selectedRectangles)
            {
                if (_initialPositions.TryGetValue(rect, out var initialPosition))
                {
                    Canvas.SetLeft(rect, initialPosition.X + offsetX);
                    Canvas.SetTop(rect, initialPosition.Y + offsetY);
                }
            }
        }
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            foreach (var rect in _selectedRectangles)
            {
                myCanvas.Children.Remove(rect);
                _rectangles.Remove(rect);
            }
            _selectedRectangles.Clear();
            _initialPositions.Clear();
        }
    }

    private bool IsPointInRectangle(Point point, Rectangle rect)
    {
        double left = Canvas.GetLeft(rect);
        double top = Canvas.GetTop(rect);
        return point.X >= left && point.X <= left + rect.Width &&
               point.Y >= top && point.Y <= top + rect.Height;
    }

    private Rectangle GetRectangleAtPoint(Point point)
    {
        foreach (var rect in _rectangles)
        {
            if (IsPointInRectangle(point, rect))
            {
                return rect;
            }
        }
        return null;
    }

    private void ClearSelectedRectangles()
    {
        foreach (var rect in _selectedRectangles)
        {
            rect.Stroke = Brushes.Black; // 恢复颜色
        }
        _selectedRectangles.Clear();
        _initialPositions.Clear();
    }
}