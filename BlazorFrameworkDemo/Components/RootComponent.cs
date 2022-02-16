using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompQComponents.Lib.Attributes;
using CompQComponents.Lib.Components;
using static CompQComponents.Lib.Components.EventItems;

namespace BlazorFrameworkDemo.Components
{
    public class RootComponent : QComponent<BasicElement>
    {
        private RootContainer Container;
        
        public RootComponent()
        {
            Container = new RootContainer();
        }

        protected override IEnumerable<QComponent> Children 
        {
            get
            {
                yield return Container;
            }
            set
            {
                
            }
        }
    }
}