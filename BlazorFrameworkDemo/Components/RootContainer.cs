using System.Collections.Generic;
using CompQComponents.Lib.Attributes;
using CompQComponents.Lib.Components;

namespace BlazorFrameworkDemo.Components
{
    public class RootContainer : QComponent<BasicElement>
    {

        public QComponent SectionA;
        public QComponent MiddlePanelSection;
        public QComponent SectionC;

        public RootContainer()
        {

            this.Attributes = new AttributeBuilder()
                .Set<CssAttribute>(attr => attr.WithClass("root-container")).Values;
            
            SectionA = new BlankElement
            {
                Attributes = new AttributeBuilder()
                    .Set<CssAttribute>(attr=>attr.WithClass("left-panel"))
                    .Values
            };
            
            MiddlePanelSection = new MiddlePanel { };
            MiddlePanelSection.GetAttribute<CssAttribute>().WithClass("middle-panel");
            
            SectionC = new BlankElement
            {
                Attributes = new AttributeBuilder()
                    .Set<CssAttribute>(attr=>attr.WithClass("right-panel"))
                    .Values
            };
        }
        
        protected override IEnumerable<QComponent> Children 
        {
            get
            {
                yield return SectionA;
                yield return MiddlePanelSection;
                yield return SectionC;
            }
            set
            {
                
            }
        }
    }
}