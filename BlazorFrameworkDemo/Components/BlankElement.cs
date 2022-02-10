using CompQComponents.Lib.Components;

namespace BlazorFrameworkDemo.Components
{
    public class BlankElement : QComponent<BasicElement>
    {

        public BlankElement()
        {
            IsUnique = false;
        }
    }
}