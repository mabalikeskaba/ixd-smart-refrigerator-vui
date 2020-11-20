using System.IO;
using System.Linq;

namespace SmartRefrigerator.Vui
{
  public class Program
  {
    static void Main()
    {
      var trackableKeywords = File.ReadAllLines("trackable-keywords.txt").ToList();
      new Dialog(trackableKeywords).Begin(); 
    }
  }
}
