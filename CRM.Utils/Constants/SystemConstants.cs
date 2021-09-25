using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Utils.Constants
{
    public static class SystemConstants
    {

        public const string SuccessMessage = "SuccessMessage";
        public const string WarningMessage = "WarningMessage";
        public const string InfoMessage = "InfoMessage";
        public const string ErrorMessage = "ErrorMessage";

        public const string ImageResizerBlogImageSettings = "maxwidth=1600;maxheight=1600;quality=90;autorotate=false";
        public const string ImageResizerBlogThumbImageSettings = "maxwidth=100;maxheight=100;autorotate=false";
        public const string ImageResizerServiceImageSettings = "maxwidth=1600;maxheight=1600;quality=90;autorotate=false";
        public const string ImageResizerServiceThumbImageSettings = "maxwidth=100;maxheight=100;autorotate=false";

        public static string ServiceServiceImagePath = ConfigurationManager.AppSettings["ServiceService.ImagePath"];
        public static string ServiceServiceImageThumbPath = ConfigurationManager.AppSettings["ServiceService.ImagePath"] + "thumbs\\";
        public static string ServiceServiceTempImagePath = ConfigurationManager.AppSettings["ServiceService.TempImagePath"];
        public static string ServiceServiceTempImageThumbPath = ConfigurationManager.AppSettings["ServiceService.TempImagePath"] + "thumbs\\";
        public static string ServiceImagePath = ConfigurationManager.AppSettings["Service.ImagePath"];
        public static string ServiceImageThumbPath = ConfigurationManager.AppSettings["Service.ImagePath"] + "thumbs/";
        public static string ServiceTempImagePath = ConfigurationManager.AppSettings["Service.TempImagePath"];
        public static string ServiceTempImageThumbPath = ConfigurationManager.AppSettings["Service.TempImagePath"] + "thumbs/";


        public const int DefaultBlogPageSize = 10;
        public const int DefaultServicePageSize = 10;
        public const int DefaultPropertyPageSize = 10;
        public const int DefaultKeywordPageSize = 10;
    }
}
