namespace SmartRefrigerator.Vui
{
  public class DialogActions
  {
    private ShoppingList mShoppingList;

    public DialogActions(ShoppingList shoppingList)
    {
      mShoppingList = shoppingList;
    }

    public void ExecuteAction(string actionName, string[] voiceInput = null)
    {
      switch(actionName)
      {
        case "AddToShoppingList": mShoppingList.Add(voiceInput); break;
        case "AddLasagnaIngredients": mShoppingList.AddLasagnaIngredients(); break;
        case "SpeakShoppingList": mShoppingList.SpeakShoppingList(); break;
      }
    }
  }
}
