using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.ViewModels.Common
{
    public class ServiceCallResult
    {
        public ServiceCallResult()
        {
            ErrorMessages = new List<string>();
            WarningMessages = new List<string>();
            SuccessMessages = new List<string>();
        }
        public bool Success { get; set; }
        public object Item { get; set; }

        public IList<string> ErrorMessages { get; set; }
        public IList<string> SuccessMessages { get; set; }
        public IList<string> WarningMessages { get; set; }
    }
}
