using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace Avalonia.Raspi
{
    /// <summary>
    /// Reads /proc/cpuinfo to obtain common values
    /// </summary>
    public class LinuxCpuInfo
    {
        public string VendorId { get; private set; }
        public int CpuFamily { get; private set; }
        public int Model { get; private set; }
        public string ModelName { get; private set; }
        public int Stepping { get; private set; }
        public double MHz { get; private set; }
        public string CacheSize { get; private set; }

        public void GetValues()
        {
            string[] cpuInfoLines = File.ReadAllLines(@"/proc/cpuinfo");

            CpuInfoMatch[] cpuInfoMatches =
            {
                new CpuInfoMatch(@"^vendor_id\s+:\s+(.+)", value => VendorId = Convert.ToString(value)),
                new CpuInfoMatch(@"^cpu family\s+:\s+(.+)", value => CpuFamily = Convert.ToInt32(value)),
                new CpuInfoMatch(@"^model\s+:\s+(.+)", value => Model = Convert.ToInt32(value)),
                new CpuInfoMatch(@"^model name\s+:\s+(.+)", value => ModelName = Convert.ToString(value)),
                new CpuInfoMatch(@"^stepping\s+:\s+(.+)", value => Stepping = Convert.ToInt32(value)),
                new CpuInfoMatch(@"^cpu MHz\s+:\s+(.+)", value => MHz = Convert.ToDouble(value)),
                new CpuInfoMatch(@"^cache size\s+:\s+(.+)", value => CacheSize = Convert.ToString(value))
            };

            foreach (string cpuInfoLine in cpuInfoLines)
            {
                foreach (CpuInfoMatch cpuInfoMatch in cpuInfoMatches)
                {
                    Match match = cpuInfoMatch.regex.Match(cpuInfoLine);
                    if (match.Groups[0].Success)
                    {
                        string value = match.Groups[1].Value;
                        cpuInfoMatch.updateValue(value);
                    }
                }
            }
        }

        public class CpuInfoMatch
        {
            public Regex regex;
            public Action<string> updateValue;

            public CpuInfoMatch(string pattern, Action<string> update)
            {
                this.regex = new Regex(pattern, RegexOptions.Compiled);
                this.updateValue = update;
            }
        }
    }
}
