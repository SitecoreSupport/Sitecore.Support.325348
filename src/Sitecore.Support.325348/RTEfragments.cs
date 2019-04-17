using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sitecore.Support.Pipelines.RenderField
{
  /// <summary>
  /// Corrects anchors position in RTE links.
  /// </summary>
  internal class RTEfragments
  {
    /// <summary>
    /// The pattern for finding links.
    /// </summary>
    protected static readonly string pattern = "href=\"";
    /// <summary>
    /// pattenrm used to replacefragments.
    /// It is used in combination with unique ID.
    /// Example: #fragmentcodeCBFC3588DD7B424985A2C73BE5C1E43E
    /// </summary>
    protected static readonly string fragmentcode = "#fragmentcode";

    protected RenderFieldArgs args;
    protected string action = "";

    /// <summary>Sets the correct position of anchors in RTE links.</summary>
    /// <param name="args">The arguments.</param>
    /// <contract>
    ///   <requires name="args" condition="none" />
    /// </contract>
    public void Process(RenderFieldArgs args, string action)
    {
      this.args = args;
      this.action = action;
      Assert.ArgumentNotNull(args, "args");
      if (args.FieldTypeKey == "rich text")
      {
        args.Result.FirstPart = this.CheckLinks(args.Result.FirstPart);
        args.Result.LastPart = this.CheckLinks(args.Result.LastPart);
      }
    }

    /// <summary>
    /// Checks if links exist in the RTE field and modifies them.
    /// </summary>
    /// <param name="text">The text.</param>
    protected string CheckLinks(string text)
    {
      if (!text.Contains(pattern))
      {
        return text;
      }
      int num = 0;
      StringBuilder stringBuilder = new StringBuilder();
      int num2;
      while ((num2 = text.IndexOf(pattern, num, StringComparison.Ordinal)) >= 0)
      {
        int length = num2 - num + pattern.Length;
        stringBuilder.Append(text.Substring(num, length));
        int length2 = text.IndexOf("\"", num2 + pattern.Length + 1, StringComparison.Ordinal) - num2 - pattern.Length;
        string text2 = text.Substring(num2 + pattern.Length, length2);
        num = num2 + pattern.Length + text2.Length;
        text2 = this.FindAnchor(text2);
        stringBuilder.Append(text2);
      }
      stringBuilder.Append(text.Substring(num));
      return stringBuilder.ToString();
    }

    /// <summary>
    /// Changes the position of the anchor in the link if needed.
    /// </summary>
    /// <param name="link">The link.</param>
    protected string FindAnchor(string link)
    {
      int num = link.IndexOf("#");
      if (num < 0)
      {
        // no #fragment
        return link;
      }
      if (action == "pull")
      {
        string fragment = link.Substring(num);
        string tempFragment = fragmentcode + Guid.NewGuid().ToString("N").ToUpper();
        SaveFragment(tempFragment, fragment);
        return link.Substring(0, num) + tempFragment;
      }
      if (action == "push")
      {
        var x = fragmentcode + Guid.NewGuid().ToString("N").ToUpper();
        string leftPart = link.Substring(0, num);
        string tempFragment = link.Substring(num, x.Length);
        string rightPart = link.Substring(num + x.Length);
        return leftPart + rightPart + PullFragment(tempFragment);
      }
      return link;
    }

    protected void SaveFragment(string tempKey, string realFragment)
    {
      if (args.CustomData.ContainsKey(tempKey))
      {
        Sitecore.Diagnostics.Log.Error("bug#325348: Contact Sitecore support. Unexpected duplicate is removed: tempKey=" + tempKey + ", realFragment=" + (string)args.CustomData[tempKey], null);
        args.CustomData.Remove(tempKey);
      }
      args.CustomData.Add(tempKey, realFragment);
    }

    protected string PullFragment(string tempKey)
    {
      if (args.CustomData.ContainsKey(tempKey) == false)
        return "";

      string retValue = args.CustomData[tempKey] as string;
      args.CustomData.Remove(tempKey);
      return retValue;
    }
  }
}