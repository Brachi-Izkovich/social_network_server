using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Helpers
{
    public class IndexNameResolver
    {
        private readonly IConfiguration _cfg;
        public IndexNameResolver(IConfiguration cfg)
        {
            _cfg = cfg;
        }
        public string GetIndexName()
        {
            var baseName = _cfg["Pinecone:BaseIndexName"] ?? "socialnetwork";
            Console.WriteLine("baseName: " + baseName);
            var machineName = Environment.MachineName.ToLower();

            // מתאימים לפי שמות מחשב (תבדקי מה השם אצלך ואצל החברה שלך)
            if (machineName.Contains("DESKTOP-FKDF8KP"))
                return $"{baseName}-miri";
            else if (machineName.Contains("BRACHIH-COMPUTE"))
                return $"{baseName}-brachi";
            else
                return $"{baseName}-dev"; // ברירת מחדל למחשב אחר
        }
    }
}
