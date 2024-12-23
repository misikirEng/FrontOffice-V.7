//***************************************************************************************************
// Assembly	: CNET ERP Desktop
// File		: Flyouts\MesssageFlyouts.cs
// Company	: CNET Software Technologies P.L.C
//
// Developers	: Andinet Asamnew and Jeti Gemeda
// Created		: 3/16/2015 - 17:25
//***************************************************************************************************
// Copyright (c) 2015 CNET Software Technologies P.L.C. All rights reserved.
// Description:	messsage flyouts class
//***************************************************************************************************

using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.ERP.Client.Common.UI.Flyouts
{
    public partial class MesssageFlyouts : DevExpress.XtraEditors.XtraForm
    {

        #region Public Member Variables

        public DialogResult OKCancel;
        public  DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction closeAppAction = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction()
        {
            Caption = "Confirm",
            Description = "\t Are you sure you want to exit CNET ERP v2016?",
            //Image = global::CNET.ERP.EventManagement.Properties.Resources.aiga_information
        };


        #endregion

        #region Constructors

        public MesssageFlyouts()
        {
            InitializeComponent();

        }

        #endregion

        #region Public Member Methods

        public void ShowFlyout()
        {
            closeAppAction.Commands.Add(FlyoutCommand.OK);
            closeAppAction.Commands.Add(FlyoutCommand.Cancel);

            closeERPFlyout.Action = closeAppAction;

            OKCancel = wuiViewMessage.ShowFlyoutDialog(closeERPFlyout);

        }

        #endregion
    }
}
