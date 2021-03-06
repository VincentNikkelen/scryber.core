﻿using System;
using Scryber.Components;
using Scryber.Drawing;
using Scryber.Native;
using Scryber.Styles;

namespace Scryber.Svg.Components
{
    public abstract class SVGShape : SVGBase, IPDFGraphicPathComponent
    {



        public SVGShape(PDFObjectType type)
            : base(type)
        {
        }

        private PDFGraphicsPath _path;

        protected PDFGraphicsPath Path
        {
            get { return _path; }
            set { _path = value; }
        }

        PDFGraphicsPath IPDFGraphicPathComponent.Path
        {
            get { return this.Path; }
            set { this.Path = value; }
        }


        protected abstract PDFGraphicsPath CreatePath(PDFSize available, Style fullstyle);

        PDFGraphicsPath IPDFGraphicPathComponent.CreatePath(PDFSize available, Style fullstyle)
        {
            return this.CreatePath(available, fullstyle);
        }

        public PDFObjectRef OutputToPDF(PDFRenderContext context, PDFWriter writer)
        {
            Style fullstyle = context.FullStyle;
            if (null == fullstyle)
                throw new ArgumentNullException("context.FullStyle");

            PDFGraphics graphics = context.Graphics;
            if (null == graphics)
                throw new ArgumentNullException("context.Graphics");

            if (null != this.Path)
            {
                PDFBrush brush = fullstyle.CreateFillBrush();
                PDFPen pen = fullstyle.CreateStrokePen();

                if (null != pen && null != brush)
                    graphics.FillAndStrokePath(brush, pen, context.Offset, this.Path);

                else if (null != brush)
                    graphics.FillPath(brush, context.Offset, this.Path);

                else if (null != pen)
                    graphics.DrawPath(pen, context.Offset, this.Path);


            }
            return null;
        }

        protected override void OnPreLayout(PDFLayoutContext context)
        {
            this.UpdateShapeBounds(context);
            base.OnPreLayout(context);
        }

        protected virtual void UpdateShapeBounds(PDFContextBase context)
        {
            var bounds = this.GetBounds();
            if (bounds != PDFRect.Empty)
            {
                this.X = bounds.X;
                this.Y = bounds.Y;
                this.Width = bounds.Width;
                this.Height = bounds.Height;
            }
        }



        protected virtual PDFRect GetBounds()
        {
            return PDFRect.Empty;
        }
    }
}
