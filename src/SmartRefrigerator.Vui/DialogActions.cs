namespace SmartRefrigerator.Vui
{
  public class DialogActions
  {
    private ShoppingList mShoppingList;

    public DialogActions(ShoppingList shoppingList)
    {
      mShoppingList = shoppingList;
    }

    public void ExecuteAction(string actionName)
    {
      switch(actionName)
      {
        case "SpeakShoppingList": mShoppingList.SpeakShoppingList(); break;
      }
    }
  }
}
