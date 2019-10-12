using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;

public partial class WitAiSender : MonoBehaviour
{
    private bool actionFound = false;
    void Handle (string jsonString)
    {
        string myItem = "";
        string myOrigin = "";
        string myDestination = "";

        if (jsonString != null)
        {
            RootObject theAction = new RootObject ();
			JsonConvert.PopulateObject (jsonString, theAction);

            myResultBox.text = jsonString;  //Debug only

            if (theAction.entities.intent != null &&
               theAction.entities.intent[0].value == "chessmove")
            {
                if (theAction.entities.item != null)
                {
                    foreach (item _item in theAction.entities.item)
                    {
                        Debug.Log(_item.value);
                        myItem += _item.value;
                        actionFound = true;
                    }
                }
                if (theAction.entities.origin != null)
                {
                    foreach (origin _origin in theAction.entities.origin)
                    {
                        Debug.Log(_origin.value);
                        myOrigin += "," + _origin.value;
                        actionFound = true;
                    }
                }
                if (theAction.entities.destination != null)
                {
                    foreach (destination destination in theAction.entities.destination)
                    {
                        Debug.Log(destination.value);
                        myDestination += "," + destination.value;
                        actionFound = true;
                    }
                }
            }

            #region Fix Keywords
            myItem = myItem;
            myOrigin = FixSquareCoordinates(myOrigin);
            myDestination = FixSquareCoordinates(myDestination);

            #endregion Fix Keywords

            myResultBox.text += "\n\n" + myItem + "\n" + myOrigin + "\n" + myDestination;



            if (!actionFound)
            {
                myResultBox.text = myResultBox.text  + "\nRequest unknown, please ask a different way.";
			}
            else
            {
				actionFound = false;
			}
 		}//END OF IF

 	}//END OF HANDLE VOID
    private string FixSquareCoordinates(string arg)
    {
        string work = arg.Replace(" ", "").ToLower();
        work = work.Replace("one", "1");
        work = work.Replace("two", "2");
        work = work.Replace("three", "3");
        work = work.Replace("four", "4");
        work = work.Replace("five", "5");
        work = work.Replace("six", "6");
        work = work.Replace("seven", "7");
        work = work.Replace("eight", "8");
        work = work.Replace("nine", "9");
        return work;
    }
}//END OF CLASS
	


public class item
{
    public bool suggested { get; set; }
    public double confidence { get; set; }
    public string value { get; set; }
    public string type { get; set; }
}

public class origin
{
    public bool suggested { get; set; }
    public double confidence { get; set; }
    public string value { get; set; }
    public string type { get; set; }
}

public class destination
{
    public bool suggested { get; set; }
    public double confidence { get; set; }
    public string value { get; set; }
    public string type { get; set; }
}

public class intent
{
    public double confidence { get; set; }
    public string value { get; set; }
}

public class Entities
{
	public List<item> item { get; set; }
    public List<origin> origin { get; set; }
    public List<destination> destination { get; set; }
    public List<intent> intent { get; set; }
}

public class RootObject
{
	public string _text { get; set; }
	public Entities entities { get; set; }
	public string msg_id { get; set; }
}