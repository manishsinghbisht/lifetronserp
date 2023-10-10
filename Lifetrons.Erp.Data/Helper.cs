using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Data
{
    public class Helper
    {
        /// <summary>
        /// Never change this string constant. Its stored in database and used for separation.
        /// </summary>
        public const string SysSeparator = " ___";

        public static readonly string ConnectionStringName;
        public static readonly string AppName = ConfigurationManager.AppSettings["AppName"];
        public static readonly string AppHomeURL = ConfigurationManager.AppSettings["AppHomeURL"];
        public static readonly string AppSupportEmail = ConfigurationManager.AppSettings["AppSupportEmail"];
        public static readonly string SupportUserId = ConfigurationManager.AppSettings["SupportUserId"];
        public static readonly string SupportUserOrgId = ConfigurationManager.AppSettings["SupportUserOrgId"];
        public static readonly string EmailConfigIdForFilesOps = ConfigurationManager.AppSettings["EmailConfigIdForFilesOps"];
        public static readonly string BackupEmailForFilesOps = ConfigurationManager.AppSettings["BackupEmailForFilesOps"];
        public static readonly string SMSServiceEnabled = ConfigurationManager.AppSettings["SMSServiceEnabled"];

        public static string RemoveSystemAppendedDateTime(string name)
        {
            Regex rgx = new Regex(@"\d{2}:\d{2}:\d{2}");
            Match match = rgx.Match(name);
            if (match.Index > 12)
            {
                int index = match.Index - 12;
                string dateString = name.Substring(index);
                name = name.Replace(dateString, "");
            }
            return name;
        }

        public static string RemoveSysSeparator(string name)
        {
            int sysSeparatorIndex = name.LastIndexOf(Helper.SysSeparator);

            if (sysSeparatorIndex == -1) return name;

            return name.Substring(0, sysSeparatorIndex);
        }

        public enum AddressType
        {
            Billing,
            Shipping
        }

        public static bool ValidExtensionForFileJob(string fileType, string fileName)
        {
            string validExtensions = Lifetrons.Erp.Data.Helper.FileExtensions[fileType];
            var extensions = validExtensions.Split(',');
            foreach (var extension in extensions)
            {
                if(fileName.Contains(extension))
                {
                    return true;
                }
            }

            return false;
        }

        public static readonly Dictionary<string, string> FileExtensions = new Dictionary<string, string>
        {
            { "CV", ".doc,.docx,.rtf,.pdf" },
            { "Image", ".jpeg,.jpg,.png" },
            { "Transcription", ".mp4,.mp3,.wav" },
            { "Video", ".mp4,.mkv" },
        };

        public enum FileRateType
        {
            PerPageUrgent,
            PerPageNonUrgent,
            PerFileUrgent,
            PerFileNonUrgent
        }

        public enum FileType
        {
            CV,
            Image,
            Transcription,
            Video
        }

        public enum FileStatus
        {
            Queued,
            Assigned,
            Review,
            Approved,
            Submitted,
            Delivered,
            Rejected
        }

        public static readonly Dictionary<string, string> SystemDefinedEnterpriseStages = new Dictionary<string, string>
        {
            { "Planning", "846DEE0E-B400-4BA8-8BC4-EA9B3651DFB8" },
            { "Stock", "6C146128-2B84-426E-A373-FA1A63758A05" },
            { "Procurement", "DD82DA9B-CE4A-4CEB-A93F-DF41873BCC09" },
            { "Production", "442E4CA7-1D3A-4589-BA3E-738B233E9B95" },
            { "Logistics", "7B3B9C4E-6D41-407B-9821-F2CE8DE77E28" },
            { "Services & Activities", "7D80B392-57F8-422F-BF58-D3BD36EAE666" },
        };

        public static readonly Dictionary<string, string> SystemDefinedProcesses = new Dictionary<string, string>
        {
            { "Planning", "4391D948-386E-4EA7-9AF2-DA045DD76FFF" },//Start or First Process of Planning stage
            { "Raw Stock", "4C1E26D9-CB75-4C57-831C-AB062257F81B" },
            { "FG Stock", "D50CCE64-9D16-4BD2-AB2F-33838DB0267F" },
            { "Scrap Stock", "6379561C-2E96-E411-A369-F0921C571FF8" },
            { "Procurement", "C66C6A14-2E96-E411-A369-F0921C571FF8" },
            { "Assembly","E780BC09-2E96-E411-A369-F0921C571FF8" }, //Start or First Process of Production/Assembly stage
            { "Quality", "74A231FE-2D96-E411-A369-F0921C571FF8" },
            { "Packing","4A92DFF3-2D96-E411-A369-F0921C571FF8" },
            { "Dispatch","213444E4-2D96-E411-A369-F0921C571FF8" },
            {"Services & Activities", "4CA9CEE4-C2F3-453F-A2B2-441EA9C1EEFD"}
        };

        
        public enum JobStatus
        {
            NotStarted = 0,
            InProcess = 1,
            Completed = 2
        }

        public static byte[] StreamToByte(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
            //using (var binaryReader = new BinaryReader(input)
            //{
            //    return binaryReader.ReadBytes(input);
            //}
        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class ExcludeFromEqualityComparison : System.Attribute
    {
        private string _propertyName;
        public ExcludeFromEqualityComparison(string propertyName)
        {
            this._propertyName = propertyName;
        }
    }
}
