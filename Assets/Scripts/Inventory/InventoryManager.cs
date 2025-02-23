using System.Collections;
using System.Collections.Generic;


public class InventoryManager : SingletonPersistent<InventoryManager>
{
   public List<Objects> nowBags;

   public void Addobj(Objects newObj)
   {
      nowBags.Add(newObj);
   }
}
