using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormationsTool
{
    public class FormationFile
    {
        public List<Formation> Formations { get; set; } = new List<Formation>();

        public bool FormationExists(string name)
        {
            return Formations.Any(f => f.Name == name);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(Formations);
        }
    }
}
