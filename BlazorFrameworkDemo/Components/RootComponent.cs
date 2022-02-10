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

        private int R = 0;
        private int G = 0;
        private int B = 0;
        public QComponent SimpleElement;
        public QComponent SimpleElement2;
        
        public RootComponent()
        {
            IsUnique = true;

            SimpleElement = new BlankElement
            {
                IsUnique = true,
                Content = "hello world",
                Attributes = new AttributeBuilder()
                    .Set<CssAttribute>(attr=>attr.WithClass("inner-panel")).Values,
                
                EventContainer = new HashSet<EventCallbackItem>().SetEvent(
                    OnMouseDown.AddEventListener(
                        args =>
                        {
                            Console.WriteLine("clicked1");
                            
                            R++;
                            SimpleElement!.GetAttribute<StyleAttribute>().WithStyle(
                                ("background-color", $"rgb({R%=255},{G},{B})"));

                            SimpleElement.TriggerRender();
                        }
                        )
                    )
            };
            
            SimpleElement2 = new BlankElement
            {
                IsUnique = true,
                Content = "hello world",
                Attributes = new AttributeBuilder()
                    .Set<CssAttribute>(attr=>attr.WithClass("inner-panel"))
                    .Set<StyleAttribute>(attr=>attr.WithStyle(("left","300px"))).Values,
                
                EventContainer = new HashSet<EventCallbackItem>().SetEvent(
                    OnMouseDown.AddEventListener(
                        args =>
                        {
                            Console.WriteLine("clicked2");
                            
                            R++;
                            SimpleElement2!.GetAttribute<StyleAttribute>().WithStyle(
                                ("background-color", $"rgb({G},{G},{R%=255})"));

                            SimpleElement2.TriggerRender();
                        }
                    )
                )
            };
        }

        protected override IEnumerable<QComponent> Children 
        {
            get
            {
                yield return SimpleElement;
                yield return SimpleElement2;
            }
            set
            {
                
            }
        }
    }
}