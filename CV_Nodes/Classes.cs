using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV_Nodes.Nodes;
using Microsoft.Extensions.DependencyInjection;
using NodeVision.SDK;
using NodeVision.SDK.GraphModel.Indexing;

namespace CV_Nodes
{

    public class Classes : IHostedExtension
    {
        public string PluginName => "CV_Nodes";

        public Version Version => Version.Parse("0.0.1");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<INodeIndexProvider, NodeIndex>();
            services.AddTransient<ImageSource>();

        }
    }

    public class NodeIndex : INodeIndexProvider
    {
        public VirtualFolder Build(VirtualFolder Root)
        {
            Root.CreateFolder("机器视觉")
                .CreateFile<ImageSource>("图像源");

            return Root;
        }
    }

}
