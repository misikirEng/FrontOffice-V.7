using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CNET.FrontOffice_V._7
{
    public  class Utility
    {
        public static void AdjustForm(XtraForm form)
        {

         //   return;
            form.SuspendLayout();

            form.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            form.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;

            UILogicBase formlogic = null;
            if ( form.Tag != null)
            {
                formlogic = form.Tag as UILogicBase;
            }
            if (formlogic != null)
            {
                if (formlogic.FormSize == new Size(0, 0))
                {
                    form.ClientSize = new System.Drawing.Size(958, 522);
                }
                else
                {
                    form.ClientSize = formlogic.FormSize;
                }
            }
            
            if(form.MdiParent==null)
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            if (form.Owner != null)
            {
             //   form.Location = form.Owner.Location;
                form.Left += form.Owner.ClientSize.Width / 2 - form.Width / 2;
                form.Top += form.Owner.ClientSize.Height / 2 - form.Height / 2;
            }

            form.ShowIcon = true;
            form.ShowInTaskbar = false;
            form.MaximizeBox = false;
            form.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;

            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;



            form.ResumeLayout(false);
        }

        public static void AdjustRibbon(LayoutControlItem layoutcontrolItem, Boolean addStatusBar = true)
        {
            try
            {
                layoutcontrolItem.Location = new System.Drawing.Point(0, 0);
                layoutcontrolItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
                layoutcontrolItem.ShowInCustomizationForm = false;
                layoutcontrolItem.Size = new System.Drawing.Size(1056, 61);
                layoutcontrolItem.MaxSize = new System.Drawing.Size(2000, 61);
                layoutcontrolItem.MinSize = new System.Drawing.Size(0, 61);
                layoutcontrolItem.TextSize = new System.Drawing.Size(0, 0);
                layoutcontrolItem.TextToControlDistance = 0;
                layoutcontrolItem.TextVisible = false;

                layoutcontrolItem.SizeConstraintsType = SizeConstraintsType.Custom;

                var panel = layoutcontrolItem.Control as PanelControl;

                ((System.ComponentModel.ISupportInitialize)(panel)).BeginInit();
                panel.SuspendLayout();
                panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                panel.Location = new System.Drawing.Point(1, 1);
                panel.Size = new System.Drawing.Size(1056, 58);
                panel.Dock = System.Windows.Forms.DockStyle.Fill;
                ((System.ComponentModel.ISupportInitialize)(panel)).EndInit();
                panel.ResumeLayout(false);


                var ribbonControl = panel.Controls[0] as RibbonControl;

                ((System.ComponentModel.ISupportInitialize)(ribbonControl)).BeginInit();
                ribbonControl.AllowDrop = false;
                ribbonControl.ButtonGroupsVertAlign = DevExpress.Utils.VertAlignment.Top;
                ribbonControl.Dock = System.Windows.Forms.DockStyle.Fill;
                ribbonControl.Location = new System.Drawing.Point(0, 0);
                ribbonControl.PageCategoryAlignment = DevExpress.XtraBars.Ribbon.RibbonPageCategoryAlignment.Left;
                ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
                ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
                ribbonControl.Size = new System.Drawing.Size(1056, 62);

                ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
                if (addStatusBar)
                {
                    AddStatusBar(ribbonControl);
                }
                ((System.ComponentModel.ISupportInitialize)(ribbonControl)).EndInit();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public  static void AddStatusBar(DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl)
        {
            var ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            ribbonControl.StatusBar = ribbonStatusBar;
            ribbonStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            ribbonStatusBar.Location = new System.Drawing.Point(3, 552);
            ribbonStatusBar.Ribbon = ribbonControl;
            
            ribbonStatusBar.Size = new System.Drawing.Size(939, 27);

            var bsiStatusBarText  = new DevExpress.XtraBars.BarStaticItem();
            bsiStatusBarText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
            bsiStatusBarText.Appearance.Options.UseTextOptions = true;
            bsiStatusBarText.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
            bsiStatusBarText.ItemAppearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;

             
            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] { bsiStatusBarText });
            //bsiStatusBarText.Caption = "Status Bar Text....";
            bsiStatusBarText.TextAlignment = System.Drawing.StringAlignment.Near;
            ribbonStatusBar.ItemLinks.Add(bsiStatusBarText);

            //if (!Home.StatusBarForms.ContainsKey((XtraForm)ribbonControl.FindForm()))
            //    Home.StatusBarForms.Add((XtraForm)ribbonControl.FindForm(), bsiStatusBarText);

            ribbonControl.FindForm().Controls.Add(ribbonStatusBar);

        }
    }
}
