using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Core.Tools
{
    public enum UpdatePolicy { Disable, Always, Daily, Weekly, Montly, Auto }
    public static class UpdateManager
    {
        public static async Task<bool> CheckUpdateSubjectListAsync(DateTime lastUpdate, UpdatePolicy policy)
        {
            switch (policy)
            {
                case UpdatePolicy.Disable:
                    return false;
                case UpdatePolicy.Always:
                    return true;
                case UpdatePolicy.Auto:
                    return false;//Not implemented. TODO: Smart update
                case UpdatePolicy.Daily:
                    break;
                case UpdatePolicy.Weekly:
                    break;
                case UpdatePolicy.Montly:
                    break;
                default:
                    break;
            }
            await Task.Run(()=> { });
            return false;
        }
    }
}
