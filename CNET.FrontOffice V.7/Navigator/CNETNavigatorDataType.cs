//***************************************************************************************************
// Assembly	: CNET ERP Desktop
// File		: Navigator\CNETNavigatorDataType.cs
// Company	: CNET Software Technologies P.L.C
//
// Developers	: Andinet Asamnew and Jeti Gemeda
// Created	: 3/15/2015 - 2:48 PM
//***************************************************************************************************
// Copyright (c) 2015 CNET Software Technologies P.L.C. All rights reserved.
// Description:	CNET navigator data type class
//***************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.ERP.Client.Common.UI.Library.Navigator
{

   /// <summary>
   /// CNET navigator.
   /// </summary>

   public class CNETNavigator
   {

       #region Constructors
       /// <summary>
       /// Initializes a new instance of the CNETNavigator class.
       /// </summary>

       public CNETNavigator()
       {

       }

       #endregion

       #region Private Member Variables

       private String name; /// The name
       private List<CNETNavigatorAncester> ancesters = new List<CNETNavigatorAncester>(); /// The ancesters

       #endregion

       /// <summary>
       /// Gets or sets the name.
       /// </summary>
       /// <value>
       /// The name.
       /// </value>

       public String Name
       {
           get { return name; }
           set { name = value; }
       }

       /// <summary>
       /// Gets or sets the ancesters.
       /// </summary>
       /// <value>
       /// The ancesters.
       /// </value>

       public List<CNETNavigatorAncester> Ancesters
       {
           get { return ancesters; }
           set { ancesters = value; }
       }

       /// <summary>
       /// Adds an ancestor.
       /// </summary>
       /// <param name="ancestor">  The ancestor. </param>

       public void AddAncester(CNETNavigatorAncester ancester)
       {
           ancesters.Add(ancester);
       }

  
   }

   /// <summary>
   /// CNET navigator ancestor.
   /// </summary>

   public class CNETNavigatorAncester
   {
       #region Private Member Variables

       private String name; /// The name

       private String tooltip;  /// The tooltip

       private List<CNETNavigatorNode> childs = new List<CNETNavigatorNode>();  /// The childs

       private CNETNavigator creater;   /// The creater

       #endregion

       #region Public Member Variables

       /// <summary>
       /// Gets or sets the name.
       /// </summary>
       /// <value>
       /// The name.
       /// </value>

       public string Name
       {
           get { return name; }
           set { name = value; }
       }
      
      

       /// <summary>
       /// Gets or sets the tooltip.
       /// </summary>
       /// <value>
       /// The tooltip.
       /// </value>

       public String Tooltip
       {
           get { return tooltip; }
           set { tooltip = value; }
       }
   

       /// <summary>
       /// Gets or sets the childs.
       /// </summary>
       /// <value>
       /// The childs.
       /// </value>

       public List<CNETNavigatorNode> Childs
       {
           get { return childs; }
           set {
               childs = value; }
       }

       /// <summary>
       /// Adds the childs.
       /// </summary>
       /// <param name="child"> The child. </param>

       public void AddChilds(CNETNavigatorNode child)
       {
           childs.Add(child);

   //        child.Ancester = this;
       
       }

       
       /// <summary>
       /// Gets or sets the creater.
       /// </summary>
       /// <value>
       /// The creater.
       /// </value>

       public CNETNavigator Creater
       {
           get { return creater; }
           set
           {
               value.AddAncester(this);
               creater = value; }
       }

       #endregion

   }

   /// <summary>
   /// CNET navigator node.
   /// </summary>

   public class CNETNavigatorNode
   {

       //Object Code
       public string Code { get; set; }

       private String name; /// The name

       /// <summary>
       /// Gets or sets the name.
       /// </summary>
       /// <value>
       /// The name.
       /// </value>

       public String Name
       {
           get { return name; }
           set { name = value; }
       }

       private String tooltip;  /// The tooltip

       /// <summary>
       /// Gets or sets the tooltip.
       /// </summary>
       /// <value>
       /// The tooltip. </value>

       public String Tooltip
       {
           get { return tooltip; }
           set { tooltip = value; }
       }

       private Boolean expanded;	/// true if expanded

       /// <summary>
       /// Gets or sets a value indicating whether the expanded.
       /// </summary>
       /// <value>
       /// true if expanded, false if not.
       /// </value>

       public Boolean Expanded
       {
           get { return expanded; }
           set { expanded = value; }
       }

       private CNETNavigatorNode parent;	/// The parent

       /// <summary>
       /// Gets or sets the parent.
       /// </summary>
       /// <value>
       /// The parent.
       /// </value>

       public CNETNavigatorNode Parent
       {
           get { return parent; }
           set
           {
               value.childs.Add(this);
               parent = value; }
       }

       #region Private Member Variables

       private List<CNETNavigatorNode> childs = new List<CNETNavigatorNode>();  /// The childs
       private String formName; /// Name of the form
       private Boolean isSelected;  /// true if this instance is selected
       private Boolean isDisabled;  /// true if this instance is disabled
       private CNETNavigatorAncester ancester = new CNETNavigatorAncester();	/// The ancestor

       #endregion

       #region Public Member Variables

       /// <summary>
       /// Gets or sets the childs.
       /// </summary>
       /// <value>
       /// The childs.
       /// </value>

       public List<CNETNavigatorNode> Childs
       {
           get { return childs; }
           set { childs = value; }
       }

       /// <summary>
       /// Adds a child.
       /// </summary>
       /// <param name="childNode"> The child node. </param>

       public void AddChild(CNETNavigatorNode childNode)
       {
           childNode.parent = this;

           Childs.Add(childNode);
       }

       /// <summary>
       /// Gets or sets the name of the form.
       /// </summary>
       /// <value>
       /// The name of the form.
       /// </value>

       public String FormName
       {
           get { return formName; }
           set { formName = value; }
       }

       private Object tag;  /// The tag

       /// <summary>
       /// Gets or sets the tag.
       /// </summary>
       /// <value>
       /// The tag.
       /// </value>

       public Object Tag
       {
           get { return tag; }
           set { tag = value; }
       }


                                                                               
       /// <summary>
       /// Gets or sets a value indicating whether this instance is selected.
       /// </summary>
       /// <value>
       /// true if this instance is selected, false if not.
       /// </value>

       public Boolean IsSelected
       {
           get { return isSelected; }
           set { isSelected = value; }
       }

      

       /// <summary>
       /// Gets or sets a value indicating whether this instance is disabled.
       /// </summary>
       /// <value>
       /// true if this instance is disabled, false if not.
       /// </value>

       public Boolean IsDisabled
       {
           get { return isDisabled; }
           set { isDisabled = value; }
       }

       

       /// <summary>
       /// Gets or sets the ancestor.
       /// </summary>
       /// <value>
       /// The ancestor.
       /// </value>

       public CNETNavigatorAncester Ancester
       {   
           get { return ancester; }
           set
           {
               if (value != null)
               {
                   value.Childs.Add(this);
                   ancester = value;
               }
           }
       }

       #endregion

   }
     







}
