//***************************************************************************************************
// Assembly	: CNET ERP Desktop
// File		: Navigator\CNETPageNavigator.cs
// Company	: CNET Software Technologies P.L.C
//
// Developers	: Andinet Asamnew and Jeti Gemeda
// Created		: 3/15/2015 - 2:51 PM
//***************************************************************************************************
// Copyright (c) 2015 CNET Software Technologies P.L.C. All rights reserved.
// Description:	CNET page navigator class
//***************************************************************************************************

using System;   /// The system
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraTreeList.Nodes;
using System.Xml;
 
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using DevExpress.Utils;
using CNET.ERP.Client.Common.UI.Library.DLLLibrary.Navigator;
using CNET.ERP.ResourceProvider;

namespace CNET.ERP.Client.Common.UI.Library.Navigator
{
    /// <summary> CNET page navigator.
    /// </summary>


    public class CNETPageNavigator
    {


        #region Member Variables

        XtraForm targetForm;	/// Target form
        DockManager dockmanager;	/// The dockmanager
        DockPanel mainPanel;	/// The main panel
        public Boolean allowSearch = true; /// true to allow, false to deny search
        DockPanel searchPnl = new DockPanel();  /// The search pnl
        DockPanel favoritesPnl = new DockPanel();   /// The favorites pnl
        public CNETNavigator RootNavigator = new CNETNavigator();   /// The root navigator
        ICNETNavigatable navigatable;   /// The navigatable
        public TreeList searchTree;	/// The search tree
        DevExpress.XtraTreeList.TreeList favoritesTree; /// The favorites tree
        public DevExpress.XtraEditors.ButtonEdit txtSearch;	/// The text search
        DevExpress.XtraTreeList.Columns.TreeListColumn colName; /// Name of the col
        DevExpress.XtraBars.Docking.CustomHeaderButton expandbut;   /// The expandbut
        Boolean showCheckbox = false;   /// true to show, false to hide the checkbox
        Boolean processFavorites = true;	/// true to process favorites
        public CNETNavigatorAncester favAncester;  /// The fav ancestor
        Dictionary<TreeListNode, int> favoritesCollecter = new Dictionary<TreeListNode, int>(); /// The favorites collecter
        public static int navigatorWidth;
        public CNET.ERP.Navigator.DTO.NavigatorDTO DTO;
        bool isFirstItemInList = true;

        DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();  /// The first serializable appearance object

    
        int currentimageIndex = -1; /// Zero-based index of the currentimage
 
        


        #endregion

        #region Public Property

        public static string SelctedNodeTitle { get; set; }


        public  int defaultImageIndex = 0;	/// The default image index
        public static String NavigatorName { get; set; }
        public static String SourceFile { get; set; }
        public static String SourceFilePath { get; set; }
        public static Boolean TouchFriendlyNavigator { get; set; }
        public XmlDocument SourceDoc { get; set; }
        private Boolean searchAndApplyIconsFromLib;

        public Boolean SearchAndApplyIconsFromLib
        {
            get { return searchAndApplyIconsFromLib; }
            set {
                searchAndApplyIconsFromLib = value; }
        }

        public event EventHandler<MenuIconEventArgs> MenuIconAssining;

        public void ApplyDefaultProperty()
        {

            SearchAndApplyIconsFromLib = true;

        }



        
        #endregion

        #region Public Member Methods

        /// <summary> Shows the navigator.
        /// </summary>

        public void ShowNavigator()
        {

            InitializeUIComponents();
           
            if (DTO != null)
                BuildStructure(DTO);
                //BuildStructure(SourceDoc);

            else if (!String.IsNullOrEmpty(SourceFile) )
                BuildStructure(SourceFile);

            else if (!String.IsNullOrEmpty(SourceFilePath))
                BuildStructureFromPath(SourceFilePath);

            else
                return;

            CreateLogOutButton();

            BuildPanels();


        }

        /// <summary>
        /// Shows the navigator.
        /// </summary>
        /// <param name="XMLFileName">  Filename of the XML file. </param>

        public void ShowNavigator(String XMLFileName)
        {
            //LoadData data

            InitializeUIComponents();

            //either read from xml or data base the navigator structure
            BuildStructureFromPath(XMLFileName);

            BuildPanels();


        }

        /// <summary>
        /// Initializes the UI components.
        /// </summary>

        private void InitializeUIComponents()
        {
            ApplyDefaultProperty();

            dockmanager = new DockManager();

            dockmanager.StartSizing += dockmanager_StartSizing;
            dockmanager.Form = targetForm;
           // dockmanager.ActiveChildChanged += dmanager_ActiveChildChanged;
            this.txtSearch = new DevExpress.XtraEditors.ButtonEdit();
            Image backimg = Provider.GetImage("backward", ProviderType.APPLICATIONICON, PictureSize.Dimension_16X16);
            EditorButton clearSearch = new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, backimg, new DevExpress.Utils.SuperToolTip());//hello
            clearSearch.Visible = false;
            this.txtSearch.Properties.Buttons.Clear();
            this.txtSearch.Properties.Buttons.Add(clearSearch);
            this.txtSearch.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtSearch.Properties.Click += new EventHandler(this.txtSearch_Properties_Click);
            this.txtSearch.Size = new Size(294, 20);
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

            colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colName.Caption = "Name";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;

            expandbut = new CustomHeaderButton("Expand All", DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton);

         //    dmanager_ActiveChildChanged(null, new DockPanelEventArgs(dockmanager.RootPanels[0] ));
        }




        void dockmanager_StartSizing(object sender, StartSizingEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CNETPageNavigator"/> class. assigns parent form
        /// and event listener interface(navigatable)
        /// </summary>
        /// <param name="targetForm">   The target form. </param>
        /// <param name="navigatable">  navigatable interface implemeter. </param>

        public CNETPageNavigator(XtraForm targetForm, ICNETNavigatable navigatable)
        {

            //the form that will display the navigator , will act as a parent form
            this.targetForm = targetForm;

            //the class that will receive various evens triggered by navigator 
            this.navigatable = navigatable;


        }

        /// <summary>
        /// Initializes a new instance of the CNETPageNavigator class.
        /// </summary>

        public CNETPageNavigator()
        {
            ApplyDefaultProperty();

        }


        /// <summary>
        /// Gets a tree.
        /// </summary>
        /// <param name="ancestor"> The ancestor. </param>
        /// <returns>
        /// The tree.
        /// </returns>

        public DevExpress.XtraTreeList.TreeList GetTree(CNETNavigatorAncester ancester)
        {
            DevExpress.XtraTreeList.TreeList tree = new DevExpress.XtraTreeList.TreeList();

            buildTree(tree, ancester);

            return tree;


        }

        /// <summary>
        /// Hightlight node.
        /// </summary>
        /// <param name="nodeTag">  The node tag. </param>

        public void HightlightNode(String nodeTag)
        {
            if (String.IsNullOrEmpty(nodeTag)) return;

            foreach (DockPanel dp in dockmanager.Panels)
            {
                if (dp.Text == "Search" || dp.Text == "Favorites") continue;
                if (dp.ControlContainer == null) continue;

                TreeList tl = (TreeList)dp.ControlContainer.Controls[0];

                TreeListNode tln = null;

                int index = -1;

                foreach (TreeListNode tnl in tl.Nodes)
                {

                    tln = FindRecur(tnl, nodeTag, null, ref index);

                    index++;

                    if (tln != null)

                        break;

                }

                if (index != -1 && tln != null)
                {
                   
                    dockmanager.ActivePanel = dp;
                    tl.FindNodeByID(index).Selected = true;
                    dp.Visibility = DockVisibility.AutoHide;
                    //dockmanager.ActivePanel.ActiveChild = dp;

                    //if(tln!=null)break;
                }

            }

        }

        #endregion

        #region Private Member Methods

        /// <summary>
        /// XML node builder recur.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="xmlNode">  The XML node. </param>

        private void XMLNodeBuilderRecur(CNETNavigatorNode sender, XmlNode xmlNode)
        {     
            if (xmlNode.HasChildNodes)
            {
                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    CNETNavigatorNode current = new CNETNavigatorNode();
                    current.Name = child.Attributes["Name"].Value;
                    current.Code = child.Attributes["Id"] != null ? child.Attributes["Id"].Value : "";
                    //current.Tag = sender.Tag.ToString() + "//" + current.Name;
                    current.Tag = current.Code == "" ? sender.Tag.ToString() + "//" + current.Name : sender.Tag.ToString() + "//" + current.Code;
                    current.Tooltip = child.Attributes["ToolTip"].Value;
                    current.Expanded = Boolean.Parse(child.Attributes["Expanded"].Value);
                    current.FormName = child.Attributes["FormName"].Value;
                    current.IsDisabled = Boolean.Parse(child.Attributes["IsDisabled"].Value);
                    current.IsSelected = Boolean.Parse(child.Attributes["IsSelected"].Value);

                    sender.AddChild(current);

                    XMLNodeBuilderRecur(current, child);

                }
            }
        }
        private void XMLNodeBuilderRecur(CNETNavigatorNode sender, CNET.ERP.Navigator.DTO.CNETNavigatorNode xmlNode)
        {
            if (xmlNode.Childs != null && xmlNode.Childs.Count >0)
            {
                foreach (CNET.ERP.Navigator.DTO.CNETNavigatorNode child in xmlNode.Childs)
                {
                    CNETNavigatorNode current = new CNETNavigatorNode();
                    current.Name = child.Name;
                    current.Code = child.Code!= null ? child.Code : "";
                    //current.Tag = sender.Tag.ToString() + "//" + current.Name;
                    current.Tag = current.Code == "" ? sender.Tag.ToString() + "//" + current.Name : sender.Tag.ToString() + "//" + current.Code;
                    current.Tooltip = child.ToolTip;
                    current.Expanded = child.Expanded;
                    current.FormName = child.FormName;
                    current.IsDisabled = child.IsDisabled;
                    current.IsSelected = child.IsSelected;

                    sender.AddChild(current);

                    XMLNodeBuilderRecur(current, child);

                }
            }
        }



        private DevExpress.XtraEditors.SimpleButton sbLogOut;

        public static event EventHandler LogOutClicked;

        void CreateLogOutButton()
        {
            this.sbLogOut = new DevExpress.XtraEditors.SimpleButton();
            this.sbLogOut.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.sbLogOut.Image = Provider.GetImage("Log Off", ProviderType.OTHER);// (Image)CNET.ERP.ResourceProvider.Properties.Resources.Log_Off.ToBitmap();
            this.sbLogOut.Click += sbLogOut_Click;
            this.sbLogOut.Name = "sbLogOut";


            this.sbLogOut.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;

            this.sbLogOut.Size = new System.Drawing.Size(125, 35);
            this.sbLogOut.TabIndex = 1;
            this.sbLogOut.Text = "&Log Out";
            this.sbLogOut.Dock = DockStyle.Bottom;


        }



        void sbLogOut_Click(object sender, EventArgs e)
        {
            if (LogOutClicked != null)
            {
                LogOutClicked.Invoke(this, new EventArgs());
            }

        }
        /// <summary>
        /// Event handler. Called by txtSearch for text changed events.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="e">        Event information. </param>

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(txtSearch.EditValue.ToString().Trim()))
            {
                //hide the delete button
                txtSearch.Properties.Buttons[0].Visible = false;

                searchTree.ClearNodes();
                //hide the search panel if the search txt is empity

                return;
            }

            //show the search panel if the search text is not null
            //searchPnl.Visibility = DockVisibility.AutoHide;

            searchPnl.Dock = DockingStyle.Left;
            //dockmanager.ActivePanel = searchPnl;

            txtSearch.Properties.Buttons[0].Visible = true;

            txtSearch.Focus();

            txtSearch.SelectionStart = txtSearch.EditValue.ToString().Length + 1;

            String searchKey = txtSearch.EditValue.ToString().Trim();

            CNETNavigator searchResult = Search(RootNavigator, searchKey);

            foreach (CNETNavigatorAncester anc in searchResult.Ancesters)
            {

                buildTree(searchTree, anc);

            }

        }

        /// <summary>
        /// Searches for the first match.
        /// </summary>
        /// <param name="targetNavig">  Target navig. </param>
        /// <param name="searchKey">    The search key. </param>
        /// <returns>
        /// .
        /// </returns>

        private CNETNavigator Search(CNETNavigator targetNavig, String searchKey)
        {

            if (targetNavig == null || searchKey == null) return null;

            searchTree.ClearNodes();

            CNETNavigator searchNav = new CNETNavigator();

            CNETNavigatorAncester san = new CNETNavigatorAncester();

            foreach (CNETNavigatorAncester anc in targetNavig.Ancesters)
            {
                foreach (CNETNavigatorNode node in anc.Childs)
                {
                    SearchRecur(node, searchKey, san);
                }
            }

            searchNav.AddAncester(san);

            return searchNav;

        }

        /// <summary>
        /// Searches for the first recur.
        /// </summary>
        /// <param name="searchNode">   The search node. </param>
        /// <param name="searchKey">    The search key. </param>
        /// <param name="result">       The result. </param>

        private void SearchRecur(CNETNavigatorNode searchNode, String searchKey, CNETNavigatorAncester result)
        {

            if (searchNode.Childs.Count > 0)
            {
                foreach (CNETNavigatorNode child in searchNode.Childs)
                {

                    SearchRecur(child, searchKey, result);

                }

            }

            if (searchNode.Name.ToLower().Contains(searchKey.ToLower()) || searchKey.ToLower().Contains(searchNode.Name.ToLower()))
            {
                //searchNode.Tag = GetFullName(searchNode);
                result.AddChilds(searchNode);


            }


        }

        /// <summary>
        /// Event handler. Called by txtSearch_Properties for click events.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="e">        Event information. </param>

        private void txtSearch_Properties_Click(object sender, EventArgs e)
        {
            txtSearch.ResetText();

            searchTree.ClearNodes();

        }

        /// <summary>
        /// Searches for the first recur.
        /// </summary>
        /// <param name="searchNode">   The search node. </param>
        /// <param name="searchKey">    The search key. </param>
        /// <param name="answer">       The answer. </param>
        /// <param name="index">        [in,out] Zero-based index of the. </param>
        /// <returns>
        /// The found recur.
        /// </returns>


        private TreeListNode FindRecur(TreeListNode searchNode, String searchKey, TreeListNode answer, ref int index)
        {
            if (answer != null)
            {
                return answer;
            }
            if (searchNode.Tag.ToString().ToLower() == searchKey.ToLower())
            {
                return searchNode;
            }

            if (searchNode.HasChildren)
            {
                foreach (TreeListNode child in searchNode.Nodes)
                {
                    index++;

                    answer = FindRecur(child, searchKey, answer, ref index);

                    if (answer != null) return answer;
                }

            }
            return null;

        }

        #endregion

        #region Public Member Methods

        

        /// <summary>
        /// populate the treelist by creating nodes based on the ancestor.
        /// </summary>
        /// <param name="treelist">                 . </param>
        /// <param name="cNETNavigaterAncestor">    . </param>

        public void buildTree(DevExpress.XtraTreeList.TreeList treelist, CNETNavigatorAncester cNETNavigaterAncestor)
        {

            ImageCollection imageCollection = new ImageCollection();

            if (treelist == null || cNETNavigaterAncestor == null) return;

            treelist.BeginUnboundLoad();

            TreeListNode root = null;
            if (SearchAndApplyIconsFromLib)
            {
                currentimageIndex = defaultImageIndex;
            }
            else
            {
                currentimageIndex = 0;
            }
            int imgindex = -1;

            if (SearchAndApplyIconsFromLib)
            {
                if (cNETNavigaterAncestor.Name != null)
                    try
                    {
                        imgindex = Provider.GetImageIndex(cNETNavigaterAncestor.Name.Trim().Replace(' ', '_'), ProviderType.APPLICATIONICON, pictureSize);
                    }
                    catch { }
            }
            if (imgindex == -1) imgindex = currentimageIndex;
            else currentimageIndex = imgindex;

            int x = currentimageIndex;

            foreach (CNETNavigatorNode n in cNETNavigaterAncestor.Childs)
            {
                //n.Expanded = true;

                build(root, n, treelist, n.Tag.ToString());

            }

            treelist.EndUnboundLoad();

        }
        //default image index if no image index is specified


        /// <summary>
        /// Builds.
        /// </summary>
        /// <param name="tlNode">   The tl node. </param>
        /// <param name="navnode">  The navnode. </param>
        /// <param name="treeList"> List of trees. </param>
        /// <param name="path">     Full pathname of the file. </param>

        public void build(TreeListNode tlNode, CNETNavigatorNode navnode, DevExpress.XtraTreeList.TreeList treeList, String path)
        {

            if (!String.IsNullOrEmpty(NavigatorName)) if (navnode.Name == "_POS_") navnode.Name = NavigatorName;


            //if node is disabled, don't add to the tree node
            if (navnode.IsDisabled) return;

            TreeListNode newnode = treeList.AppendNode(new object[] { navnode.Name }, tlNode);

            //try
            //{

            //    treeList.Nodes.FirstNode.Expanded = true;

            //}
            //catch
            //{

            //}
            int imgindex = 0;
            // if (SearchAndApplyIconsFromLib)
            // {
            string name = "";

            //check whether the node is the child of Voucher(Modules) categories
            if (navnode.Parent != null && navnode.Parent.Name == "Sales and Marketing System")
            {
                name = "Sales and Marketing System";
            }
            else if (navnode.Parent != null && navnode.Parent.Name == "Accounting SubSystem")
            {
                name = "Accounting SubSystem";
            }
            else if (navnode.Parent != null && navnode.Parent.Name == "Warehouse Management System")
            {
                name = "Warehouse Management System";
            }
            else if (navnode.Parent != null && navnode.Parent.Name == "Production SubSystem")
            {
                name = "Production SubSystem";
            }
            else
            {

                if (!string.IsNullOrEmpty(navnode.Code))
                {
                    name = getNavigationNameByCode(navnode.Code);
                    if (string.IsNullOrEmpty(name))
                    {
                        name = navnode.Name;
                    }
                }
                else
                {
                    name = navnode.Name;
                }

            }





            imgindex = Provider.GetImageIndex(name.Replace('*', ' ').Trim(), ProviderType.APPLICATIONICON, pictureSize); ;

            if (imgindex == -1) imgindex = currentimageIndex;

            newnode.ImageIndex = imgindex;
            newnode.SelectImageIndex = newnode.ImageIndex;
            //  }
            /*  else
              {
                  MenuIconEventArgs eveArg = new MenuIconEventArgs(navnode.Name.Trim().Replace(' ', '_'));

                  if (MenuIconAssining != null)
                      MenuIconAssining.Invoke(this,eveArg);

                  if (eveArg.IconImage != null)
                  {
                   //   imageCollection.AddImage(eveArg.IconImage);
                      currentimageIndex++;


                      newnode.ImageIndex = currentimageIndex;
                      newnode.SelectImageIndex = currentimageIndex;

                  }



              }*/


            if (navnode.Parent != null && navnode.Parent.Name == "Recent")
            {
                newnode.Tag = navnode.Tag;
            }
            else
                ////if (treeList.Name == "Search")
                ////    newnode.Tag = path;
                ////else
                ////    newnode.Tag = GetFullName(newnode);
                newnode.Tag = navnode.Tag.ToString() + "_" + navnode.Name;


            if (navnode.Childs.Any())
            {

                if (imgindex != -1) currentimageIndex = imgindex;

                foreach (CNETNavigatorNode node in navnode.Childs)
                {
                    build(newnode, node, treeList, path);
                }
            }

        }

        //   public DevExpress.XtraTreeList.TreeList MainTree;

        public DevExpress.XtraTreeList.TreeList MainTree;
        /// <summary>
        /// Adds a panel.
        /// </summary>
        /// <param name="ancestor"> The ancestor. </param>
        /// <returns>
        /// . </returns>
        public void MainTree_BeforeExpand(object sender, EventArgs e)
        {
            DevExpress.XtraTreeList.TreeList tree = (DevExpress.XtraTreeList.TreeList)sender;
            tree.FocusedNode = null;

        }
        public DockPanel AddPanel(CNETNavigatorAncester ancestor)
        {
            txtSearch.Dock = DockStyle.Top;

            if (mainPanel != null && mainPanel.ParentPanel != null)
            {
                mainPanel.ParentPanel.Tabbed = true;
                mainPanel.MaximumSize = mainPanel.Size;
                mainPanel.MinimumSize = mainPanel.Size;
                mainPanel.Visibility = DockVisibility.AutoHide;

            }


            if (dockmanager.RootPanels.Count == 0)
            {

                mainPanel = dockmanager.AddPanel(DockingStyle.Left);

                dockmanager.Sizing += dockmanager_Sizing;
                dockmanager.StartSizing += dockmanager_StartSizing;

                if (!String.IsNullOrEmpty(NavigatorName))
                    mainPanel.Text = NavigatorName;
                else
                    mainPanel.Text = ancestor.Name;

                mainPanel.Name = ancestor.Name.Trim().Replace(' ', '_');

                mainPanel.TabsPosition = TabsPosition.Left;

                if (SearchAndApplyIconsFromLib)
                    mainPanel.Image = Provider.GetIcon(ancestor.Name, PictureSize.Dimension_16X16);

                mainPanel.Options.ShowCloseButton = false;

                mainPanel.Size = new Size(300, 500);

                mainPanel.CustomHeaderButtons.AddRange(new DevExpress.XtraBars.Docking2010.IButton[] { expandbut });

                mainPanel.CustomButtonClick += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.panelContainerMain_CustomButtonClick);

                MainTree = new DevExpress.XtraTreeList.TreeList();

                if (!String.IsNullOrEmpty(NavigatorName))
                    MainTree.Name = NavigatorName;
                else
                    MainTree.Name = ancestor.Name;
                MainTree.Tag = ancestor.Name;

                MainTree.Click += MainTreeClick;
                MainTree.BeforeExpand += MainTree_BeforeExpand;

                MainTree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { this.colName });

                stylizeTree(MainTree);

                MainTree.Dock = DockStyle.Fill;
                mainPanel.Visibility = DockVisibility.AutoHide;

                buildTree(MainTree, ancestor);

                mainPanel.ControlContainer.Controls.Add(MainTree);

                navigatorWidth = MainTree.Width;
                InitializeDockPanel(dockmanager.Panels[0]);
                return mainPanel;
            }
            else
            {
                DockPanel pnlNew = dockmanager.RootPanels[0].AddPanel();

                pnlNew.Text = ancestor.Name;

                pnlNew.Name = ancestor.Name.Trim().Replace(' ', '_');


                pnlNew.TabsPosition = TabsPosition.Left;



                pnlNew.Options.ShowCloseButton = false;

                pnlNew.CustomHeaderButtons.AddRange(new DevExpress.XtraBars.Docking2010.IButton[] { expandbut });

                pnlNew.CustomButtonClick += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.panelContainerMain_CustomButtonClick);



                DevExpress.XtraTreeList.TreeList tree = new DevExpress.XtraTreeList.TreeList();

                tree.Name = ancestor.Name;

                tree.Tag = ancestor.Name;

                tree.Click += tree_Click;
                tree.BeforeExpand += MainTree_BeforeExpand;

                stylizeTree(tree);

                tree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { this.colName });

                tree.Dock = DockStyle.Fill;

                buildTree(tree, ancestor);

                pnlNew.ControlContainer.Controls.Add(tree);

                mainPanel = pnlNew;

                return pnlNew;

            }

        }
        public void tree_Click(object sender, EventArgs e)
        {
            DevExpress.XtraTreeList.TreeList tree = (DevExpress.XtraTreeList.TreeList)sender;

            TreeListHitInfo hitInfo = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));

            if (hitInfo.Node == null) return;

          

          //  if (hitInfo.Node.HasChildren) return;


            TreeListNode tnode = tree.FocusedNode;
            

         //   if (hi.Node != tnode) return;

            if (navigatable != null)
            {
                if (tnode == null) return;

               
                    //else
                    //    path = GetFullName(tnode);

                    if (processFavorites)
                        AddtofavoritesRecent(tnode);

                    navigatable.ShowForm(tnode.Tag.ToString(), tnode.ImageIndex);
                
                
            }

            return;

        }
        void dockmanager_Sizing(object sender, SizingEventArgs e)
        {
          //  e.Cancel = true;
        }


        /// Builds the panels.
        /// </summary>

        public void BuildPanels()
        {

            //create a dockpanel for each ancesters (gsl,modules....)
            foreach (CNETNavigatorAncester a in RootNavigator.Ancesters)
            {
                AddPanel(a);

            }


            //Add Favorites panel if processFavorites is true 
            if (processFavorites)
            {
                CNETNavigatorAncester favorites = new CNETNavigatorAncester();

                favorites.Name = "Favorites";

                favoritesPnl = AddPanel(favorites);
                favoritesPnl.Expanded += FavoritePnlExpanded;

                favoritesTree = (DevExpress.XtraTreeList.TreeList)favoritesPnl.ControlContainer.Controls[0];


                stylizeTree(favoritesTree);

                 favoritesPnl.Visibility = DockVisibility.AutoHide;

            }


            //ADD Search Panel 
            if (allowSearch)
            {
                CNETNavigatorAncester searchA = new CNETNavigatorAncester();
                searchA.Name = "Search";

                searchPnl = AddPanel(searchA);

                searchPnl.Tabbed = true;
                searchPnl.Expanded += SearchPanelExpanded;

                searchTree = (DevExpress.XtraTreeList.TreeList)searchPnl.ControlContainer.Controls[0];

                searchTree.Click += MainTreeClick;

                stylizeTree(searchTree);

                
                // Add Search box to search pnl
                
                txtSearch.Parent = searchPnl;
                

                searchPnl.Visibility = DockVisibility.AutoHide;


            }

          //  

        }

        private void FavoritePnlExpanded(Object sender, EventArgs args)
        {
            favoritesTree.ClearNodes();

            buildTree(favoritesTree, favAncester);
            //expand recent ones
            if (favoritesTree.Nodes.Count > 0)
                favoritesTree.Nodes[0].ExpandAll();
        }

        protected void SearchPanelExpanded(Object sender, EventArgs args)
        {
            txtSearch.Focus();
        }

     

    public PictureSize pictureSize;
        /// <summary>
        /// Stylizes the tree.
        /// </summary> <param name="tree"> The tree. </param>

        public void stylizeTree(DevExpress.XtraTreeList.TreeList tree)
        {

            if (TouchFriendlyNavigator)
                pictureSize = PictureSize.Dimension_40X40;
            else
                pictureSize = PictureSize.Dimension_24X24;


            if (SearchAndApplyIconsFromLib)
            tree.SelectImageList = Provider.GetImageCollection(pictureSize, ProviderType.APPLICATIONICON);
            tree.Appearance.FocusedCell.BackColor = System.Drawing.SystemColors.Highlight; ;
            tree.Appearance.FocusedCell.Options.UseBackColor = true;
            tree.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.Highlight;
            tree.Appearance.FocusedRow.Options.UseBackColor = true;
            tree.Appearance.Row.BackColor = System.Drawing.Color.White;
            tree.Appearance.Row.Options.UseBackColor = true;
            tree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            tree.Cursor = System.Windows.Forms.Cursors.Hand;
            tree.Location = new Point(845, 27);
            tree.OptionsBehavior.Editable = false;
            tree.OptionsFind.AllowFindPanel = false;
            tree.OptionsFind.FindMode = DevExpress.XtraTreeList.FindMode.Always;
            tree.LookAndFeel.UseDefaultLookAndFeel = true;
            tree.LookAndFeel.SetSkinStyle("Metropolis");
            tree.OptionsFind.ShowClearButton = false;
            tree.OptionsFind.ShowCloseButton = false;
            tree.OptionsView.ShowAutoFilterRow = false;
            tree.OptionsView.ShowColumns = false;
            tree.OptionsView.ShowIndicator = false;
            tree.OptionsView.ShowVertLines = false;
            if (showCheckbox)
                tree.OptionsView.ShowCheckBoxes = true;
            else
                tree.OptionsView.ShowCheckBoxes = false;

        }



        /// <summary>
        /// Addtofavorites recent.
        /// </summary>
        /// <param name="node"> The node. </param>

        public void AddtofavoritesRecent(TreeListNode node)
        {

            //favoritesPnl.Visibility = DockVisibility.AutoHide;

            var favNode = favoritesCollecter.Where(u => u.Key.Tag.ToString() == node.Tag.ToString()).FirstOrDefault();

            if (favNode.Key == null)//not yet added to the collection
            {
                favoritesCollecter.Add(node, 1);
            }
            else
            {
                favoritesCollecter[favNode.Key] = favNode.Value + 1;
            }

            //sort by value

            IEnumerable<TreeListNode> ordered = favoritesCollecter.OrderByDescending(u => u.Value).Select(x => x.Key);

            favAncester = new CNETNavigatorAncester();

            favAncester.Name = "recent";

            CNETNavigatorNode recentNavig = new CNETNavigatorNode();

            recentNavig.Name = "Recent";

            recentNavig.Tag = "";

            recentNavig.Ancester = favAncester;

            foreach (TreeListNode tln in ordered)
            {

                CNETNavigatorNode nod = new CNETNavigatorNode();

                nod.Name = tln.GetDisplayText(0);

                nod.Tag = tln.Tag;

                recentNavig.AddChild(nod);

                //nod.Ancester = favAncester;

            }

            CNETNavigatorNode favNavig = new CNETNavigatorNode();

            favNavig.Name = "Favorites";

            favNavig.Tag = "";

            favNavig.Ancester = favAncester;

            navigatable.RecentUpdated();


        }



        /// <summary>
        /// Reads an XML.
        /// </summary>
        /// <param name="fileFullPath">  Filename of the XML file. </param>

        public void BuildStructureFromPath(String fileFullPath)
        {
            String fileName = String.Empty;
 
            XmlDocument xmldoc = new XmlDocument();
            
            try
            {
                //xmldoc.Load(Application.StartupPath + "\\XMLs\\" + fileName);
                xmldoc.Load(fileFullPath);
            }
            catch { return; }

            BuildStructure(xmldoc);


        }


        public void BuildStructure(String xmlContent)
        {
            XmlDocument xmldoc = new XmlDocument();

            xmldoc.LoadXml(xmlContent);

            BuildStructure(xmldoc);

        }
        private void BuildStructure(CNET.ERP.Navigator.DTO.NavigatorDTO DTO) 
        {
            RootNavigator.Name = DTO.Name;
            foreach (var node in DTO.Ancestors) 
            {
                CNETNavigatorAncester ancester = new CNETNavigatorAncester { Name = node.Name, Tooltip = node.ToolTip };

                foreach (CNET.ERP.Navigator.DTO.CNETNavigatorNode navigNode in node.Childs)
                {
                    CNETNavigatorNode current = new CNETNavigatorNode();
                    if (navigNode !=null && navigNode.Name != null && !string.IsNullOrEmpty(navigNode.Name))
                    {
                        current.Name = navigNode.Name;
                        // current.Tag = ancester.Name + "//" + current.Name;
                        current.Tag = ancester.Name;
                        current.Tooltip = navigNode.ToolTip;
                        current.Expanded =navigNode.Expanded;



                        current.FormName = navigNode.FormName;
                        current.IsDisabled = navigNode.IsDisabled;
                        current.IsSelected = navigNode.IsSelected;
                        XMLNodeBuilderRecur(current, navigNode);
                        ancester.AddChilds(current);
                    }



                }
            
                RootNavigator.AddAncester(ancester);
            }
        }

        private void BuildStructure(XmlDocument xmldoc)
        {

            XmlNodeList Rootnode = xmldoc.SelectNodes("CnetNavigator");

            foreach (XmlNode rn in Rootnode)
            {
                XmlAttribute n= rn.Attributes["Name"];
                RootNavigator.Name = rn.Attributes["Name"]!=null ? rn.Attributes["Name"].Value: null;

                foreach (XmlNode ancesternode in rn.ChildNodes) //Iterate CNETNavigaterAncester
                {
                    CNETNavigatorAncester ancester = new CNETNavigatorAncester();

                    ancester.Name = ancesternode.Attributes["Name"].Value;

                    ancester.Tooltip = ancesternode.Attributes["ToolTip"].Value;

                    //XtraMessageBox.Show(ancestor.Name);

                    foreach (XmlNode navigNode in ancesternode.ChildNodes)
                    {
                        CNETNavigatorNode current = new CNETNavigatorNode();
                        if (navigNode.Attributes["Name"] != null)
                        {
                            current.Name = navigNode.Attributes["Name"].Value;
                            // current.Tag = ancester.Name + "//" + current.Name;
                            current.Tag = ancester.Name;
                            current.Tooltip = navigNode.Attributes["ToolTip"].Value;
                            current.Expanded = Boolean.Parse(navigNode.Attributes["Expanded"].Value);



                            current.FormName = navigNode.Attributes["FormName"].Value;
                            current.IsDisabled = Boolean.Parse(navigNode.Attributes["IsDisabled"].Value);
                            current.IsSelected = Boolean.Parse(navigNode.Attributes["IsSelected"].Value);
                            XMLNodeBuilderRecur(current, navigNode);
                            ancester.AddChilds(current);
                        }
                      


                    }
                    RootNavigator.AddAncester(ancester);

                }

            }

        }



        /// <summary>
        /// Shows the checkboxes on the navigator items.
        /// </summary>
        /// <param name="showCheckbox"> if set to <c>true</c> [show checkbox]. </param>

        public void ShowCheckbox(Boolean showCheckbox)
        {
            this.showCheckbox = showCheckbox;

        }

        /// <summary>
        /// Allow if favorites(follow recent opened pages and favorites list ) should be processed by the
        /// navigator.
        /// </summary>
        /// <param name="processFavouritesStatus"> if set to <c>true</c> [process favorites status]. </param>

        public void ProcessFavorites(Boolean processFavouritesStatus)
        {
            this.processFavorites = processFavouritesStatus;

        }

        /// <summary>
        /// Gets selected nodes.
        /// </summary>
        /// <returns>
        /// The selected nodes.
        /// </returns>

        public List<String> GetSelectedNodes()
        {
            return null;
        }

        /// <summary>
        /// Deactivates the given nodes.
        /// </summary>
        /// <param name="nodes">    The nodes. </param>

        #endregion

        #region Event Listners

        /// <summary>
        /// Event handler. Called by panelContainerMain for custom button click events.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="e">        Event information. </param>
        private void panelContainerMain_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            DockPanel pnl = sender as DockPanel;
            DevExpress.XtraTreeList.TreeList tree;
            try
            {
                
                tree = (DevExpress.XtraTreeList.TreeList)pnl.ControlContainer.Controls[0];
            }
            catch
            {
                return;
            }


            if (e.Button.Properties.Caption == "Expand All")
            {
                e.Button.Properties.Caption = "Collapse All";
                tree.ExpandAll();

            }
            else
            {

                e.Button.Properties.Caption = "Expand All";
                tree.CollapseAll();
            }

        }

        /// <summary>
        /// Event handler. Called by tree for click events.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="e">        Event information. </param>


        public void MainTreeClick(object sender, EventArgs e)
        {
            
            DevExpress.XtraTreeList.TreeList tree = (DevExpress.XtraTreeList.TreeList)sender;

            TreeListHitInfo hitInfo = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));

            //if (hitInfo.Node == null) return;

           

            ////if the node in the hitInfo has children, it is category not last child node
            //if (hitInfo.Node.HasChildren) return;

            TreeListNode tnode = tree.FocusedNode;
            

           // if (hitInfo.Node != tnode) return;
            //TO DO: CREATE A METHOD THAT BUILDS THE FULL PATH OF THE SELECTED NODE GSL//ARTICLE

            if (navigatable != null)
            {
                if (tnode == null) return;

                //check if the selected item is a nodecategory or a node
                string path = string.Empty;
                string title = string.Empty;

                if (!tnode.HasChildren)     
                {
                    //  if (tnode.TreeList.Name.Equals("Favorites") || tnode.TreeList.Name.Equals("Search"))
                    string[] splited = tnode.Tag.ToString().Split('_');
                    path = splited[0];
                    title = splited[1];
                    path.ToLower();
                    //else
                    //    path = GetFullName(tnode);

                    if (processFavorites)
                        AddtofavoritesRecent(tnode);

                    path = POSMenuText(path);

                    navigatable.ShowForm(tnode.Tag.ToString(), tnode.ImageIndex);
                }
                else
                {
                    path = tnode.Tag.ToString(); 
                    navigatable.ShowForm(path,tnode.ImageIndex);
                }
            }

            return;

        }

        public static String POSMenuText(String toReplace)
        {
            if (String.IsNullOrEmpty(toReplace)) return String.Empty;

            if (!toReplace.Contains("_POS_")) return toReplace;

            if (toReplace.Contains("_POS_") && String.IsNullOrEmpty(NavigatorName))

                return toReplace.Replace("_POS_", "POS");

            return toReplace.Replace("_POS_", NavigatorName);


        }

        /// <summary>
        /// Gets full name.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// The full name.
        /// </returns>

        public String GetFullName(TreeListNode node)
        {
            if (node == null) return String.Empty;

            TreeListNode selected = node;

            String answer = String.Empty;

            do
            {
                if (!String.IsNullOrEmpty(selected.GetDisplayText(0)))
                {
                    if (answer.Equals(String.Empty)) answer = selected.GetDisplayText(0);
                    else answer = selected.GetDisplayText(0) + "//" + answer;
                }


                selected = selected.ParentNode;
            }
            while (selected != null);

            if (!String.IsNullOrEmpty(answer))
                answer = node.TreeList.Tag.ToString() + "//" + answer;

            return answer;

        }

        /// <summary>
        /// Event handler. Called by dmanager for active child changed events.
        /// </summary>
        /// <param name="sender">   The sender. </param>
        /// <param name="e">        Event information. </param>

        void dmanager_ActiveChildChanged(object sender, DockPanelEventArgs e)
        {
            DockPanel dockpanel = e.Panel.ActiveChild;

            InitializeDockPanel(dockpanel);
        }

        private void InitializeDockPanel(DockPanel dockpanel)
        {
            //dockpanel.ControlContainer.Controls[0].Visible = false;
            if (dockpanel == null) return;
            //txtSearch.Parent = dockpanel.ControlContainer;
            sbLogOut.Parent = dockpanel.ControlContainer;

            dockpanel.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;


            if (dockpanel.ControlContainer != null)
            {
                dockpanel.ControlContainer.Panel.Controls.Add(sbLogOut);
                //dockpanel.ControlContainer.Panel.Controls.Add(txtSearch);
            }
            //dockpanel.ControlContainer.Controls.RemoveAt(0);
            
            //expand the first node
            if (dockpanel.ControlContainer != null)
                if (dockpanel.ControlContainer.Controls[0].GetType().Equals(typeof(DevExpress.XtraTreeList.TreeList)))
                {
                    DevExpress.XtraTreeList.TreeList tlst = (DevExpress.XtraTreeList.TreeList)dockpanel.ControlContainer.Controls[0];
                    //check if the tree contains nodes
                    if (tlst.Nodes.Count > 0)
                    {
                        tlst.CollapseAll();

                        tlst.Nodes[0].ExpandAll();
                    }

                }
            //txtSearch.Focus();

            expandbut.Caption = "Expand All";

            

            //if (dockpanel.Text.Equals("Favorites"))
            //{
            //    favoritesTree.ClearNodes();

            //    buildTree(favoritesTree, favAncester);
            //    //expand recent ones
            //    if (favoritesTree.Nodes.Count > 0)
            //        favoritesTree.Nodes[0].ExpandAll();

            //}
        }


        #endregion


        // get the navigator name based on the passed code parameter
        // this method is used to get the icon of the navigator menues
        private string getNavigationNameByCode(string code)
        {
            string name = string.Empty;
            switch (code)
            {
                #region GSL


                case "1":

                    name = "ITEM";

                    break;
                case "2":
                    name = "PRODUCT";
                    break;
                case "3":
                    name = "SEMIFINISHED";
                    break;
                case "4":
                    name = "SERVICES";
                    break;
                case "5":
                    name = "FIXEDASSET";
                    break;

                case "6":
                    name = "Customer";
                    break;
                case "7":
                    name = "SUPPLIER";
                    break;
                case "8":
                    name = "AGENT";
                    break;
                case "9":
                    name = "COMPETITOR";
                    break;
                case "10":
                    name = "STAKEHOLDER";
                    break;
                case "11":
                    name = "SHAREHOLDERS";
                    break;
                case "12":
                    name = "INSURANCE";
                    break;
                case "13":
                    name = "BANK";
                    break;
                case "25":
                    name = "GROUP";
                    break;
                case "26":
                    name = "BUSINESSsOURCE";
                    break;


                case "14":

                    name = "Employee";

                    break;
                case "15":

                    name = "Customers";

                    break;
                case "16":

                    name = "Supplier";

                    break;
                case "17":

                    name = "Guest";

                    break;
                case "18":

                    name = "Intern";

                    break;
                case "19":

                    name = "Recruit";

                    break;
                case "20":

                    name = "Patient";

                    break;
                case "21":

                    name = "Owner";

                    break;
                case "22":

                    name = "Agent";

                    break;
                case "23":

                    name = "Shareholder";
                    break;
                case "24":
                    name = "contact";
                    break;

                #endregion


                #region Module


                case "101":
                    name = "presales voucher";
                    break;
                case "102":
                    name = "sales order voucher"; break;
                case "103":
                    name = "proforma invoice";
                    break;
                case "104":
                    name = "contract aggreement";
                    break;
                case "105":
                    name = "item reservation voucher";
                    break;
                case "106":
                    name = "cash sales voucher";
                    break;
                case "107":
                    name = "cash sales summary voucher";
                    break;
                case "108":
                    name = "credit sales voucher";
                    break;
                case "109":
                    name = "sales return voucher";
                    break;
                case "110":
                    name = "settelment advise voucher";
                    break;
                case "111":
                    name = "consignment sales voucher";
                    break;
                case "112":
                    name = "consignment sales return voucher";
                    break;
                case "113":
                    name = "sales Prospect Voucher";
                    break;
                case "114":
                    name = "zeroing Voucher";
                    break;

                case "115":
                    name = "store requisition voucher";
                    break;
                case "116":
                    name = "internal store requisition voucher";
                    break;
                case "117":
                    name = "store order voucher";
                    break;
                case "118":
                    name = "store issue voucher";
                    break;
                case "119":
                    name = "store transfer voucher";
                    break;
                case "120":
                    name = "store return voucher";
                    break;
                case "121":
                    name = "stock adjustment voucher";
                    break;
                case "122":
                    name = "purchase requisition voucher";
                    break;
                case "123":
                    name = "local purchase order voucher";
                    break;
                case "124":
                    name = "purchase order voucher (ipo)";
                    break;
                case "125":
                    name = "goods receiving voucher";
                    break;
                case "126":
                    name = "imported goods receiving voucher";
                    break;
                case "127":
                    name = "internal goods receiving voucher";
                    break;
                case "128":
                    name = "consignment receiving voucher";
                    break;
                case "129":
                    name = "disposal voucher";
                    break;
                case "130":
                    name = "supplier return voucher";
                    break;
                case "131":
                    name = "delivery order voucher";
                    break;
                case "132":
                    name = "dispatch voucher";
                    break;
                case "133":
                    name = "gate pass voucher";
                    break;
                case "134":
                    name = "item consumption voucher";
                    break;
                case "135":
                    name = "petty cash payment voucher";
                    break;
                case "136":
                    name = "bank payment voucher";
                    break;
                case "137":
                    name = "cash receipt voucher";
                    break;
                case "138":
                    name = "bank credit note voucher";
                    break;
                case "139":
                    name = "bank debit note voucher";
                    break;
                case "140":
                    name = "bank transfer voucher";
                    break;
                case "141":
                    name = "bank deposit voucher";
                    break;
                case "142":
                    //logic = new  VoucherSetting(CNETConstantes.ESXPENSESETTLEMET );
                    break;
                case "143":
                    name = "iou voucher";
                    break;
                case "144":
                    name = "payment order voucher";
                    break;
                case "145":
                    name = "45 voucher";
                    break;
                case "146":
                    name = "production request voucher";
                    break;
                case "147":
                    name = "production order voucher";
                    break;
                case "148":
                    name = "batch voucher";
                    break;
                //   case "production subsystem//formulation voucher":
                case "149":
                    name = "formulation definition voucher";
                    break;
                case "150":
                    name = "Packing List Voucher";
                    break;
                case "151":
                    name = "production release voucher";
                    break;
                case "152":
                    name = "registration voucher";
                    break;
                case "153":
                    name = "service ticket voucher";
                    break;

                #endregion
            }

            return name;
        }
    }
}