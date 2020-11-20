using System.Collections.Generic;

namespace SmartRefrigerator.Vui
{
  public class ShoppingList
  {
    private List<string> mShoppingList;
    private List<string> mTrackableKeywords;

    public ShoppingList(List<string> trackableKeywords)
    {
      mShoppingList = new List<string>();
      mTrackableKeywords = trackableKeywords;
    }

    public void Add(string[] voiceInput)
    {
      foreach (var inputString in voiceInput)
      {
        if (mTrackableKeywords.Contains(inputString.ToLower()))
        {
          mShoppingList.Add(inputString);
        }
      }
    }

    public void SpeakShoppingList()
    {
      foreach (var item in mShoppingList)
      {
        VoiceSynthesizer.Instance().Speak(item);
      }
    }
  }
}
