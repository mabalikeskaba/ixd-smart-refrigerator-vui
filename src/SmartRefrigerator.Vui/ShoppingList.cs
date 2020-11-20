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
      if (voiceInput != null)
      {
        foreach (var inputString in voiceInput)
        {
          if (mTrackableKeywords.Contains(inputString.ToLower()) && !mShoppingList.Contains(inputString))
          {
            mShoppingList.Add(inputString);
          }
        }
      }
    }

    public void AddLasagnaIngredients()
    {
      Add(new string[] { "lasagna plates", "vegetables", "cheese", "salt", "pepper", "tomato sauce" });
    }

    public void SpeakShoppingList()
    {
      var i = 0;
      foreach (var item in mShoppingList)
      {
        if(i == mShoppingList.Count - 1)
          VoiceSynthesizer.Instance().Speak("and");
        VoiceSynthesizer.Instance().Speak(item);
        i++;
      }
    }
  }
}
