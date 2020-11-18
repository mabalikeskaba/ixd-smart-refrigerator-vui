using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartRefrigeratorVui
{
  public class Refrigerator
  {
    public bool HasEnoughItems => mFridgeItems.Count >= MINIMUM_ITEM_COUNT;

    private const int MINIMUM_ITEM_COUNT = 5;
    private List<string> mFridgeItems;

    public Refrigerator()
    {
      FillFromTextFile();
    }

    public IEnumerable<string> GetContent()
    {
      return mFridgeItems;
    }

    public string RemoveRandomItem()
    {
      var rnd = new Random();
      var idx = rnd.Next(mFridgeItems.Count);
      var item = mFridgeItems.ElementAt(idx);
      mFridgeItems.RemoveAt(idx);
      return item;
    }

    public void PrintFridgeContent()
    {
      var i = 1;
      foreach (var item in mFridgeItems)
      {
        Console.WriteLine($"{i}. {item}");
        i++;
      }
    }

    private void FillFromTextFile()
    {
      mFridgeItems = File.ReadAllLines("fridge-content.txt").ToList();
    }
  }
}
