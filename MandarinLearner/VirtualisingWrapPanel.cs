using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MandarinLearner
{
    public class VirtualisingWrapPanel : VirtualizingPanel, IScrollInfo
    {
        private const double ScrollLineAmount = 16.0;

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(VirtualisingWrapPanel), new PropertyMetadata(1.0, HandleItemDimensionChanged));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(VirtualisingWrapPanel), new PropertyMetadata(1.0, HandleItemDimensionChanged));

        private static readonly DependencyProperty VirtualItemIndexProperty =
            DependencyProperty.RegisterAttached("VirtualItemIndex", typeof(int), typeof(VirtualisingWrapPanel), new PropertyMetadata(-1));

        private readonly Dictionary<UIElement, Rect> childLayouts = new Dictionary<UIElement, Rect>();

        private Size extentSize;

        private bool isInMeasure;
        private ItemsControl itemsControl;
        private IRecyclingItemContainerGenerator itemsGenerator;
        private Point offset;
        private Size viewportSize;

        public VirtualisingWrapPanel()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Dispatcher.BeginInvoke(new Action(Initialize));
            }
        }

        public double ItemHeight
        {
            private get { return (double) GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            private get { return (double) GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - ScrollLineAmount);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + ScrollLineAmount);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset + ScrollLineAmount);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset - ScrollLineAmount);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset + ItemWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset - ItemWidth);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void SetHorizontalOffset(double newHorizontalOffset)
        {
            if (isInMeasure)
            {
                return;
            }

            newHorizontalOffset = Clamp(newHorizontalOffset, 0, ExtentWidth - ViewportWidth);
            offset = new Point(newHorizontalOffset, offset.Y);

            InvalidateScrollInfo();
            InvalidateMeasure();
        }

        public void SetVerticalOffset(double newVerticalOffset)
        {
            if (isInMeasure)
            {
                return;
            }

            newVerticalOffset = Clamp(newVerticalOffset, 0, ExtentHeight - ViewportHeight);
            offset = new Point(offset.X, newVerticalOffset);

            InvalidateScrollInfo();
            InvalidateMeasure();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        public bool CanVerticallyScroll { get; set; }

        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth => extentSize.Width;

        public double ExtentHeight => extentSize.Height;

        public double ViewportWidth => viewportSize.Width;

        public double ViewportHeight => viewportSize.Height;

        public double HorizontalOffset => offset.X;

        public double VerticalOffset => offset.Y;

        public ScrollViewer ScrollOwner { get; set; }

        private static int GetVirtualItemIndex(DependencyObject obj)
        {
            return (int) obj.GetValue(VirtualItemIndexProperty);
        }

        private static void SetVirtualItemIndex(DependencyObject obj, int value)
        {
            obj.SetValue(VirtualItemIndexProperty, value);
        }

        private void Initialize()
        {
            itemsControl = ItemsControl.GetItemsOwner(this);
            itemsGenerator = (IRecyclingItemContainerGenerator) ItemContainerGenerator;

            InvalidateMeasure();
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);

            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (itemsControl == null)
            {
                return new Size(double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width,
                    double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
            }

            isInMeasure = true;
            childLayouts.Clear();

            ExtentInfo extentInfo = GetExtentInfo(availableSize);

            EnsureScrollOffsetIsWithinConstrains(extentInfo);

            ItemLayoutInfo layoutInfo = GetLayoutInfo(availableSize, ItemHeight, extentInfo);

            RecycleItems(layoutInfo);

            // Determine where the first item is in relation to previously realized items
            GeneratorPosition generatorStartPosition = itemsGenerator.GeneratorPositionFromIndex(layoutInfo.FirstRealizedItemIndex);

            var visualIndex = 0;

            double currentX = layoutInfo.FirstRealizedItemLeft;
            double currentY = layoutInfo.FirstRealizedLineTop;

            using (itemsGenerator.StartAt(generatorStartPosition, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = layoutInfo.FirstRealizedItemIndex; itemIndex <= layoutInfo.LastRealizedItemIndex; itemIndex++, visualIndex++)
                {
                    bool newlyRealized;

                    var child = (UIElement) itemsGenerator.GenerateNext(out newlyRealized);
                    SetVirtualItemIndex(child, itemIndex);

                    if (newlyRealized)
                    {
                        InsertInternalChild(visualIndex, child);
                    }
                    else
                    {
                        // check if item needs to be moved into a new position in the Children collection
                        if (visualIndex < Children.Count)
                        {
                            if (!Equals(Children[visualIndex], child))
                            {
                                int childCurrentIndex = Children.IndexOf(child);

                                if (childCurrentIndex >= 0)
                                {
                                    RemoveInternalChildRange(childCurrentIndex, 1);
                                }

                                InsertInternalChild(visualIndex, child);
                            }
                        }
                        else
                        {
                            // we know that the child can't already be in the children collection
                            // because we've been inserting children in correct visualIndex order,
                            // and this child has a visualIndex greater than the Children.Count
                            AddInternalChild(child);
                        }
                    }

                    // only prepare the item once it has been added to the visual tree
                    itemsGenerator.PrepareItemContainer(child);

                    child.Measure(new Size(ItemWidth, ItemHeight));

                    childLayouts.Add(child, new Rect(currentX, currentY, ItemWidth, ItemHeight));

                    if (currentX + ItemWidth * 2 >= availableSize.Width)
                    {
                        // wrap to a new line
                        currentY += ItemHeight;
                        currentX = 0;
                    }
                    else
                    {
                        currentX += ItemWidth;
                    }
                }
            }

            RemoveRedundantChildren();
            UpdateScrollInfo(availableSize, extentInfo);

            var desiredSize = new Size(double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width,
                double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);

            isInMeasure = false;

            return desiredSize;
        }

        private void EnsureScrollOffsetIsWithinConstrains(ExtentInfo extentInfo)
        {
            offset.Y = Clamp(offset.Y, 0, extentInfo.MaxVerticalOffset);
        }

        private void RecycleItems(ItemLayoutInfo layoutInfo)
        {
            foreach (UIElement child in Children.OfType<UIElement>())
            {
                int virtualItemIndex = GetVirtualItemIndex(child);

                if (virtualItemIndex < layoutInfo.FirstRealizedItemIndex || virtualItemIndex > layoutInfo.LastRealizedItemIndex)
                {
                    GeneratorPosition generatorPosition = itemsGenerator.GeneratorPositionFromIndex(virtualItemIndex);
                    if (generatorPosition.Index >= 0)
                    {
                        itemsGenerator.Recycle(generatorPosition, 1);
                    }
                }

                SetVirtualItemIndex(child, -1);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children.OfType<UIElement>())
            {
                child.Arrange(childLayouts[child]);
            }

            return finalSize;
        }

        private void UpdateScrollInfo(Size availableSize, ExtentInfo extentInfo)
        {
            viewportSize = availableSize;
            extentSize = new Size(availableSize.Width, extentInfo.ExtentHeight);

            InvalidateScrollInfo();
        }

        private void RemoveRedundantChildren()
        {
            // iterate backwards through the child collection because we're going to be
            // removing items from it
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                UIElement child = Children[i];

                // if the virtual item index is -1, this indicates
                // it is a recycled item that hasn't been reused this time round
                if (GetVirtualItemIndex(child) == -1)
                {
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        private ItemLayoutInfo GetLayoutInfo(Size availableSize, double itemHeight, ExtentInfo extentInfo)
        {
            if (itemsControl == null)
            {
                return new ItemLayoutInfo();
            }

            // we need to ensure that there is one realized item prior to the first visible item, and one after the last visible item,
            // so that keyboard navigation works properly. For example, when focus is on the first visible item, and the user
            // navigates up, the ListBox selects the previous item, and the scrolls that into view - and this triggers the loading of the rest of the items 
            // in that row

            var firstVisibleLine = (int) Math.Floor(VerticalOffset / itemHeight);

            int firstRealizedIndex = Math.Max(extentInfo.ItemsPerLine * firstVisibleLine - 1, 0);
            double firstRealizedItemLeft = firstRealizedIndex % extentInfo.ItemsPerLine * ItemWidth - HorizontalOffset;
            double firstRealizedItemTop = firstRealizedIndex / extentInfo.ItemsPerLine * itemHeight - VerticalOffset;

            double firstCompleteLineTop = firstVisibleLine == 0 ? firstRealizedItemTop : firstRealizedItemTop + ItemHeight;
            var completeRealizedLines = (int) Math.Ceiling((availableSize.Height - firstCompleteLineTop) / itemHeight);

            int lastRealizedIndex = Math.Min(firstRealizedIndex + completeRealizedLines * extentInfo.ItemsPerLine + 2, itemsControl.Items.Count - 1);

            return new ItemLayoutInfo
            {
                FirstRealizedItemIndex = firstRealizedIndex,
                FirstRealizedItemLeft = firstRealizedItemLeft,
                FirstRealizedLineTop = firstRealizedItemTop,
                LastRealizedItemIndex = lastRealizedIndex
            };
        }

        private ExtentInfo GetExtentInfo(Size viewPortSize)
        {
            if (itemsControl == null)
            {
                return new ExtentInfo();
            }

            int itemsPerLine = Math.Max((int) Math.Floor(viewPortSize.Width / ItemWidth), 1);
            var totalLines = (int) Math.Ceiling((double) itemsControl.Items.Count / itemsPerLine);
            double extentHeight = Math.Max(totalLines * ItemHeight, viewPortSize.Height);

            return new ExtentInfo
            {
                ItemsPerLine = itemsPerLine,
                ExtentHeight = extentHeight,
                MaxVerticalOffset = extentHeight - viewPortSize.Height
            };
        }

        private void InvalidateScrollInfo()
        {
            ScrollOwner?.InvalidateScrollInfo();
        }

        private static void HandleItemDimensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wrapPanel = d as VirtualisingWrapPanel;

            wrapPanel?.InvalidateMeasure();
        }


        private double Clamp(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private class ExtentInfo
        {
            public double ExtentHeight;
            public int ItemsPerLine;
            public double MaxVerticalOffset;
        }
    }

    public class ItemLayoutInfo
    {
        public int FirstRealizedItemIndex;
        public double FirstRealizedItemLeft;
        public double FirstRealizedLineTop;
        public int LastRealizedItemIndex;
    }
}