using BlazorParameterGeneratorAttributes;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Client.Components;

[ParametersSettable]
public partial class MyComponent
{
    private int _field;

    [Parameter]
    public string Prop1 { get; set; }

    [Parameter]
    public string Prop2 { get; set; }

    [Parameter]
    public int Prop3 { get; set; }

    [Parameter]
    public string Prop4 { get; set; }

    [Parameter]
    public string Prop5 { get; set; }

    public void Temp()
    {

    }
}