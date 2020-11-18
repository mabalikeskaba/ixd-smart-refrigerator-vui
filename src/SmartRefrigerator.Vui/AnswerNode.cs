using System.Collections.Generic;
using System.Linq;

namespace SmartRefrigerator.Vui
{
  public class AnswerNode
  {
    public int Id { get; set; }
    public IList<string> Keywords { get; set; }
    public bool IsListAnswer => Keywords.Count == 1 && Keywords.ElementAt(0) == "*";
  }
}
