using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace VisualRenderersPreview
{
    public partial class XamlPage : ContentPage
    {
        public string MainText => "Teste";
        
        public XamlPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}
