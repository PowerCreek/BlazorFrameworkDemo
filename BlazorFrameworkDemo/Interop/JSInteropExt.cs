using System.Linq;
using System.Threading.Tasks;
using CompQComponents.Lib.Attributes;
using CompQComponents.Lib.Components;
using Microsoft.JSInterop;

namespace BlazorFrameworkDemo.Interop
{
    public static class JSInteropExt
    {
        private const string MODIFY_STYLE = "modifyStyleOf"; 
        private const string MODIFY_ATTR = "modifyAttributeOf"; 
        
        public static async Task SetStyle(this IJSRuntime runtime, QComponent component, bool removing, StyleAttribute? styles) => await runtime.InvokeVoidAsync(MODIFY_STYLE, component.Id, removing, styles?.StyleMap?.Select(e=>new []{e.Key,e.Value}));
        public static async Task SetClass(this IJSRuntime runtime, QComponent component, CssAttribute? classes) => await runtime.InvokeVoidAsync(MODIFY_ATTR, component.Id, classes!.AttributeName, classes.AttributeContent);
    }
}