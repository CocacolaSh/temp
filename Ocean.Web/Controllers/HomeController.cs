using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Services;
using Ocean.Entity;
using Autofac;
using Ocean.Core.Infrastructure.DependencyManagement;
using Ocean.Core;
using Ocean.Entity.Enums;
using Ocean.Core.Plugins.Upload;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.Util;
using Ocean.Entity.Enums.Loan;
using Ocean.Page;
using Ocean.Core.Utility;

namespace Ocean.Web.Controllers
{
    public class HomeController : WebBaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMpUserService _mpUserService;
        private readonly IFunongbaoService _funongbaoService;
        public HomeController(IProductService productService, ICategoryService categoryService, IMpUserService mpUserService, IFunongbaoService funongbaoService)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._mpUserService = mpUserService;
            _funongbaoService = funongbaoService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            ////MpUser mpUser = _mpUserService.GetByOpenId("openId");
            ////UserLogin.Instance.CacheUser(mpUser);
            ////UserLogin.Instance.CreateCookie(mpUser.Id, mpUser.OpenID, mpUser.PasswordKey, 0);

            //_productService.ProductTest();

            ////Category cat = new Category();
            ////cat.Name = "test2";
            ////_categoryService.Insert(cat);
            ////Product product = new Product();
            ////product.Name = "测试产品";
            ////product.Category = cat;
            ////_productService.Insert(product);

            ////product = _productService.GetById(new Guid("c8d3e0db-b737-4f71-85a3-5599f816dcff"));
            ////product.Name = "修改测试！";
            ////_productService.Update(product);

            
            //#region 枚举扩展的使用
            ////dropdownlist的数据源
            //SelectList dropdownList = new SelectList(EnumExtensions.ToListEnumItem<LoanCategory>(), "EnumValue", "EnumKey");
            ////枚举字符串key转为枚举
            //LoanCategory lc = "Operate".ToEnum<LoanCategory>();
            //ViewBag.Lc = lc;
            ////枚举字符串描术转化为枚举
            //LoanCategory lc1 = "经营贷".DescriptionToEnum<LoanCategory>();
            //ViewBag.Lc1 = lc1;
            ////枚举字符串
            //string lcstr = ((LoanCategory)1).ToString();
            //ViewBag.LcStr = lcstr;
            ////枚举值
            //string lcint = ((int)LoanCategory.Operate).ToString();
            //ViewBag.LcInt = lcint;
            ////枚举属性-获取描述
            //string lcdes = LoanCategory.Operate.Discription();
            //ViewBag.LcDes = lcdes;

            //#endregion


            //_productService.Delete(new Dictionary<string, object>() { { "Id", new Guid[] { new Guid("C53F8400-BBFA-4A72-B2E5-F601868D0FE3"), new Guid("6D4B6ED2-DF42-416D-AC83-F6EF0B227432") } } });

            //_productService.Delete(ent => (new Guid[] { new Guid("C53F8400-BBFA-4A72-B2E5-F601868D0FE3"), new Guid("6D4B6ED2-DF42-416D-AC83-F6EF0B227432") }).Contains(ent.Id));

            //_productService.Update(ent => ent.Id == new Guid("9E5BD893-8A4A-4023-A990-F04AF319BD52"), () => new Product() { Name="测试linq更新"});

            //IList<Product> products = _productService.GetList("");

            //IList<Product> products1 = _productService.GetList(" from Product");

            //IList<Product> products2 = _productService.GetList(" where 1=1");

            //PagedList<Product> products3 = _productService.GetPageList("", 2, 10);

            //PagedList<Product> products4 = _productService.GetPageList(p => p.CategoryId == new Guid("974d69a8-5b6a-4da3-a8b8-0865693fb3c2"),"Id", 1, 10);

            //IList<Category> category = _productService.GetList<Category>("from Category");

            //IList<dynamic> dynamicCat = _productService.GetList<dynamic>("from Category");
            //string catName = dynamicCat[0].ToString();

            //OceanDynamicList<object> dynamicList = _productService.GetDynamicList("", null);
            //foreach (dynamic d in dynamicList)
            //{
            //    string n = d.Name;
            //}

            //products = _productService.Table.Take(10).ToList();


            return View();
        }
        [HttpPost]
        public ActionResult Index(string a)
        {
            if (ModelState.IsValid)
            {
                UpFileEntity fileEntity0 = new UpFileEntity() { Size = 2048 };
                fileEntity0.Dir = "weixin/";
                fileEntity0.AllowType = ".jpg,.jpeg,.gif,.png,.bmp";
                UploadProvider.GetInstance().UploadFile(Request.Files["testfile"], fileEntity0);
            }
            return View();
        }

        #region 定义单元格常用到样式的枚举
        public enum stylexls
        {
            头,
            url,
            时间,
            数字,
            钱,
            百分比,
            中文大写,
            科学计数法,
            默认
        }
        #endregion

        #region 定义单元格常用到样式
        static ICellStyle Getcellstyle(IWorkbook wb, stylexls str)
        {
            ICellStyle cellStyle = wb.CreateCellStyle();
            //定义几种字体  
            //也可以一种字体，写一些公共属性，然后在下面需要时加特殊的  
            IFont font12 = wb.CreateFont();
            font12.FontHeightInPoints = 10;
            font12.FontName = "微软雅黑";
            IFont font = wb.CreateFont();
            font.FontName = "微软雅黑";
            //font.Underline = 1;下划线  
            IFont fontcolorblue = wb.CreateFont();
            fontcolorblue.Color = HSSFColor.OliveGreen.Blue.Index;
            fontcolorblue.IsItalic = true;//下划线  
            fontcolorblue.FontName = "微软雅黑";
            //边框  
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Dotted;
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Hair;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Hair;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Dotted;
            //边框颜色  
            cellStyle.BottomBorderColor = HSSFColor.OliveGreen.Blue.Index;
            cellStyle.TopBorderColor = HSSFColor.OliveGreen.Blue.Index;
            //背景图形，我没有用到过。感觉很丑  
            //cellStyle.FillBackgroundColor = HSSFColor.OLIVE_GREEN.BLUE.index;  
            //cellStyle.FillForegroundColor = HSSFColor.OLIVE_GREEN.BLUE.index;  
            cellStyle.FillForegroundColor = HSSFColor.White.Index;
            // cellStyle.FillPattern = FillPatternType.NO_FILL;  
            cellStyle.FillBackgroundColor = HSSFColor.Blue.Index;
            //水平对齐  
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            //垂直对齐  
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            //自动换行  
            cellStyle.WrapText = true;
            //缩进;当设置为1时，前面留的空白太大了。希旺官网改进。或者是我设置的不对  
            cellStyle.Indention = 0;
            //上面基本都是设共公的设置  
            //下面列出了常用的字段类型  
            switch (str)
            {
                case stylexls.头:
                    cellStyle.SetFont(font12);
                    break;
                case stylexls.时间:
                    IDataFormat datastyle = wb.CreateDataFormat();
                    cellStyle.DataFormat = datastyle.GetFormat("yyyy/mm/dd");
                    cellStyle.SetFont(font);
                    break;
                case stylexls.数字:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.SetFont(font);
                    break;
                case stylexls.钱:
                    IDataFormat format = wb.CreateDataFormat();
                    cellStyle.DataFormat = format.GetFormat("￥#,##0");
                    cellStyle.SetFont(font);
                    break;
                case stylexls.url:
                    fontcolorblue.Underline = FontUnderlineType.Single;
                    cellStyle.SetFont(fontcolorblue);
                    break;
                case stylexls.百分比:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                    cellStyle.SetFont(font);
                    break;
                case stylexls.中文大写:
                    IDataFormat format1 = wb.CreateDataFormat();
                    cellStyle.DataFormat = format1.GetFormat("[DbNum2][$-804]0");
                    cellStyle.SetFont(font);
                    break;
                case stylexls.科学计数法:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
                    cellStyle.SetFont(font);
                    break;
                case stylexls.默认:
                    cellStyle.SetFont(font);
                    break;
            }
            return cellStyle;
        }
        #endregion

        [HttpGet]
        public ActionResult Excel()
        {
            ////创建工作薄
            //HSSFWorkbook wk = new HSSFWorkbook();
            ////创建一个名称为mySheet的表
            //ISheet tb = wk.CreateSheet("mySheet");
            ////创建一行，此行为第二行
            //IRow row = tb.CreateRow(1);
            //for (int i = 0; i < 20; i++)
            //{
            //    ICell cell = row.CreateCell(i);  //在第二行中创建单元格
            //    cell.SetCellValue(i);//循环往第二行的单元格中添加数据
            //}
            //using (FileStream fs = System.IO.File.OpenWrite(@"c:/myxls2.xls")) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
            //{
            //    wk.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
            //}
            IWorkbook wk = null;

            using (FileStream fs = System.IO.File.OpenRead(@"c:/myxls.xls"))   //打开myxls.xls文件
            {
                wk = new HSSFWorkbook(fs);   //把xls文件中的数据写入wk中  
                ISheet tb = wk.GetSheetAt(0);
                IRow row = tb.GetRow(0);
                ICell cell = row.GetCell(0);
                cell.SetCellValue("测试");
            }

            using (FileStream fs = System.IO.File.OpenWrite(@"c:/myxls2.xls")) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
            {
                wk.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
            }
            //IWorkbook wb = new HSSFWorkbook();
            ////创建表  
            //ISheet sh = wb.CreateSheet("zhiyuan");
            ////设置单元的宽度  
            //sh.SetColumnWidth(0, 15 * 256);
            //sh.SetColumnWidth(1, 35 * 256);
            //sh.SetColumnWidth(2, 15 * 256);
            //sh.SetColumnWidth(3, 10 * 256);
            //int i = 0;
            //#region 练习合并单元格
            //sh.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 3));

            ////CellRangeAddress（）该方法的参数次序是：开始行号，结束行号，开始列号，结束列号。

            //IRow row0 = sh.CreateRow(0);
            //row0.Height = 20 * 20;
            //ICell icell1top0 = row0.CreateCell(0);
            //icell1top0.CellStyle = Getcellstyle(wb, stylexls.头);
            //icell1top0.SetCellValue("标题合并单元");
            //#endregion
            //i++;
            //#region 设置表头
            //IRow row1 = sh.CreateRow(1);
            //row1.Height = 20 * 20;

            //ICell icell1top = row1.CreateCell(0);
            //icell1top.CellStyle = Getcellstyle(wb, stylexls.头);
            //icell1top.SetCellValue("网站名");

            //ICell icell2top = row1.CreateCell(1);
            //icell2top.CellStyle = Getcellstyle(wb, stylexls.头);
            //icell2top.SetCellValue("网址");

            //ICell icell3top = row1.CreateCell(2);
            //icell3top.CellStyle = Getcellstyle(wb, stylexls.头);
            //icell3top.SetCellValue("百度快照");

            //ICell icell4top = row1.CreateCell(3);
            //icell4top.CellStyle = Getcellstyle(wb, stylexls.头);
            //icell4top.SetCellValue("百度收录");
            //#endregion

            //using (FileStream stm = System.IO.File.OpenWrite(@"c:/myMergeCell.xls"))
            //{
            //    wb.Write(stm);
            //}
            return Content("创建成功");
        }

        
    }
}