using System;
using System.Collections.Generic;
using System.Text;

/**
 * Type of objects that will be returned by the methods of the Service classes.
 * This class indicates that the found errors are related to uncoformities when trying to find a resource in the database that does not exist (by reference).
 */
public class ValidationOutputNotFound : ValidationOutput
{

    public ValidationOutputNotFound() : base()
    {
    }
   
    public override string ToString()
    {
        StringBuilder s = new StringBuilder();
        foreach (var st in FoundErrors)
        {
            s.Append(st.Key);
            s.Append(" => ");
            s.Append(st.Value);
            s.Append("\n");
        }
        return s.ToString();
    }
}