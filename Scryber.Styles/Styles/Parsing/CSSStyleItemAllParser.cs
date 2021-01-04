﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scryber.Styles;
using Scryber.Html;

namespace Scryber.Styles.Parsing
{
    /// <summary>
    /// Implements the Style Item Parser, delegating to one of it's known inner item parsers.
    /// </summary>
    public class CSSStyleItemAllParser : IParserStyleFactory
    {
        private IDictionary<string, IParserStyleFactory> _knownStyles;

        public CSSStyleItemAllParser()
            : this(_allknown)
        {
        }

        public CSSStyleItemAllParser(IDictionary<string, IParserStyleFactory> known)
        {
            if (null == known)
                throw new ArgumentNullException("known");

            _knownStyles = known;
        }


        public bool SetStyleValue(IHtmlContentParser parser, IPDFStyledComponent component, CSSStyleItemReader reader)
        {
            IParserStyleFactory found;

            if (string.IsNullOrEmpty(reader.CurrentAttribute) && reader.ReadNextAttributeName() == false)
                return false;

            if (_knownStyles.TryGetValue(reader.CurrentAttribute, out found))
                return found.SetStyleValue(parser, component, reader);
            else if (null != parser && parser.IsLogging)
                parser.Log("Could not set the style value on attribute '" + reader.CurrentAttribute + "' as it is not a known style attribute.");

            return false;
        }

        public bool SetStyleValue( Style style, CSSStyleItemReader reader)
        {
            IParserStyleFactory found;

            if (string.IsNullOrEmpty(reader.CurrentAttribute) && reader.ReadNextAttributeName() == false)
                return false;

            if (_knownStyles.TryGetValue(reader.CurrentAttribute, out found))
                return found.SetStyleValue(style, reader);
            else
                return false;
        }


        private static ReadOnlyDictionary<string, IParserStyleFactory> _allknown;

        static CSSStyleItemAllParser()
        {
            Dictionary<string, IParserStyleFactory> all = new Dictionary<string, IParserStyleFactory>(StringComparer.OrdinalIgnoreCase);

            all.Add(CSSStyleItems.Border, new CSSBorderParser());
            all.Add(CSSStyleItems.BorderStyle, new CSSBorderStyleParser());
            all.Add(CSSStyleItems.BorderColor, new CSSBorderColorParser());
            all.Add(CSSStyleItems.BorderWidth, new CSSBorderWidthParser());

            all.Add(CSSStyleItems.FillColor, new CSSFillColourParser());

            all.Add(CSSStyleItems.Background, new CSSBackgroundParser());
            all.Add(CSSStyleItems.BackgroundColor, new CSSBackgroundColorParser());
            all.Add(CSSStyleItems.BackgroundImage, new CSSBackgroundImageParser());
            all.Add(CSSStyleItems.BackgroundRepeat, new CSSBackgroundRepeatParser());
            all.Add(CSSStyleItems.BackgroundPosition, new CSSBackgroundPositionParser());
            all.Add(CSSStyleItems.BackgroundSize, new CSSBackgroundSizeParser());

            all.Add(CSSStyleItems.FontStyle, new CSSFontStyleParser());
            all.Add(CSSStyleItems.FontWeight, new CSSFontWeightParser());
            all.Add(CSSStyleItems.FontSize, new CSSFontSizeParser());
            all.Add(CSSStyleItems.FontLineHeight, new CSSFontLineHeightParser());
            all.Add(CSSStyleItems.FontFamily, new CSSFontFamilyParser());
            all.Add(CSSStyleItems.Font, new CSSFontParser());

            all.Add(CSSStyleItems.MarginsLeft, new CSSMarginsLeftParser());
            all.Add(CSSStyleItems.MarginsRight, new CSSMarginsRightParser());
            all.Add(CSSStyleItems.MarginsBottom, new CSSMarginsBottomParser());
            all.Add(CSSStyleItems.MarginsTop, new CSSMarginsTopParser());
            all.Add(CSSStyleItems.Margins, new CSSMarginsParser());

            all.Add(CSSStyleItems.PaddingLeft, new CSSPaddingLeftParser());
            all.Add(CSSStyleItems.PaddingRight, new CSSPaddingRightParser());
            all.Add(CSSStyleItems.PaddingBottom, new CSSPaddingBottomParser());
            all.Add(CSSStyleItems.PaddingTop, new CSSPaddingTopParser());
            all.Add(CSSStyleItems.Padding, new CSSPaddingParser());

            all.Add(CSSStyleItems.Opacity, new CSSOpacityParser());
            all.Add(CSSStyleItems.ColumnCount, new CSSColumnCountParser());
            all.Add(CSSStyleItems.ColumnGap, new CSSColumnGapParser());

            all.Add(CSSStyleItems.ColumnSpan, new CSSColumnSpanParser());

            all.Add(CSSStyleItems.Left, new CSSLeftParser());
            all.Add(CSSStyleItems.Top, new CSSTopParser());

            all.Add(CSSStyleItems.Width, new CSSWidthParser());
            all.Add(CSSStyleItems.Height, new CSSHeightParser());
            all.Add(CSSStyleItems.MinimumHeight, new CSSMinHeightParser());
            all.Add(CSSStyleItems.MinimumWidth, new CSSMinWidthParser());
            all.Add(CSSStyleItems.MaximumHeight, new CSSMaxHeightParser());
            all.Add(CSSStyleItems.MaximumWidth, new CSSMaxWidthParser());

            all.Add(CSSStyleItems.TextAlign, new CSSTextAlignParser());
            all.Add(CSSStyleItems.VerticalAlign, new CSSVerticalAlignParser());

            all.Add(CSSStyleItems.TextDecoration, new CSSTextDecorationParser());
            all.Add(CSSStyleItems.LetterSpacing, new CSSLetterSpacingParser());
            all.Add(CSSStyleItems.WordSpacing, new CSSWordSpacingParser());

            all.Add(CSSStyleItems.WhiteSpace, new CSSWhiteSpaceParser());

            all.Add(CSSStyleItems.Display, new CSSDisplayParser());
            all.Add(CSSStyleItems.Overflow, new CSSOverflowActionParser());

            all.Add(CSSStyleItems.ListStyleType, new CSSListStyleTypeParser());
            all.Add(CSSStyleItems.ListStyle, new CSSListStyleParser());

            all.Add(CSSStyleItems.PageBreakInside, new CSSPageBreakInsideParser());
            all.Add(CSSStyleItems.PageBreakAfter, new CSSPageBreakAfterParser());
            all.Add(CSSStyleItems.PageBreakBefore, new CSSPageBreakBeforeParser());

            _allknown = new ReadOnlyDictionary<string, IParserStyleFactory>(all);
        }
    }
}
