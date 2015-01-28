using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using Ocean.Core.Utility;

namespace Ocean.Communication.ContentProviders.Core
{
    public abstract class CollapsibleContentProvider : IContentProvider
    {
        public virtual Task<ContentProviderResult> GetContent(ContentProviderHttpRequest request)
        {
            return GetCollapsibleContent(request).Then(result =>
            {
                if (IsCollapsible && result != null)
                {
                    result.Content = String.Format(CultureInfo.InvariantCulture,
                                                      ContentFormat,
                                                      IsPopOut ? @"<div class='collapsible_pin'></div>" : "",
                                                      result.Title,
                                                      result.Content);
                }

                return result;
            });
        }

        protected virtual Regex ParameterExtractionRegex
        {
            get
            {
                return new Regex(@"(\d+)");
            }
        }

        protected virtual IList<string> ExtractParameters(Uri responseUri)
        {
            return ParameterExtractionRegex.Match(responseUri.AbsoluteUri)
                                .Groups
                                .Cast<Group>()
                                .Skip(1)
                                .Select(g => g.Value)
                                .Where(v => !String.IsNullOrEmpty(v)).ToList();
        }

        protected abstract Task<ContentProviderResult> GetCollapsibleContent(ContentProviderHttpRequest request);

        public virtual bool IsValidContent(Uri uri)
        {
            return false;
        }

        protected virtual bool IsCollapsible { get { return true; } }

        protected virtual bool IsPopOut { get { return false; } }

        private const string ContentFormat = @"<div class='collapsible_content'>{0}<h3 class='collapsible_title'>{1} (点击显示/隐藏)</h3><div class='collapsible_box'>{2}</div></div>";
    }
}