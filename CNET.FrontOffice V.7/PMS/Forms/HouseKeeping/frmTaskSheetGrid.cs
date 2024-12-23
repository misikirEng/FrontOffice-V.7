using CNET.ERP.Client.Common.UI;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using ProcessManager;
using CNET.FrontOffice_V._7.HouseKeeping;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.CodeParser;
using DevExpress.XtraPrinting.Export;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.CommonSchema;

namespace CNET.FrontOffice_V._7.HouseKeeping
{
    public partial class frmTaskSheetGrid : DevExpress.XtraEditors.XtraForm
    {
        static DateTime currentDate { get; set; }// = UIProcessManager.GetServiceTime().Value;

        static string time { get; set; }// = currentDate.Date.ToString("yyyy-MM-dd");
        DateTime t { get; set; }// = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        int numberOfEmployees = 0;
        private GridHitInfo downHitInfo = null;
        public string passedDate;
        List<List<AssignmentVM>> allG = new List<List<AssignmentVM>>() { };
        List<List<AssignmentVM>> copy = new List<List<AssignmentVM>>() { };
        int gcCount = 0;
        int i = 0;
        int j = 0;
        int gy = 0;
        int ly = 0;
        List<Control> prevcon = new List<Control>();
        List<GridControl> Fgr = new List<GridControl>();
        public GridView gridView;
        List<GridControl> gr = new List<GridControl>();

        List<GridControl> prevGrds = new List<GridControl>();

        DateTime CurrentTime { get; set; }

        public frmTaskSheetGrid(string date = null)
        {
            this.passedDate = date;
            InitializeComponent();
            if (passedDate != currentDate.ToShortDateString())
            {
                bbiSave.Enabled = false;
                bbiNew.Enabled = false;
            }


            try
            {
                DateTime? dat = UIProcessManager.GetServiceTime();
                if (dat != null)
                {
                    CurrentTime = dat.Value;
                }
                else
                {
                    XtraMessageBox.Show("Error Getting Service !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                currentDate = dat.Value;

                time = currentDate.Date.ToString("yyyy-MM-dd");
                t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int gx = 12;
            int gy = 0;
            int lx = 0;
            int ly = 0;
            int gpadding = 20;
            int lpadding = 24;
            int verticalCount = 1;
            int count = 0;

            int gheight = 200;
            int gwidth = 100;
            int lheight = 204;
            int lwidth = 116;

            foreach (Control c in layoutControl1.Controls)
            {
                string h = c.Name;
                if (h.Contains("grid"))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                gx += (((count % 7) - 1) * 116);
                lx += (((count % 7) - 1) * 116);
            }

            while (count < 35)
            {
                Point frGcontrol = new Point();
                Point frLcontrol = new Point();

                string gridControlName = "gridcontrol" + count;
                string layoutControlName = "layoutcontrol" + count;
                string gridViewName = "gridview" + count;

                GridControl gridControl = new GridControl();
                gridControl.Name = gridControlName;

                if (count == 0)
                {
                    frGcontrol = new Point(gx, gy);
                    frLcontrol = new Point(lx, ly);
                }
                else if (count % 7 == 0)
                {
                    int k = 0;
                    gx = 12;
                    lx = 0;
                    gy = gy + gheight + gpadding + verticalCount;
                    ly = ly + lheight + lpadding + verticalCount;

                    frGcontrol = new Point(gx, gy);
                    frLcontrol = new Point(lx, ly);

                    verticalCount++;

                }
                else
                {
                    gx = gx + 116;
                    lx = lx + 116;
                    frGcontrol = new Point(gx, gy);
                    frLcontrol = new Point(lx, ly);
                }

                GridView gridView = new GridView();
                LayoutControlItem layoutControlItem = new LayoutControlItem();

                ((ISupportInitialize)(gridControl)).BeginInit();
                ((ISupportInitialize)(gridView)).BeginInit();
                ((ISupportInitialize)(layoutControlItem)).BeginInit();
                layoutControl1.Controls.Add(gridControl);

                //gcBills
                gridControl.AllowDrop = true;
                gridControl.Location = frGcontrol;//new Point(244, 38);
                gridControl.MainView = gridView;
                gridControl.Name = gridControlName;
                gridControl.Size = new System.Drawing.Size(gwidth, gheight);
                //gcBills.DataSource = dataTable1.Clone();

                // gcBills.TabIndex = 
                gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });

                gridControl.DragDrop += new DragEventHandler(gridControl_DragDrop);
                gridControl.DragOver += new DragEventHandler(gridControl_DragOver);
                //GridView
                gridView.GridControl = gridControl;
                gridView.Name = gridViewName;
                gridView.OptionsBehavior.Editable = false;

                gridView.MouseDown += new MouseEventHandler(gridView_MouseDown);
                gridView.MouseMove += new MouseEventHandler(gridView_MouseMove);

                //layout contro;
                layoutControlItem.Control = gridControl;
                layoutControlItem.CustomizationFormText = "layoutControlItem1";
                layoutControlItem.Location = frLcontrol;//new Point(232, 26);
                layoutControlItem.Name = layoutControlName;
                layoutControlItem.Size = new Size(lwidth, lheight);
                layoutControlItem.Text = "layoutControlItem1";
                layoutControlItem.TextSize = new Size(0, 0);
                layoutControlItem.TextToControlDistance = 0;
                layoutControlItem.TextVisible = false;

                this.layoutControlGroup1.Items.AddRange(new BaseLayoutItem[] { layoutControlItem });

                ((ISupportInitialize)(gridControl)).EndInit();
                ((ISupportInitialize)(gridView)).EndInit();
                count++;

            }
            int scrollheight = verticalCount * lheight + lpadding + verticalCount + 50;
            xtraScrollableControl1.AutoScrollMinSize = new Size(100, scrollheight);
        }

        private void gridControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(GridHitInfo)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void gridControl_DragDrop(object sender, DragEventArgs e)
        {
            GridControl grid = sender as GridControl;
            GridView view = grid.MainView as GridView;

            List<AssignmentVM> k = grid.DataSource as List<AssignmentVM>;
            GridHitInfo srcHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
            GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));

            MoveRows(srcHitInfo, hitInfo, grid);
            CheckRefresh = 0;
            view.RefreshData();
        }

        private void MoveRows(GridHitInfo srcHitInfo, GridHitInfo hitInfo, GridControl g)
        {
            if (srcHitInfo == hitInfo) return;
            //GridView view = (GridView)g.FocusedView;
            GridView Srcview = srcHitInfo.View;
            AssignmentVM sourceRow = Srcview.GetRow(srcHitInfo.RowHandle) as AssignmentVM;


            GridView Destview = hitInfo.View;
            AssignmentVM targerRow = Destview.GetRow(hitInfo.RowHandle) as AssignmentVM;

            Srcview.BeginDataUpdate();
            bool isSrcConvertible = false;
            int viewNum = 0;
            try
            {
                int j = Convert.ToInt32(srcHitInfo.View.Name.Substring(srcHitInfo.View.Name.Length - 2));
                isSrcConvertible = true;
                viewNum = j;
            }
            catch (Exception e) { }
            if (!isSrcConvertible)
            {
                viewNum = Convert.ToInt32(srcHitInfo.View.Name.Substring(srcHitInfo.View.Name.Length - 1));
            }
            allG[viewNum - 1].Remove(sourceRow);
            Srcview.RefreshData();
            Srcview.EndDataUpdate();

            Destview.BeginDataUpdate();
            bool isConvertible = false;
            int destNum = 0;
            try
            {
                int j = Convert.ToInt32(hitInfo.View.Name.Substring(hitInfo.View.Name.Length - 2));
                isConvertible = true;
                destNum = j;
            }
            catch (Exception e) { }

            if (!isConvertible)
            {
                destNum = Convert.ToInt32(hitInfo.View.Name.Substring(hitInfo.View.Name.Length - 1));
            }
            allG[destNum - 1].Add(sourceRow);
            int kk = allG.Count;
            Destview.DataSourceChanged += Destview_DataSourceChanged;
            Destview.RefreshEditor(true);
            Destview.EndDataUpdate();

            Destview.RefreshData();
            g.RefreshDataSource();
        }

        private void Destview_DataSourceChanged(object sender, EventArgs e)
        {
            GridView View = sender as GridView;
            GridControl g = View.GridControl;
            View.RefreshData();
            g.RefreshDataSource();
        }

        private void gridView_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            downHitInfo = null;
            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.RowHandle != GridControl.NewItemRowHandle)
                downHitInfo = hitInfo;
        }

        private void gridView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {

                GridView view = sender as GridView;

                if (e.Button == MouseButtons.Left && downHitInfo != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    Rectangle dragRect = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2,
                        downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                    if (!dragRect.Contains(new Point(e.X, e.Y)))
                    {
                        view.GridControl.DoDragDrop(downHitInfo, DragDropEffects.Move);
                        downHitInfo = null;
                        view.RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.StackTrace, "Error");
            }
        }
        public int? GetActivityCode(int lookup)
        {
            ActivityDefinitionDTO listAD = LocalBuffer.LocalBuffer.ActivityDefinitionBufferList.FirstOrDefault(b => b.Description == lookup);

            if (listAD != null)
            {
                return listAD.Id;
            }
            else
            {
                return null;
            }
        }
        private void frmTaskSheetGrid_Load(object sender, EventArgs e)
        {
            //CNETInfoReporter.WaitForm("Populating Task Sheets ...", "Please Wait...");
            List<HkassignmentDTO> result = new List<HkassignmentDTO>();
            if (passedDate == null)
            {
                result = UIProcessManager.GetHKAssignmentByConsigneeunit(SelectedHotelcode.Value).Where(b => b.Date == t).ToList();
            }
            else if (passedDate != null)
            {
                DateTime time = Convert.ToDateTime(passedDate);
                result = UIProcessManager.GetHKAssignmentByConsigneeunit(SelectedHotelcode.Value).Where(b => b.Date == time).ToList();
            }
            List<int> lst = new List<int>();
            foreach (HkassignmentDTO hka in result)
            {
                if (lst.Contains(hka.Employee.Value)) { continue; }
                lst.Add(hka.Employee.Value);
            }

            List<AssignmentVM> gridResults = new List<AssignmentVM>();
            List<AssignmentVM> allgrids = new List<AssignmentVM>();
            int counter = 0;

            foreach (int emp in lst)
            {
                gridResults = new List<AssignmentVM>();
                List<int> roomcode = new List<int>();
                foreach (HkassignmentDTO signment in result.Where(b => b.Employee == emp))
                {
                    if (!roomcode.Contains(signment.RoomDetail.Value)) roomcode.Add(signment.RoomDetail.Value);
                    else if (roomcode.Contains(signment.RoomDetail.Value)) continue;
                    AssignmentVM adto = new AssignmentVM();
                    adto.roomcode = signment.RoomDetail;
                    adto.date = signment.Date.ToString();
                    decimal d = Math.Round(Convert.ToDecimal(signment.Credit), 2);
                    adto.Credit = d.ToString();
                    adto.empcode = signment.Employee;
                    ConsigneeDTO name = UIProcessManager.GetConsigneeById(adto.empcode.Value);
                    string employeeName = "";
                    if (name != null)
                    {
                        string firstName = "";
                        string middleName = "";
                        string lastname = "";

                        if (name.FirstName != null)
                            firstName = name.FirstName;
                        if (name.SecondName != null)
                            lastname = name.SecondName;
                        if (name.ThirdName != null)
                            middleName = name.ThirdName;


                        employeeName = firstName + " " + middleName + " " + lastname;
                    }
                    else
                    {
                        employeeName = "";
                    }
                    adto.empname = employeeName;
                    string rmno = UIProcessManager.GetRoomDetailById(adto.roomcode.Value).Description;
                    adto.Room = rmno;
                    List<VwRoomManagmentViewDTO> r = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).Where(b => b.roomDetailCode == signment.RoomDetail).ToList();
                    if (r != null && r.Count != 0)
                    {
                        adto.HkStatus = r.FirstOrDefault().rmstatus;
                        adto.color = r.FirstOrDefault().color;
                    }
                    else if (r == null || r.Count == 0)
                    {
                        //  adto.HkStatus = GetActivityCode(CNETConstantes.CLEAN).Value;
                        adto.color = "yellow";
                    }

                    RegistrationStatusDTO status = UIProcessManager.GetRegistrationStatus(signment.RoomDetail.Value, Convert.ToDateTime(time));
                    if (status.FOStatus == "0")
                    {
                        adto.FoStatus = "VAC";
                    }
                    else if (status.FOStatus == "1")
                    {
                        adto.FoStatus = "OCC";
                    }

                    gridResults.Add(adto);
                    allgrids.Add(adto);
                }

                allG.Add(gridResults);
                string empName_code = allG[counter][0].empname + "_" + allG[counter][0].empcode;
                createGrids(allG[counter], empName_code);
                numberOfEmployees++;
                counter++;
            }
            copy = allG.Select(x => x.ToList()).ToList();
            //CNETInfoReporter.Hide();
        }

        int verticalCount = 1;
        internal void createGrids(List<AssignmentVM> source, string tag = "")
        {
            gcCount++;

            int gx = 12;
            int lx = 0;
            int gpadding = 20;
            int lpadding = 24;

            int count = 0;

            int gheight = 260;
            int gwidth = 156;
            int lheight = 265;
            int lwidth = 177;

            foreach (Control c in layoutControl1.Controls)
            {
                string h = c.Name;
                if (h.Contains("grid"))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                gx += (((count % 5) - 1) * 177);
                lx += (((count % 5) - 1) * 177);
            }

            Point frGcontrol = new Point();
            Point frLcontrol = new Point();

            string gridControlName = "gridcontrol" + gcCount;
            string layoutControlName = "layoutcontrol" + gcCount;
            string gridViewName = "gridview" + gcCount;
            string labelName = "hkname" + gcCount;

            if (count == 0)
            {
                frGcontrol = new Point(gx, gy);
                frLcontrol = new Point(lx, ly);
            }
            else if (count % 5 == 0)
            {
                int k = 0;
                gx = 12;
                lx = 0;
                gy = gy + gheight + gpadding + verticalCount;
                ly = ly + lheight + lpadding + verticalCount;

                frGcontrol = new Point(gx, gy);
                frLcontrol = new Point(lx, ly);

                verticalCount++;
            }
            else
            {
                gx = gx + 177;
                lx = lx + 177;
                frGcontrol = new Point(gx, gy);
                frLcontrol = new Point(lx, ly);
            }
            GridControl gridControl = new GridControl();
            gridView = new GridView();
            LabelControl nameLabel = new LabelControl();

            LayoutControlItem layoutControlItem = new LayoutControlItem();


            ((ISupportInitialize)(gridControl)).BeginInit();
            ((ISupportInitialize)(gridView)).BeginInit();
            ((ISupportInitialize)(layoutControlItem)).BeginInit();

            layoutControl1.Controls.Add(gridControl);
            layoutControl1.Controls.Add(nameLabel);

            createColumns(gridView);
            gridView.OptionsView.ShowFooter = true;
            gridView.OptionsView.ShowIndicator = false;

            var item = new GridColumnSummaryItem
                (DevExpress.Data.SummaryItemType.Sum, "Credit", "Total:{0}");
            var item2 = new GridColumnSummaryItem
                (DevExpress.Data.SummaryItemType.Count, "", "{0}-Rms");

            gridView.Columns["Credit"].Summary.Add(item);
            gridView.Columns["Room"].Summary.Add(item2);

            gridView.Columns["Room"].Width = 50;
            gridView.Columns["empname"].Width = 70;

            nameLabel.Name = labelName;
            if (source.Count == 0)
            {
                // nameLabel.Text = tag.Substring(tag.IndexOf(' ') + 1);
                nameLabel.Text = tag.Split(' ')[1];
            }
            else
            {
                nameLabel.Text = source[0].empname;
            }
            nameLabel.Location = new Point(frGcontrol.X, frGcontrol.Y);
            nameLabel.Size = new System.Drawing.Size(150, 15);
            nameLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            nameLabel.BringToFront();

            gridView.OptionsView.ShowGroupPanel = false;
            //gcBills
            gridControl.AllowDrop = true;
            gridControl.Location = new Point(frGcontrol.X, frGcontrol.Y + 15);
            gridControl.MainView = gridView;
            gridControl.Name = gridControlName;
            gridControl.Size = new System.Drawing.Size(gwidth, gheight);
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            if (tag != "")
            {
                gridControl.Tag = tag;
            }

            gridView.RowCellStyle += SetColorCode;
            gridView.CustomDrawCell += gridView_CustomDrawCell;
            gridControl.DragDrop += new DragEventHandler(gridControl_DragDrop);
            gridControl.DragOver += new DragEventHandler(gridControl_DragOver);

            //Gridview
            gridView.GridControl = gridControl;
            gridView.Name = gridViewName;
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsBehavior.ReadOnly = false;

            //gvBills.OptionsView.EnableAppearanceEvenRow = true;
            //gvBills.OptionsView.EnableAppearanceOddRow = true;

            gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            //gvBills.OptionsSelection.EnableAppearanceHideSelection = false;

            gridView.MouseDown += new MouseEventHandler(gridView_MouseDown);

            gridView.MouseMove += new MouseEventHandler(gridView_MouseMove);

            //layout contro;
            layoutControlItem.Control = gridControl;
            layoutControlItem.CustomizationFormText = "layoutControlItem1";
            layoutControlItem.Location = frLcontrol;//new Point(232, 26);
            layoutControlItem.Name = layoutControlName;
            layoutControlItem.Size = new Size(lwidth, lheight);
            layoutControlItem.Text = "layoutControlItem1";
            layoutControlItem.TextSize = new Size(0, 0);
            layoutControlItem.TextToControlDistance = 0;
            layoutControlItem.TextVisible = false;
            layoutControlItem.Control = nameLabel;
            layoutControlItem.CustomizationFormText = "layoutControlItem1";
            layoutControlItem.Location = frLcontrol;//new Point(232, 26);
            layoutControlItem.Name = layoutControlName;
            layoutControlItem.Size = new Size(lwidth, lheight);
            layoutControlItem.Text = "layoutControlItem1";
            layoutControlItem.TextSize = new Size(0, 0);
            layoutControlItem.TextToControlDistance = 0;
            layoutControlItem.TextVisible = false;

            layoutControlGroup1.Items.AddRange(new BaseLayoutItem[] { layoutControlItem });
            layoutControl1.ResumeLayout(false);

            ((ISupportInitialize)(gridControl)).EndInit();
            ((ISupportInitialize)(gridView)).EndInit();

            gridControl.ForceInitialize();

            gridControl.BeginUpdate();
            try
            {
                gridControl.DataSource = null;
                gridControl.DataSource = source;
                gridControl.RefreshDataSource();
            }
            finally
            {
                gridControl.EndUpdate();
            }

            int scrollheight = verticalCount * ly + lpadding + verticalCount + 50;
            xtraScrollableControl1.AutoScrollMinSize = new Size(100, scrollheight);
        }

        void gridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.Column.FieldName == "color")
                {
                    e.DisplayText = "";
                    AssignmentVM dto = View.GetRow(e.RowHandle) as AssignmentVM;

                    if (dto != null)
                    {
                        e.Appearance.BackColor = ColorTranslator.FromHtml(dto.color);
                    }

                }


            }
            catch (Exception col)
            {

            }
        }

        private void createColumns(GridView gridView)
        {
            GridColumn col6 = new GridColumn();
            col6.FieldName = "color";
            col6.Caption = "";
            col6.Visible = true;
            col6.Width = 2;
            gridView.Columns.Add(col6);

            GridColumn col1 = new GridColumn();
            col1.FieldName = "roomcode";
            col1.Caption = "Rcode";
            col1.Visible = false;
            gridView.Columns.Add(col1);

            GridColumn col2 = new GridColumn();
            col2.FieldName = "Room";
            col2.Caption = "Room";
            col2.Visible = true;
            gridView.Columns.Add(col2);

            GridColumn col3 = new GridColumn();
            col3.FieldName = "empcode";
            col3.Caption = "Ecode";
            col3.Visible = false;
            gridView.Columns.Add(col3);

            GridColumn col4 = new GridColumn();
            col4.FieldName = "empname";
            col4.Caption = "Ename";
            col4.Width = 120;
            col4.Visible = false;
            gridView.Columns.Add(col4);

            GridColumn col5 = new GridColumn();
            col5.FieldName = "Credit";
            col5.Caption = "Credit";
            col5.Visible = true;
            gridView.Columns.Add(col5);



            GridColumn col7 = new GridColumn();
            col7.FieldName = "HkStatus";
            col7.Caption = "Color";
            col7.Visible = false;
            gridView.Columns.Add(col7);

            GridColumn col8 = new GridColumn();
            col8.FieldName = "FoStatus";
            col8.Visible = false;
            gridView.Columns.Add(col8);

            GridColumn col9 = new GridColumn();
            col9.FieldName = "date";
            col9.Visible = false;
            gridView.Columns.Add(col9);

        }

        private void AddEmployeeClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            for (int j = 1; j <= gcCount; j++)
            {
                string gname = "gridcontrol" + j;
                prevcon = layoutControl1.Controls.Find(gname, true).ToList();
                GridControl g = prevcon[0] as GridControl;
                prevGrds.Add(g);
            }
            frmAddEmployee addemp = new frmAddEmployee();
            addemp.SelectedHotelcode = SelectedHotelcode.Value;
            addemp.ShowDialog();
            int? empcode = addemp.empCode;
            if (empcode != null) return;
            List<AssignmentVM> emptySource = new List<AssignmentVM>();
            ConsigneeDTO per = UIProcessManager.GetConsigneeById(empcode.Value);
            string fname = per.FirstName;
            string lname = per.ThirdName;

            string fullName = empcode + " " + fname + " " + lname;

            allG.Insert(allG.Count, emptySource);
            copy.Clear();
            copy = allG.Select(x => x.ToList()).ToList();
            createGrids(emptySource, fullName);

        }

        private void PrintSelected(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<Control> k = new List<Control>();

            for (int j = 1; j <= gcCount; j++)
            {
                string gname = "gridcontrol" + j;
                k = layoutControl1.Controls.Find(gname, true).ToList();
                GridControl g = k[0] as GridControl;
                if (g.ContainsFocus)
                {
                    Fgr.Add(g);
                }

            }
            if (Fgr.Count <= 0)
            {
                XtraMessageBox.Show("Please Select A Task To Be Printed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            List<AssignmentVM> lst = Fgr[0].DataSource as List<AssignmentVM>;
            List<AssignmentVM> hold = new List<AssignmentVM>();

            foreach (AssignmentVM a in lst)
            {
                try
                {
                    if (Fgr[0].Tag != null && Fgr[0].Tag != "")
                    {
                        List<string> split = Fgr[0].Tag.ToString().Split('_').ToList();
                        a.empcode = Convert.ToInt32(split[0]);
                        a.empname = split[0] + " " + split[1];
                        hold.Add(a);
                    }
                    else
                    {
                        a.empcode = null;
                        a.empname = "";
                        hold.Add(a);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            try
            {
                List<PrintAllAssignmentVM> printdto = new List<PrintAllAssignmentVM>();
                List<PrintSingleAssignmentVM> singlePrint = new List<PrintSingleAssignmentVM>();
                printdto = hold.Select(b => new PrintAllAssignmentVM { Room = b.Room, FoStatus = b.FoStatus, HkStatus = b.HkStatus, EmpName = b.empname, /*EmpCode = b.empcode,*/ Credit = b.Credit }).ToList();
                string date = "";
                foreach (AssignmentVM paad in hold)
                {
                    date = paad.date;//.Substring(0,10);
                    PrintSingleAssignmentVM psad = new PrintSingleAssignmentVM();
                    psad.Room = paad.Room;
                    psad.HkStatus = paad.HkStatus;
                    psad.FoStatus = paad.FoStatus;
                    string resstatus = UIProcessManager.GetRegistrationStatus(paad.roomcode.Value, t).registrationStatus;
                    psad.ResStatus = resstatus;
                    psad.Section = "";
                    psad.TurnDown = "";
                    List<VwRegistrationDocumentViewDTO> whatevs = UIProcessManager.GetRegistrationDocumentViewByDate(Convert.ToDateTime(paad.date));

                    if (whatevs != null && whatevs.Count > 0)
                        whatevs = whatevs.Where(b => b.Room == paad.roomcode).ToList();

                    string fullName = "";
                    if (whatevs.Count > 0)
                    {
                        ConsigneeDTO Guest = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(b => b.Id == whatevs[0].GuestId);

                        fullName = Guest.FirstName + " " + Guest.SecondName + " " + Guest.ThirdName;
                        psad.Name = fullName;
                        psad.DepartureDate = whatevs[0].EndDate.ToString();
                        psad.ArrivalDate = whatevs[0].StartDate.ToString();
                        int adult = 0;
                        int child = 0;
                        foreach (VwRegistrationDocumentViewDTO vrd in whatevs)
                        {
                            adult += Convert.ToInt32(vrd.Adult);
                            child += Convert.ToInt32(vrd.Child);
                        }
                        psad.Adult = adult.ToString();
                        psad.Child = child.ToString();
                    }
                    else
                    {
                        psad.Name = "";
                        psad.DepartureDate = "";
                        psad.ArrivalDate = "";
                        psad.Adult = "";
                        psad.Child = "";
                    }

                    psad.Credit = paad.Credit;
                    singlePrint.Add(psad);
                }

                GridControl grr = new GridControl();
                grr.ForceInitialize();
                GridView gv = new GridView();
                grr.ForceInitialize();

                grr.DataSource = singlePrint;
                gv.GridControl = grr;


                decimal ttlCredit = 0;
                if (singlePrint != null || singlePrint.Count != 0)
                {
                    foreach (PrintSingleAssignmentVM psad in singlePrint)
                    {
                        ttlCredit += Convert.ToDecimal(psad.Credit);
                    }
                }

                TaskAssignmentReport report = new TaskAssignmentReport();
                report = new TaskAssignmentReport(grr, hold[0].empname, date, singlePrint.Count, ttlCredit);
                report.Landscape = true;
                ReportPrintTool pt = new ReportPrintTool(report);
                pt.ShowPreview();
                Fgr.Clear();
            }
            catch (Exception exx)
            {

            }
        }

        private void createColumnForprint(GridView con)
        {
            GridColumn col1 = new GridColumn();
            col1.FieldName = "Room";
            col1.Caption = "HK Status";
            col1.Visible = true;
            con.Columns.Add(col1);

            GridColumn col2 = new GridColumn();
            col1.FieldName = "HkStatus";
            col1.Caption = "House Keeping";
            col1.Visible = true;
            con.Columns.Add(col2);

            GridColumn col3 = new GridColumn();
            col3.FieldName = "FoStatus";
            col3.Caption = "Fo Status";
            col3.Visible = true;
            con.Columns.Add(col3);

            GridColumn col4 = new GridColumn();
            col3.FieldName = "Section";
            col3.Caption = "Section";
            col3.Visible = true;
            con.Columns.Add(col4);

            GridColumn col5 = new GridColumn();
            col5.FieldName = "ResStatus";
            col5.Caption = "Res Status";
            col5.Visible = true;

            con.Columns.Add(col5);

            GridColumn col6 = new GridColumn();
            col6.FieldName = "TurnDown";
            col6.Caption = "Turn Down";
            col6.Visible = true;
            con.Columns.Add(col6);

            GridColumn col7 = new GridColumn();
            col7.FieldName = "Name";
            col7.Caption = "Guest Name";
            col7.Visible = true;
            con.Columns.Add(col7);

            GridColumn col8 = new GridColumn();
            col8.FieldName = "ArrivalDate";
            col8.Caption = "Arrival Date";
            col8.Visible = true;
            con.Columns.Add(col8);

            GridColumn col9 = new GridColumn();
            col9.FieldName = "DepartureDate";
            col9.Caption = "Departure Date";
            col9.Visible = true;
            con.Columns.Add(col9);

            GridColumn col10 = new GridColumn();
            col10.FieldName = "Adult";
            col10.Caption = "Adults";
            col10.Visible = true;
            con.Columns.Add(col10);

            GridColumn col11 = new GridColumn();
            col11.FieldName = "Child";
            col11.Caption = "Childs";
            col11.Visible = true;
            con.Columns.Add(col11);

            GridColumn col12 = new GridColumn();
            col12.FieldName = "Credit";
            col12.Caption = "Credit";
            col12.Visible = true;
            con.Columns.Add(col12);
        }

        private void PrintAll(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridControl printGrid = new GridControl();
            GridView g = new GridView();
            g.GridControl = printGrid;

            createColumnForprint(g);

            List<AssignmentVM> lst = new List<AssignmentVM>();
            lst = allG.SelectMany(b => b).ToList();
            int totalRooms = lst.Count;
            decimal totalCredit = 0;
            foreach (List<AssignmentVM> asd in allG)
            {
                foreach (AssignmentVM ad in asd)
                {
                    totalCredit += Convert.ToDecimal(ad.Credit);
                }
            }
            List<PrintAllAssignmentVM> printdto = new List<PrintAllAssignmentVM>();
            printdto = lst.Select(b => new PrintAllAssignmentVM { Room = b.Room, FoStatus = b.FoStatus, HkStatus = b.HkStatus, EmpName = b.empname, /*EmpCode = b.empcode,*/ Credit = b.Credit }).ToList();
            printGrid.DataSource = printdto;

            HouseKeepingReport report = new HouseKeepingReport();
            report = new HouseKeepingReport(printGrid, true, totalRooms.ToString(), totalCredit.ToString(), "Task Sheet Report");
            report.Landscape = true;
            ReportPrintTool pt = new ReportPrintTool(report);
            pt.ShowPreview();

        }

        private void SaveNewTask(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gr.Clear();
            //CNETInfoReporter.WaitForm("Saving Tasks...", "Please Wait...");
            List<Control> k = new List<Control>();
            List<GridControl> newGrid = new List<GridControl>();
            for (int j = 1; j <= gcCount; j++)
            {
                string gname = "gridcontrol" + j;
                k = layoutControl1.Controls.Find(gname, true).ToList();
                GridControl g = k[0] as GridControl;
                gr.Add(g);
            }
            foreach (GridControl gk in gr)
            {
                if (prevGrds.Contains(gk))
                {
                    continue;
                }
                else
                {
                    prevGrds.Add(gk);
                    //newGrid.Add(gk);
                }
            }
            //if (newGrid.Count == 0) { return; }
            try
            {
                foreach (GridControl aa in prevGrds)
                {
                    List<AssignmentVM> assign = aa.DataSource as List<AssignmentVM>;
                    foreach (AssignmentVM a in assign)
                    {
                        HkassignmentDTO hk = new HkassignmentDTO();
                        string[] name = aa.Tag.ToString().Split('_');
                        int l = 0;
                        if (name != null && name.Length == 1)
                        {
                            l = Convert.ToInt32(aa.Tag.ToString().Split(' ')[0]);
                        }
                        else
                        {
                            l = Convert.ToInt32(name[1].Trim());
                        }
                        hk.Employee = l;// person.Where(b => b.firstName == name[0] && (b.middleName == name[1] || b.lastName == name[1])).FirstOrDefault().code;
                        hk.Credit = Convert.ToDecimal(a.Credit);
                        hk.Date = Convert.ToDateTime(DateTime.Now);
                        hk.Consigneeunit = SelectedHotelcode;
                        hk.RoomDetail = a.roomcode;
                        HkassignmentDTO h = UIProcessManager.SelectAllHKAssignment().Where(b => b.RoomDetail == hk.RoomDetail & Convert.ToDateTime(b.Date).ToShortDateString() == Convert.ToDateTime(hk.Date).ToShortDateString() && b.Consigneeunit == SelectedHotelcode).FirstOrDefault();
                        bool result = UIProcessManager.DeleteHKAssignmentById(h.Id);
                        HkassignmentDTO createHK = UIProcessManager.CreateHKAssignment(hk);

                    }
                }
                XtraMessageBox.Show("Task Saved Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                logActiviy();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Task Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //CNETInfoReporter.Hide();
        }
        private void logActiviy()
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = GetActivityCode(CNETConstantes.HK_MAINTAINED).Value;
                act.TimeStamp = CurrentTime.ToLocalTime();
                act.Year = CurrentTime.Year;
                act.Month = CurrentTime.Month;
                act.Day = CurrentTime.Day;
                act.Reference = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.Pointer = CNETConstantes.HouseKeeping_Mgt;
                act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                act.Platform = "1";
                act.ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                act.Remark = "HK TASK SHEET SAVED/MODIFIED";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }
        private Color StatusColor(int objectState)
        {
            VwRoomManagmentViewDTO mgmt = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).Where(r => r.roomStatusCode == objectState).First();
            if (mgmt == null)
            {
                return Color.Transparent;
            }
            else
            {

                if (mgmt.color.Contains(','))
                {
                    string[] colorValues = mgmt.color.Split(',');
                    int r = Convert.ToInt32(colorValues[0]);
                    int g = Convert.ToInt32(colorValues[1]);
                    int b = Convert.ToInt32(colorValues[2]);
                    Color color = Color.FromArgb(r, g, b);
                    return color;
                }
                else
                {
                    Color color = Color.FromName(mgmt.color);

                    return color;
                }

            }
        }

        int CheckRefresh = 0;
        private void SetColorCode(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {

            }
            catch (Exception col)
            {

            }
        }

        private void GetValuesForSinglePrint()
        {

        }


        public int? SelectedHotelcode { get; set; }
    }
}