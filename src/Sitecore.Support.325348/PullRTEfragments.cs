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
  /// Moves all anchors/fragments into args CustomData. (a unique key placed instead in the field)
  /// </summary>
  public class PullRTEfragments
  {
    public void Process(RenderFieldArgs args)
    {
      new RTEfragments().Process(args, "pull");
    }
  }
  /// <summary>
  /// Get all anchors/fragments from args CustomData into the end of every link where it was taken from.
  /// </summary>

}