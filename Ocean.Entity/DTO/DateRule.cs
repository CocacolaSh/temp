using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class DateRule
    {
        /// <summary>
        /// 季度
        /// </summary>
        public int Quarterly { get; set; }
        /// <summary>
        /// 季度的月份
        /// </summary>
        public IList<int> Months { get; set; }
        /// <summary>
        /// 当前季度申请月份
        /// </summary>
        public int ApplyMonth { get; set; }

        /// <summary>
        /// 导入开始日
        /// </summary>
        public const int ImportStartDay = 1;
        /// <summary>
        /// 导入结束日
        /// </summary>
        public const int ImportEndDay = 4;

        /// <summary>
        /// 申请日
        /// </summary>
        public const int ApplyStartDay = 5;
        /// <summary>
        /// 申请调额临界值
        /// </summary>
        public const int ApplyMiddleDay = 15;
        /// <summary>
        /// 申请调额结束日
        /// </summary>
        public const int ApplyEndDay = 15;
        /// <summary>
        /// 调额日：首月或次月22号
        /// </summary>
        public const int ChangeDay = 22;

        /// <summary>
        /// 开始年限
        /// </summary>
        public const int StartYear = 2013;
        /// <summary>
        /// 开始季度
        /// </summary>
        public const int StartQuarter = 4;
    }
    public class DateRuleList
    {
        public static IList<DateRule> DateRules = new List<DateRule>();
        static DateRuleList()
        {
            DateRule dateRule1 = new DateRule();
            dateRule1.Quarterly = 1;
            IList<int> months1=new List<int>();
            months1.Add(1);
            months1.Add(2);
            months1.Add(3);
            dateRule1.Months = months1;
            dateRule1.ApplyMonth = 1;//测试
            DateRules.Add(dateRule1);

            dateRule1 = new DateRule();
            dateRule1.Quarterly = 2;
            months1 = new List<int>();
            months1.Add(4);
            months1.Add(5);
            months1.Add(6);
            dateRule1.Months = months1;
            dateRule1.ApplyMonth = 4;
            DateRules.Add(dateRule1);

            dateRule1 = new DateRule();
            dateRule1.Quarterly = 3;
            months1 = new List<int>();
            months1.Add(7);
            months1.Add(8);
            months1.Add(9);
            dateRule1.Months = months1;
            dateRule1.ApplyMonth = 7;
            DateRules.Add(dateRule1);

            dateRule1 = new DateRule();
            dateRule1.Quarterly = 1;
            months1 = new List<int>();
            months1.Add(10);
            months1.Add(11);
            months1.Add(12);
            dateRule1.Months = months1;
            dateRule1.ApplyMonth = 10;
            DateRules.Add(dateRule1);
        }
    }
}
