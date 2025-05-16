using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp4;

public class FlyoutClass
{
    public string Title { get; set; } = "";
    public string IconSource { get; set; } = "";
    public Type TargetType { get; set; } = typeof(ContentPage);
}
