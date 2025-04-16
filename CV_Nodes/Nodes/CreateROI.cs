using Avalonia.Controls.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CV_Nodes.Views;
using NodeVision.SDK.GraphModel.Connectors;
using NodeVision.SDK.GraphModel.Nodes;
using NodeVision.SDK.Misc;
using OpenCvSharp;
using SkiaSharp;

namespace CV_Nodes.Nodes;
public partial class CreateROI:NodeViewModelBase
{
    ConnectorViewModel<Mat>  input_image,output_image;
    CreateROIView view;
    ConnectorViewModel<List<Rectangle>> rectangles;
    public CreateROI()
    {
        Title = "创建RIO";
        Inputs.Add(input_image=ConnectorBuilder.CreateInput<Mat>(this, "input_image"));
        Outputs.Add(output_image = ConnectorBuilder.CreateOutPut<Mat>(this, "output_image"));
        Outputs.Add(rectangles = ConnectorBuilder.CreateOutPut<List<Rectangle>>(this, "ROI"));

        output_image.Data = new Mat();
        view = new CreateROIView();
    }
    public override void OnInit()
    {
        rectangles.Data=view._rectangles;
    }
    [ObservableProperty]
    SKImage image;
    public Action? RequestUpdate{ get; set; } 

    public override void OnExecute()
    {
        if (input_image.Data == null)
            return;
        var temp_image = input_image.Data;
        // 把每一帧图像都画上矩形框
        if (rectangles.Data != null)
        {
            foreach (Rectangle rect in rectangles.Data)
            {
                // 将 Rectangle 转换为 OpenCvSharp 的 Rect
                var opencvRect = new OpenCvSharp.Rect(
                    (int)rect.RadiusX,
                    (int)rect.RadiusY,
                    (int)rect.Width,
                    (int)rect.Height
                );

                // 在 Mat 上绘制矩形
                Cv2.Rectangle(
                    temp_image,
                    opencvRect,
                    new Scalar(0, 255, 0), // 绿色边框
                    2 // 边框宽度
                );
            }
        }
        output_image.Data = temp_image;
        image = MatExtensions.ToSKImage(temp_image);
    }

    //public override bool CanExecute() => a != null && b != null;

    public override void OnDoubleClick()
    {
        view.IsVisible = true;
    }
}

