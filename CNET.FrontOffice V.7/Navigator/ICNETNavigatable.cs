using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.ERP.Client.Common.UI.Library.Navigator
{
  /// <summary>
  /// Interface for icnet navigatable.
  /// </summary>

  public interface ICNETNavigatable
  {

      #region Member Method
      /// <summary>
      /// Shows the form.
      /// </summary>
      /// <param name="formFullName">   Name of the form full. </param>
      /// <param name="specificationImageIndex">     Zero-based index of the image. </param>
      /// <param name="title">The title of the page</param>

      void ShowForm(string formFullName, int imageIndex);

      void RecentUpdated();


      #endregion
  }
 
}
