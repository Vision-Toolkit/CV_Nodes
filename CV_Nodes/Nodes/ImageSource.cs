using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using NodeVision.SDK.GraphModel.Connectors;
using NodeVision.SDK.GraphModel.Nodes;
using NodeVision.SDK.Misc;
using OpenCvSharp;
using SkiaSharp;

namespace CV_Nodes.Nodes;
public partial class ImageSource:NodeViewModelBase,IDrawingLayer
{
    ConnectorViewModel<Mat>  c;
    VideoCapture ?cap;
    public ImageSource()
    {
        Title = "图像源";
        
        Outputs.Add(c = ConnectorBuilder.CreateOutPut<Mat>(this, "c"));

        c.Data = new Mat();
    }
    public override void OnInit()
    {
         cap= new VideoCapture(0);
    }
    [ObservableProperty]
    SKImage image;
    public Action? RequestUpdate{ get; set; } 

    public override void OnExecute()
    {
        cap.Read(c.Data);
        image=MatExtensions.ToSKImage(c.Data);
    }
    //public override bool CanExecute() => a != null && b != null;
}

