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
    public List<Rectangle> _rectangles = new List<Rectangle>(); // �洢���о��ο�
    private List<Rectangle> _selectedRectangles = new List<Rectangle>(); // �洢ѡ�еľ��ο�
    private Dictionary<Rectangle, Point> _initialPositions = new Dictionary<Rectangle, Point>(); // �洢ѡ�о��εĳ�ʼλ��

    public CreateROIView()
    {
        InitializeComponent();
        myCanvas.PointerPressed += MyCanvas_PointerPressed;
        myCanvas.PointerReleased += MyCanvas_PointerReleased;
        myCanvas.PointerMoved += MyCanvas_PointerMoved; // ��� PointerMoved �¼�
        this.KeyDown += MainWindow_KeyDown; // ��� KeyDown �¼�
    }
    private void MyCanvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        _startPoint = e.GetPosition(myCanvas);

        // ����Ƿ�����ĳ�����ο�
        var clickedRectangle = GetRectangleAtPoint(_startPoint);

        if (clickedRectangle != null)
        {
            // �����ס Ctrl ������֧�ֶ�ѡ
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                if (_selectedRectangles.Contains(clickedRectangle))
                {
                    // ����Ѿ�ѡ�У���ȡ��ѡ��
                    _selectedRectangles.Remove(clickedRectangle);
                    clickedRectangle.Stroke = Brushes.Black; // �ָ���ɫ
                }
                else
                {
                    // ���δѡ�У�����ӵ�ѡ���б�
                    _selectedRectangles.Add(clickedRectangle);
                    clickedRectangle.Stroke = Brushes.HotPink; // ����Ϊ�ۺ�ɫ
                }
            }
            else
            {
                // ���û�а�ס Ctrl ������ֻѡ�е�ǰ����
                ClearSelectedRectangles(); // ���֮ǰ��ѡ��
                _selectedRectangles.Add(clickedRectangle);
                clickedRectangle.Stroke = Brushes.HotPink; // ����Ϊ�ۺ�ɫ
            }

            // ��¼ѡ�о��εĳ�ʼλ��
            _initialPositions.Clear();
            foreach (var rect in _selectedRectangles)
            {
                _initialPositions[rect] = new Point(Canvas.GetLeft(rect), Canvas.GetTop(rect));
            }
        }
        else
        {
            // ���û�е�����ο��򴴽��µľ��ο�
            ClearSelectedRectangles(); // ���֮ǰ��ѡ��
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

            _rectangles.Add(_currentRectangle); // ����ɵľ��ο���ӵ��б���
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

            // �������ƫ��������ÿ��ѡ�о��ε�λ��
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
            rect.Stroke = Brushes.Black; // �ָ���ɫ
        }
        _selectedRectangles.Clear();
        _initialPositions.Clear();
    }
}