﻿using System;
namespace Scryber.Html.Components
{
    [PDFParsableComponent("template")]
    public class HTMLTemplate : Scryber.Data.ForEach
    {
        [PDFTemplate()]
        [PDFElement()]
        public IPDFTemplate TemplateContent
        {
            get { return base.Template; }
            set { base.Template =value; }
        }

        [PDFAttribute("data-bind", BindingOnly = true)]
        public object DataBindValue
        {
            get { return base.Value; }
            set { base.Value = value; }
        }

        [PDFAttribute("data-show", BindingOnly = true)]
        public bool DataShowValue
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        [PDFAttribute("data-content")]
        public string DataContent
        {
            get; set;
        }

        /// <summary>
        /// Global Html hidden attribute used with xhtml as hidden='hidden'
        /// </summary>
        [PDFAttribute("hidden")]
        public string Hidden
        {
            get
            {
                if (this.Visible)
                    return string.Empty;
                else
                    return "hidden";
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value != "hidden")
                    this.Visible = true;
                else
                    this.Visible = false;
            }
        }

        public HTMLTemplate()
        {
        }

        protected override IPDFTemplate GetTemplateForBinding(PDFDataContext context, int index, int count)
        {
            if(null != this.DataContent)
            {
                return this.GetDataContent(this.DataContent, context);
            }
            else
                return base.GetTemplateForBinding(context, index, count);
        }
    }
}
