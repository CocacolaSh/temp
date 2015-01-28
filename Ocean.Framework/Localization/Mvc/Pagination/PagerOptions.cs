/*
 ASP.NET MvcPager control
 Copyright:2009-2010 Webdiyer (http://en.webdiyer.com)
 Source code released under Ms-PL license
 */
namespace Ocean.Framework.Mvc.Pagination
{
    public class PagerOptions
    {
        public PagerOptions()
        {
            AutoHide = true;
            PageIndexParameterName = "page";
            NumericPagerItemCount = 10;
            AlwaysShowFirstLastPageNumber = false;
            ShowPrevNext = true;
            PrevPageText = "上一页";
            NextPageText = "下一页";
            ShowNumericPagerItems = true;
            ShowFirstLast = true;
            FirstPageText = "首页";
            LastPageText = "末页";
            ShowMorePagerItems = true;
            MorePageText = "...";
            ShowDisabledPagerItems = true;
            SeparatorHtml = "&nbsp;&nbsp;";
            UseJqueryAjax = false;
            ContainerTagName = "div";
            ShowPageIndexBox = false;
            ShowGoButton = true;
            PageIndexBoxType = PageIndexBoxType.TextBox;
            MaximumPageIndexItems = 80;
            GoButtonText = "Go";
            ContainerTagName = "div";
            InvalidPageIndexErrorMessage = "Invalid page index";
            PageIndexOutOfRangeErrorMessage = "Page index out of range";
            ShowInfo= false;
            TotalInfoFormatString = "共%_TotalPage_%页 ";
            UseCurrentPageUrl = false;
        }
        /// <summary>
        /// whether or not hide control(render nothing) automatically when there's only one page
        /// </summary>
        public bool AutoHide { get; set; }

        /// <summary>
        /// PageIndexOutOfRangeErrorMessage
        /// </summary>
        public string PageIndexOutOfRangeErrorMessage { get; set; }

        /// <summary>
        /// InvalidPageIndexErrorMessage
        /// </summary>
        public string InvalidPageIndexErrorMessage { get; set; }
        /// <summary>
        /// page index parameter name in url
        /// </summary>
        public string PageIndexParameterName { get; set; }

        /// <summary>
        /// Whether or not show page index box
        /// </summary>
        public bool ShowPageIndexBox { get; set; }

        /// <summary>
        /// Page index box type
        /// </summary>
        public PageIndexBoxType PageIndexBoxType { get; set; }

        /// <summary>
        /// Maximum page index items listed in dropdownlist
        /// </summary>
        public int MaximumPageIndexItems { get; set; }

        /// <summary>
        /// whether or not show go button
        /// </summary>
        public bool ShowGoButton { get; set; }

        /// <summary>
        /// text displayed on go button
        /// </summary>
        public string GoButtonText { get; set; }

        /// <summary>
        /// numeric pager item format string
        /// </summary>
        public string PageNumberFormatString { get; set; }

        public string PageNumberCss { get; set; }
        private string _containerTagName;
        /// <summary>
        /// html tag name when control rendered
        /// </summary>
        public string ContainerTagName
        {
            get
            {
                return _containerTagName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new System.ArgumentException("ContainerTagName can not be null or empty", "ContainerTagName");
                _containerTagName = value;
            }
        }

        /// <summary>
        /// all pageritem wrapper format string
        /// </summary>
        public string PagerItemWrapperCss { get; set; }

        /// <summary>
        /// current pager item format string
        /// </summary>
        public string CurrentPageNumberCss { get; set; }

        /// <summary>
        /// NumericPager Item Wrapper Format String
        /// </summary>
        public string NumericPagerItemWrapperCss { get; set; }

        /// <summary>
        /// Current Pager Item Wrapper Format String
        /// </summary>
        public string CurrentPagerItemWrapperCss { get; set; }

        ///// <summary>
        ///// NavigationPager Item Wrapper Format String
        ///// </summary>
        //public string NavigationPagerItemWrapperFormatString { get; set; }

        /// <summary>
        /// 第一页按键的样式类名
        /// </summary>
        public string FirstItemWrapperCss { get; set; }

        /// <summary>
        /// 下一页按键的样式类名
        /// </summary>
        public string NextItemWrapperCss { get; set; }

        /// <summary>
        /// 前一页按键的样式类名
        /// </summary>
        public string PrevItemWrapperCss { get; set; }

        /// <summary>
        /// 最后页按键的样式类名
        /// </summary>
        public string LastItemWrapperCss { get; set; }

        public string DisabledCss { get; set; }

        /// <summary>
        /// MorePagerItem Wrapper Format String
        /// </summary>
        public string MorePagerItemWrapperCss { get; set; }

        /// <summary>
        /// PageIndexBox Wrapper Format String
        /// </summary>
        public string PageIndexBoxWrapperFormatString { get; set; }

        /// <summary>
        /// GoToPage Section Wrapper Format String
        /// </summary>
        public string GoToPageSectionWrapperFormatString { get; set; }
        /// <summary>
        /// GoToPage Section Wrapper Format String
        /// </summary>
        public string GoToPageSectionWrapperCss { get; set; }

        /// <summary>
        /// whether or not show first and last numeric page number
        /// </summary>
        public bool AlwaysShowFirstLastPageNumber { get; set; }
        /// <summary>
        /// numeric pager items count
        /// </summary>
        public int NumericPagerItemCount { get; set; }
        /// <summary>
        /// whether or not show previous and next pager items
        /// </summary>
        public bool ShowPrevNext { get; set; }
        /// <summary>
        /// previous page text
        /// </summary>
        public string PrevPageText { get; set; }
        /// <summary>
        /// next page text
        /// </summary>
        public string NextPageText { get; set; }
        /// <summary>
        /// whether or not show numeric pager items
        /// </summary>
        public bool ShowNumericPagerItems { get; set; }
        /// <summary>
        /// whether or not show first and last pager items
        /// </summary>
        public bool ShowFirstLast { get; set; }
        /// <summary>
        /// first page text
        /// </summary>
        public string FirstPageText { get; set; }
        /// <summary>
        /// last page text
        /// </summary>
        public string LastPageText { get; set; }
        /// <summary>
        /// whethor or not show more pager items
        /// </summary>
        public bool ShowMorePagerItems { get; set; }
        /// <summary>
        /// more page text
        /// </summary>
        public string MorePageText { get; set; }
        /// <summary>
        /// client id of paging control container
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// HOrizontal alignment
        /// </summary>
        public string HorizontalAlign { get; set; }
        /// <summary>
        /// CSS class to apply
        /// </summary>
        public string CssClass { get; set; }
        /// <summary>
        /// whether or not show disabled pager items
        /// </summary>
        public bool ShowDisabledPagerItems { get; set; }
        /// <summary>
        /// seperating item html
        /// </summary>
        public string SeparatorHtml { get; set; }

        public bool ShowInfo { get; set; }
        public string TotalInfoFormatString { get; set; }
        public string TotalInfoWrapperCss { get; set; }
        /// <summary>
        /// 使用当前请求的页面地址
        /// </summary>
        public bool UseCurrentPageUrl { get; set; }
        /// <summary>
        /// whether or not use jQuery ajax, as opposed to MicrosoftAjax
        /// </summary>
        internal bool UseJqueryAjax { get; set; }
    }
    public enum PageIndexBoxType {
        TextBox,//input box
        DropDownList //dropdownlist
    }

    public enum PaginationMode
    {
        /// <summary>
        /// 上一页/下一页 模式
        /// </summary>
        NextPrevious = 1,
        /// <summary>
        /// 首页 上一页/下一页 末页 模式
        /// </summary>
        NextPreviousFirstLast = 2,
        /// <summary>
        /// 上一页/下一页 + 数字 模式，例如： 上一页 1 2 3 4 5 下一页
        /// </summary>
        NumericNextPrevious = 3,
        /// <summary>
        /// 数字 模式，例如： 1 2 3 4 5...
        /// </summary>
        Numerics = 4
    }
    public enum ShowInfoGoMode
    {
        /// <summary>
        /// 不显示统计信息和跳转部分
        /// </summary>
        None = 0,
        /// <summary>
        /// 简洁方式, 例如: 1/6  GO
        /// </summary>
        Simple = 1,
        /// <summary>
        /// 完整方式,例如: 共3页 第 页 到第  页
        /// </summary>
        Default = 2,
        /// <summary>
        /// 不显示跳转部分的默认格式, 例如: 共3页
        /// </summary>
        OnlyShowInfo = 3,
        /// <summary>
        /// 不显示跳转部分的简单格式, 例如: 1/6
        /// </summary>
        OnlyInfoSimple = 4
    }
}