using System.Web.Mvc;

namespace Ocean.Web.Areas.UCenter
{
    public class UCenterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UCenter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "UCenter_Default",
                "UCenter/{controller}/{action}/{id}",
                new { controller="Main", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}