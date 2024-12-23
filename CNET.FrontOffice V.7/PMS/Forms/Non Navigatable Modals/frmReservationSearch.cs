using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Repository;
using System;
using CNET.ERP.Client.UI_Logic.PMS.Contracts;
using CNET.ERP.Client.Common.UI.Library;
using CNET.ERP.ResourceProvider;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms
{
    public partial class frmReservationSearch : UILogicBase, ILogicHelper
    {
        public frmReservationSearch()
        {
            InitializeComponent();
            FormSize = new System.Drawing.Size(958, 522);
            
            ApplyIcons();
        }


        private void ApplyIcons()
        {
            ImageProvider.AssignIcon(bbiSearch, CNETStandardIcons.SEARCH);
        }

        public void InitializeUI()
        {
            if (!DesignMode)
            {
                Utility.AdjustRibbon(lciRibbonHolder);
            }
            CNETFooterRibbon.ribbonControl = rcRegistrationSearch;

            InitializeHeader();
        }

        public void InitializeData()
        {
        }

        public void LoadData(UILogicBase requesterForm, object args)
        {
        }

        public RibbonPageGroup CreateGroup()
        {
            DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup;

            ribbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpMain.Groups.Add(ribbonPageGroup);
            ribbonPageGroup.Text = " ";

            return ribbonPageGroup;
        }

        public BarEditItem CreateBarEditItem(String name, RibbonPageGroup group)
        {
            DevExpress.XtraBars.BarEditItem barEditItem;

            barEditItem = new DevExpress.XtraBars.BarEditItem();

            rcRegistrationSearch.Items.Add(barEditItem);

            group.ItemLinks.Add(barEditItem);

            barEditItem.Caption = name;
            barEditItem.Width = 150;
            barEditItem.EditHeight = 15;

            return barEditItem;
        }

        public void CreateRepo(RepositoryItem repositoryItem, RibbonPageGroup parentGroup, String text)
        {
            var barEditItem = CreateBarEditItem(text, parentGroup);

            barEditItem.Edit = repositoryItem;
        }

        public RepositoryItem CreateTextEdit(String name, RibbonPageGroup parentGroup)
        {
            DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit;

            repositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();

            ((System.ComponentModel.ISupportInitialize)(repositoryItemTextEdit)).BeginInit();
           rcRegistrationSearch.RepositoryItems.Add(repositoryItemTextEdit);
            repositoryItemTextEdit.AutoHeight = false;
            ((System.ComponentModel.ISupportInitialize)(repositoryItemTextEdit1)).EndInit();

            CreateRepo(repositoryItemTextEdit, parentGroup, name);

            return repositoryItemTextEdit;
        }

        public  CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacCompnay = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacGroup = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacSource = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacAgent = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacBlock = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacCorpNo = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacIATANo = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacCRSNo = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacNum = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacMemoType = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        public CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic cacCommunication = new CNETAdvancedCombo.UserControl.CNETAdvancedCombo.Logic();
        private RepositoryItemTextEdit repoName = null;

        public void InitializeHeader()
        {
            var group1 = CreateGroup();

            repoName = new RepositoryItemTextEdit();

            CreateRepo(repoName, group1, "Name   ");

            var companyProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            companyProperty.ColumnFields = new String[] { "Id", "name" };
            cacCompnay.SetProperty(companyProperty);
            CreateRepo(cacCompnay.GetInPlaceEditor(), group1, "Company");

            var group2 = CreateGroup();

            var groupProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            groupProperty.ColumnFields = new String[] { "Id", "name" };
            cacGroup.SetProperty(groupProperty);
            CreateRepo(cacGroup.GetInPlaceEditor(), group2, "Group ");


            var sourceProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            sourceProperty.ColumnFields = new String[] { "Id", "name" };
            cacSource.SetProperty(sourceProperty);
            CreateRepo(cacSource.GetInPlaceEditor(), group2, "Source");
            var group3 = CreateGroup();
            var agentProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            agentProperty.ColumnFields = new String[] { "Id", "name" };
            cacAgent.SetProperty(agentProperty);
            CreateRepo(cacAgent.GetInPlaceEditor(), group3, "Agent");


            var blockProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            blockProperty.ColumnFields = new String[] { "Id", "name" };
            cacBlock.SetProperty(blockProperty);
            CreateRepo(cacBlock.GetInPlaceEditor(), group3, "Block");

            var group4 = CreateGroup();

            var corpNoProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            corpNoProperty.ColumnFields = new String[] { "Id", "name" };
            cacCorpNo.SetProperty(corpNoProperty);
            CreateRepo(cacCorpNo.GetInPlaceEditor(), group4, "Corp No");

            var IATANoProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            IATANoProperty.ColumnFields = new String[] { "Id", "name" };
            cacIATANo.SetProperty(IATANoProperty);
            CreateRepo(cacIATANo.GetInPlaceEditor(), group4, "IATA No");

            var group5 = CreateGroup();

            var crsNoProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            crsNoProperty.ColumnFields = new String[] { "Id", "name" };
            cacCRSNo.SetProperty(crsNoProperty);
            CreateRepo(cacCRSNo.GetInPlaceEditor(), group5, "CRS No");

            var numProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            numProperty .ColumnFields = new String[] { "Id", "name" };
            cacNum.SetProperty(numProperty);
            CreateRepo(cacNum.GetInPlaceEditor(), group5, "####  ");

            var group6 = CreateGroup();

            var repoArrivalFrom = new RepositoryItemTimeEdit();
            CreateRepo(repoArrivalFrom, group6, "Arrival From");

            var repoArrivalTo = new RepositoryItemTimeEdit();
            CreateRepo(repoArrivalTo, group6, "Arrival To  ");


            var group7 = CreateGroup();

            var memoTypeProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            memoTypeProperty.ColumnFields = new String[] { "Id", "name" };
            cacMemoType.SetProperty(memoTypeProperty);
            CreateRepo(cacMemoType.GetInPlaceEditor(), group7, "Memory Type ");

            var communicationProperty = CNETAdvancedCombo.Property.CNETAdvancedComboProperty.GetDefaultInstance();
            communicationProperty.ColumnFields = new String[] { "Id", "name" };
            cacCommunication.SetProperty(communicationProperty);
            CreateRepo(cacCommunication.GetInPlaceEditor(), group7, "Communication");
        }
    }
}
