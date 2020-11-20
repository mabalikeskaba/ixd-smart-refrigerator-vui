using System.Collections.Generic;
using System.Linq;

namespace SmartRefrigerator.Vui
{
  public class DialogNode
  {
    public int Id { get; set; }
    public string SpeechText { get; set; }
    public IList<PathNode> AnswerNodeIds { get; set; }
    public IList<string> PostSpeakingActions { get; set; }
    public int AlternateNode { get; set; }
    public bool AskOnce { get; set; } = false;
    public bool IsEndNode => AnswerNodeIds.Count == 0;
    public bool IsListenOnly => AnswerNodeIds.FirstOrDefault()?.AnswerNode == 0;
  }
}
