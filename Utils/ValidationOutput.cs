using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Type of objects that will be returned by the methods of the Service classes.
 * This class holds the effective return of the service method a set of errors to be presented to the user.
 */
public abstract class ValidationOutput
{
    /**
     * The expected result from the called Service method.
     */
    public Object DesiredReturn { get; set; }

    /**
     * A set of key-value pairs detailing the encountered error.
     * 
     * Key: What field or component did not meet the stablished requirements.
     * Value: String detailing what business rule was not met.
     */
    public Dictionary<string, string> FoundErrors { get; }

    public ValidationOutput()
    {
        FoundErrors = new Dictionary<string, string>();
    }

    /**
     * Adds an error.
     */
    public void AddError(string attribute, string errorMessage)
    {
        FoundErrors.Add(attribute, errorMessage);
    }

    /**
     * Method that will join the errors found of two ValidationOutput objects
     */
    public bool Join(ValidationOutput other)
    {
        if (!other.GetType().Equals(this.GetType()))
        {
            return false;
        }
        foreach (var key in other.FoundErrors.Keys.ToList())
        {
            FoundErrors.Add(key, other.FoundErrors[key]);
        }
        return true;
    }

    /**
     * Method that will append to the beggining of each key in the FoundErrors map the passed string.
     */
    public void AppendToAllkeys(string strToAppend)
    {
        
        foreach (var key in FoundErrors.Keys.ToList())
        {
            FoundErrors.Add(strToAppend + key, FoundErrors[key]);
            FoundErrors.Remove(key);
        }
    }


    /**
     * Verifies if there are any errors were found.
     */
    public bool HasErrors()
    {
        return FoundErrors.Keys.Count > 0;
    }
}