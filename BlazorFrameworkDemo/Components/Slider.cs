﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFrameworkDemo.Interop;
using CompQComponents.Lib.Attributes;
using CompQComponents.Lib.Components;
using Microsoft.AspNetCore.Components.Web;
using static CompQComponents.Lib.Components.EventItems;

namespace BlazorFrameworkDemo.Components
{
    public class Slider : QComponent<BasicElement>
    {
        public List<QComponent> Items;
        public int SlotSize = 500;
        public int SlotCount = 5;
        public int? ScrollTrack = 0;
        public float Magnitude = 1;
        public int? RolloverCheck = 0;
        public int itemsPast = 0;
        public StyleAttribute ScrollAttributeData;

        public Action TriggerParentRender = () => { };
        
        public string GetGridAreaString() => $"{string.Join("",Items.Select(e => $"\"{e.Id}\""))}";
        
        public Slider()
        {
            NeverSetKey = false;
            IsUnique = true;
            Items = Enumerable.Range(0, SlotCount)
                .Select(e =>
                {
                    var comp = new BlankElement
                    {
                        IsUnique = true, Content = $"{e} simple",
                        EventContainer = new HashSet<EventCallbackItem>().SetEvent(
                            OnMouseDown.AddEventListener(args =>
                            {
                                Console.WriteLine($"im clicking babyyyyyyy! {e}");
                            })
                        ),
                    } as QComponent;

                    comp.GetAttribute<StyleAttribute>().WithStyle(("grid-area", comp.Id));
                    
                    return comp;
                }).ToList();

            GetAttribute<CssAttribute>().WithClass("slider");
            
            ScrollAttributeData = GetAttribute<StyleAttribute>().WithStyle(
                ("position", "absolute"),
                ("grid-template-areas", GetGridAreaString()));
        }

        public Stack<QComponent> ItemBackHistory = new();
        public Stack<QComponent> ItemForwardHistory = new();

        public async Task PerformSlide(double dirY)
        {
            if (itemsPast <= 0 && ScrollTrack >= 0 && dirY <= 0)
            {
                // Console.WriteLine($"past: {itemsPast} {ScrollTrack}");
                ScrollTrack = 0;
                ScrollAttributeData.WithStyle(
                    ("top", $"0px"),
                    ("--slider_slot_display", "unset"),
                    ("--slider_slot_height", $"{SlotSize}px")
                );
                
                await (Wrapper as BasicElement)!.JsRuntime!.SetStyle(this, false, ScrollAttributeData);
                return;
            }

            var diff = ScrollTrack - (int) dirY;    
            
            RolloverCheck = (ScrollTrack%SlotSize)??0;        
            ScrollTrack -= (int) dirY;

            // ScrollTrack %= SlotSize;

            bool rolledOver = dirY switch
            {
                {} val when val < 0 => ScrollTrack%SlotSize < RolloverCheck,
                {} val when val > 0 => ScrollTrack%SlotSize > RolloverCheck,
                _ => false
            };
            
            RolloverCheck = ScrollTrack%SlotSize;
            
            int? position = ScrollTrack;
            
            if (rolledOver)
            {
                //Console.WriteLine($"past: {itemsPast} {ScrollTrack}");
                int lastItemPast = itemsPast;
                if(itemsPast >= 0) itemsPast += Math.Sign(dirY);
                if (itemsPast < 0) itemsPast = 0;

                var item = new BlankElement
                {
                    IsUnique = true,
                    Content = $"{new Random().NextInt64()}",
                };
                
                item.GetAttribute<StyleAttribute>().WithStyle(("grid-area", item.Id));

                var a = Items.First();
                var b = Items.Last();

                if (dirY is < 0)
                {
                    if (itemsPast is > 4)
                    {
                        Items.Remove(b);
                        ItemForwardHistory.Push(b);
                        //Console.WriteLine("take");
                        position = ScrollTrack -= SlotSize;
                    }
                    else if (itemsPast == 4)
                    {
                        //Console.WriteLine("takeA");
                        position = ScrollTrack -= SlotSize;
                    }

                    if (ItemBackHistory.TryPop(out var prevItem))
                    {
                        Items.Insert(0, prevItem);
                    }
                }
                else if (dirY is > 0)
                {
                    if (itemsPast is > 4)
                    {
                        ItemBackHistory.Push(a);
                        Items.Remove(a);
                        //Console.WriteLine("give");
                        position = ScrollTrack += SlotSize;
                    }

                    Items.Add(ItemForwardHistory.TryPop(out var itemForward) ? itemForward : item);
                }

                TriggerRender();
                TriggerParentRender();
            }
            
            if (!rolledOver && itemsPast == 0 && dirY < 0 && (ScrollTrack >= 0))
                position = ScrollTrack = 0;
            
            ScrollAttributeData.WithStyle(
                ("top", $"{position}px"),
                ("--slider_slot_display", "unset"),
                ("--slider_slot_height", $"{SlotSize}px"),
                ("grid-template-areas", GetGridAreaString())
            );

            await (Wrapper as BasicElement)!.JsRuntime!.SetStyle(this, false, ScrollAttributeData);
        }

        public object LOCK_OBJECT = new object();

        public const int Intervals = 10;
        public const double TouchDelta = 1;
        public ConcurrentQueue<double> DirectionQueue = new();

        public double GetDeltaY<T>(T obj)
        {
            return obj switch
            {
                {} a when a is PointerEventArgs pointer => GetPointerDeltaY(pointer)*TouchDelta,
                {} a when a is WheelEventArgs mouse => mouse.DeltaY,
            };
        }

        public double? AverageTouchY = null;
        public object average_lock = new object();
        
        public double GetPointerDeltaY(PointerEventArgs args)
        {
            lock (average_lock)
            {
                var last = AverageTouchY.Value;
                AverageTouchY = args.ScreenY;
                var val = last - AverageTouchY.Value;
                return val;
            }
        }

        public void StartPointer(PointerEventArgs args)
        {
            lock (average_lock)
            {
                DirectionQueue.Clear();
                AverageTouchY = args.ScreenY;
            }
        }
        
        public void ResetAverageDelta()
        {
            lock (average_lock)
            {
                DirectionQueue.Clear();
                AverageTouchY = null;
            }
        }

        public async Task OnScrollMethod<T>(T args, bool doThing = false)
        {
            double absDelta = 0;
            double delta = 0;
            lock (average_lock)
            {
                delta = GetDeltaY(args);
                
                if (args is WheelEventArgs)
                {
                    DirectionQueue.Enqueue(delta);
                }

                if (!doThing && args is PointerEventArgs)
                {
                    DirectionQueue.Enqueue(delta);
                }
            }

            while ((args is WheelEventArgs || DirectionQueue.Count > 1) && DirectionQueue.TryDequeue(out double item))
            {
                await PerformSlide(item);
                await Task.Delay(10);
            }

            if (doThing && DirectionQueue.Count == 1 && DirectionQueue.TryDequeue(out double item2))
            {
                delta = item2 * 3;
                double val = 0;
                if (Math.Abs(item2) <= 3)
                {                
                    //await PerformSlide(delta);
                    return;
                }

                lock (DirectionQueue)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        val = Math.Clamp(delta / ((i * 1.1) + 2), -SlotCount * 0.9, SlotSize * 0.9);
                        delta -= val;
                        DirectionQueue.Enqueue(delta);
                    }

                }

                while (!DirectionQueue.IsEmpty && DirectionQueue.TryDequeue(out var item3))
                {
                    await PerformSlide(item3);
                    await Task.Delay(5);
                }

                await PerformSlide(delta);
            }
        }

        protected override IEnumerable<QComponent> Children {
            get
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    yield return Items[i];
                }
            }
            set { }
        }
    }
}