using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Framework.Caching.Cache;
using Ocean.Entity.Enums;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Core.ExceptionHandling;
using Ocean.Entity.DTO;
using Ocean.Core.Utility;
using Senparc.Weixin.MP.CommonService.CustomMessageHandler;
using Senparc.Weixin.MP.MvcExtension;

namespace Ocean.Web.Controllers
{
    public class BranchController : PageBaseController //WeixinOAuthController
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            this._branchService = branchService;
        }

        /// <summary>
        /// 网点地图
        /// </summary>
        [HttpGet]
        public ActionResult BranchMap(Guid id)
        { 
            Branch branch = _branchService.GetById(id);
            return View(branch);
        }

        /// <summary>
        /// 网点地图2
        /// </summary>
        [HttpGet]
        public ActionResult BranchMap2(Guid id)
        {
            ViewBag.longitude = RQuery["longitude"];
            ViewBag.latitude = RQuery["latitude"];
            Branch branch = _branchService.GetById(id);
            return View(branch);
        }

        /// <summary>
        /// 网点信息
        /// </summary>
        [HttpGet]
        public ActionResult BranchInfo(Guid id)
        {
            ViewBag.longitude = RQuery["longitude"];
            ViewBag.latitude = RQuery["latitude"];
            Branch branch = _branchService.GetById(id);
            return View(branch);
        }

        /// <summary>
        /// 按距离获取网点信息
        /// </summary>
        [HttpGet]
        public ActionResult BranchList()
        {
            IList<Branch> listBranch = _branchService.GetALL().Where(b => b.Status == 1).ToList();
            string point = RQuery["point"];
            string _X = "0.00";//经度
            string _Y = "0.00";//纬度

            if (!string.IsNullOrWhiteSpace(point))
            {
                string[] arrPoint = point.Split(',');

                if (arrPoint.Length > 1)
                {
                    _X = arrPoint[0];//经度
                    _Y = arrPoint[1];//纬度
                }
            }

            ViewBag.longitude = _X;
            ViewBag.latitude = _Y;
            //计算距离
            IList<BranchDTO> listBranchDTO = new List<BranchDTO>();

            foreach (Branch branch in listBranch)
            {
                BranchDTO dtoBranch = new BranchDTO(branch);
                dtoBranch.Distance = DistanceHelper.GetDistance(double.Parse(_Y), double.Parse(_X), double.Parse(dtoBranch.Latitude ?? "24.737727"), double.Parse(dtoBranch.Longitude ?? "118.656141"));
                listBranchDTO.Add(dtoBranch);
            }

            if (listBranchDTO != null && listBranchDTO.Count > 0)
            {
                listBranchDTO = listBranchDTO.OrderBy(b => b.Distance).ToList();
            }

            return View(listBranchDTO);
        }

        [HttpGet]
        public ActionResult BranchListMap()
        {
            IList<Branch> listBranch = _branchService.GetALL().Where(b => b.Status == 1).ToList();
            string point = RQuery["point"];
            string _X = "0.00";//经度
            string _Y = "0.00";//纬度

            if (!string.IsNullOrWhiteSpace(point))
            {
                string[] arrPoint = point.Split(',');

                if (arrPoint.Length > 1)
                {
                    _X = arrPoint[0];//经度
                    _Y = arrPoint[1];//纬度
                }
            }

            ViewBag.longitude = _X;
            ViewBag.latitude = _Y;
            //计算距离
            IList<BranchDTO> listBranchDTO = new List<BranchDTO>();

            foreach (Branch branch in listBranch)
            {
                BranchDTO dtoBranch = new BranchDTO(branch);
                dtoBranch.Distance = DistanceHelper.GetDistance(double.Parse(_Y), double.Parse(_X), double.Parse(dtoBranch.Latitude ?? "24.737727"), double.Parse(dtoBranch.Longitude ?? "118.656141"));
                listBranchDTO.Add(dtoBranch);
            }

            if (listBranchDTO != null && listBranchDTO.Count > 0)
            {
                listBranchDTO = listBranchDTO.OrderBy(b => b.Distance).ToList();
            }

            listBranchDTO = listBranchDTO.Take(10).ToList();
            return View(listBranchDTO);
        }

        [HttpGet]
        public ActionResult ATMInfo()
        {
            string point = RQuery["point"];
            string _X = "0.00";//经度
            string _Y = "0.00";//纬度

            if (!string.IsNullOrWhiteSpace(point))
            {
                string[] arrPoint = point.Split(',');

                if (arrPoint.Length > 1)
                {
                    _X = arrPoint[0];//经度
                    _Y = arrPoint[1];//纬度
                }
            }

            ViewBag.longitude = _X;
            ViewBag.latitude = _Y;
            return View();
        }

        /// <summary>
        /// 网点地图2
        /// </summary>
        [HttpGet]
        public ActionResult ShopListMap()
        {
            string point = RQuery["point"];
            string _X = "0.00";//经度
            string _Y = "0.00";//纬度

            if (!string.IsNullOrWhiteSpace(point))
            {
                string[] arrPoint = point.Split(',');

                if (arrPoint.Length > 1)
                {
                    _X = arrPoint[0];//经度
                    _Y = arrPoint[1];//纬度
                }
            }

            ViewBag.longitude = _X;
            ViewBag.latitude = _Y;
            return View();
        }
    }
}