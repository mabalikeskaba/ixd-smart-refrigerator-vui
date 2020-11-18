using System.Collections.Generic;

namespace SmartRefrigerator.Vui
{
  public class DialogStructure
  {
    public IList<DialogNode> DialogNodes { get; set; }
    public IList<AnswerNode> AnswerNodes { get; set; }
  }
}
