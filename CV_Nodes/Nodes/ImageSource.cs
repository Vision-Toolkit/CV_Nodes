using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using NodeVision.SDK.GraphModel.Connectors;
using NodeVision.SDK.GraphModel.Nodes;
using NodeVision.SDK.Misc;
using OpenCvSharp;

namespace CV_Nodes.Nodes;
public partial class ImageSource:NodeViewModelBase,IImagePreviewNode
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
    IImage image;
    public override void OnExecute()
    {
        cap.Read(c.Data);
        image=c.Data.ToIImage();
    }
    //public override bool CanExecute() => a != null && b != null;
}

