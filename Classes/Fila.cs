using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.Classes
{
    public class Fila
    {
        public List<string> GetVideos { get; } = new List<string>();

        public List<string> Enfileira(string url)
        {
            GetVideos.Add(url);
            return GetVideos;
        }

        public string Desenfileira()
        {
            string url = GetVideos.First();
            GetVideos.RemoveAt(0);
            return url;
        }

        public bool IsEmpty()
        {
            return GetVideos.Count == 0 ? true : false;
        }
    }
}
