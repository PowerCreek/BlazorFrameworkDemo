using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorFrameworkDemo.Interop;
using CompQComponents.Lib.Attributes;
using CompQComponents.Lib.Components;
using Microsoft.AspNetCore.Components.Web;
using static CompQComponents.Lib.Components.EventItems;

namespace BlazorFrameworkDemo.Components
{
    public class SectionA : QComponent<BasicElement>
    {
        
    }
    public class MiddlePanel : QComponent<BasicElement>
    {
        
        public Slider Slider = new()
        {
            SlotCount = 5
        };

        public MiddlePanel()
        {
            NeverSetKey = false;
            TriggerRender();

            Slider.TriggerParentRender = () => TriggerRender();
            
            DoScrollAction = async args => Slider.OnScrollMethod((args as WheelEventArgs)!);
           
            EventContainer = new HashSet<EventCallbackItem>()
                .SetEvent(
                    OnTouchMove.AddEventListener(args =>
                    {
                    }),
                    
                    OnPointerDown.AddEventListener(args =>
                    {
                        if(args is PointerEventArgs buttons && (buttons.Buttons & 1) == 0) return;
                        Slider.StartPointer((args as PointerEventArgs)!);
                    }),
                    
                    OnPointerMove.AddEventListener(async args =>
                    {
                        if (args is PointerEventArgs buttons && (buttons.Buttons & 1) == 0)
                        {
                            Slider.ResetAverageDelta();
                            return;
                        }
                        if (!Slider.AverageTouchY.HasValue) return;
                        Slider.OnScrollMethod((args as PointerEventArgs)!);
                    }),
                    OnPointerUp.AddEventListener(async args =>
                    {
                        if (!Slider.AverageTouchY.HasValue) return;
                        await Slider.OnScrollMethod((args as PointerEventArgs)!, true);
                        Slider.ResetAverageDelta();
                    }),
                    
                    OnWheel.AddEventListener(args =>
                    {
                        Slider.OnScrollMethod((args as WheelEventArgs)!);
                    })
                );
        }
        
        public Action<EventArgs> DoScrollAction;
        
        public Dictionary<int, object> PositionMap = new();

        protected override IEnumerable<QComponent> Children
        {
            get
            {
                yield return Slider;
            }
            set { }
        }
    }
    
    public class SectionC : QComponent<BasicElement>
    {
        
    }
}