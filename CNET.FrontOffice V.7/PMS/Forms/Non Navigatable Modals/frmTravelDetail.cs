using System;
using System.Drawing;
using System.Linq;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.ERP.Client.Common.UI.Library;
using System.Windows.Forms;

using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
using CNET.FrontOffice_V._7;
using CNET.ERP.ResourceProvider;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ViewSchema;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmTravelDetail : XtraForm
    {
        //Fields

        public RegistrationListVMDTO RegExtension { get; set; }

        public List<TravelDetailDTO> AddedTravelDetails { get; set; }
        public bool IsFromReservation { get; set; }

        private List<VwRouteDetailDTO> _routeDetailList = new List<VwRouteDetailDTO>();

        private TravelDetailDTO _tdArrival = null;
        private TravelDetailDTO _tdDeparture = null;


        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }


        //************  CONSTRUCTOR ***********************/
        public frmTravelDetail()
        {
            InitializeComponent();

            InitializeUI();
        }


        #region Helper Methods


        private void InitializeUI()
        {

            //Route
            GridColumn columnGuest = sluRoute.Properties.View.Columns.AddField("Id");
            columnGuest.Visible = false;
            columnGuest = sluRoute.Properties.View.Columns.AddField("FlightCode");
            columnGuest.Visible = true;
            columnGuest = sluRoute.Properties.View.Columns.AddField("Carrier");
            columnGuest.Visible = true;
            columnGuest = sluRoute.Properties.View.Columns.AddField("TransportType");
            columnGuest.Visible = true;
            columnGuest = sluRoute.Properties.View.Columns.AddField("From");
            columnGuest.Visible = true;
            columnGuest = sluRoute.Properties.View.Columns.AddField("To");
            columnGuest.Visible = true;
            sluRoute.Properties.DisplayMember = "FlightCode";
            sluRoute.Properties.ValueMember = "Id";

            // Required Action
            lukReqAction.Properties.Columns.Add(new LookUpColumnInfo("Description", "Action"));
            lukReqAction.Properties.DisplayMember = "Description";
            lukReqAction.Properties.ValueMember = "Id";
        }

        private bool InitializeData()
        {
            try
            {
                // Progress_Reporter.Show_Progress("Initializing form data...", "Please Wait...");

                AddedTravelDetails = new List<TravelDetailDTO>();

                List<LookupDTO> actionRequiredList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.IsActive && l.Type == CNETConstantes.ACTION_REQUIRED).ToList();


                //Guest Info
                if (RegExtension != null)
                {
                    teGuestName.Text = RegExtension.Guest;
                    teRegNo.Text = RegExtension.Registration;

                    deTravelDetail.EditValue = RegExtension.Arrival;
                }

                //Station Code
                if (actionRequiredList != null)
                {
                    lukReqAction.Properties.DataSource = actionRequiredList;
                    LookupDTO station = actionRequiredList.FirstOrDefault(c => c.IsDefault);
                    if (station != null)
                    {
                        lukReqAction.EditValue = station.Id;
                    }
                }


                //Populate Route Detail
                PopulateRouteDetail();



                if (!IsFromReservation)
                {

                    List<TravelDetailDTO> tDetailList = UIProcessManager.GetTravelDetailByvoucher(RegExtension.Id);

                    //added travel detail 
                    if (tDetailList != null && tDetailList.Count > 0)
                    {
                        _tdArrival = tDetailList.FirstOrDefault(td => td.Type == CNETConstantes.TravelDetail_Arrival);
                        _tdDeparture = tDetailList.FirstOrDefault(td => td.Type == CNETConstantes.TravelDetail_Departure);

                        //Populate Arrival Travel Detail
                        PopulateTravelDetails(_tdArrival);

                    }
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing travel detail form. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void PopulateRouteDetail()
        {
            _routeDetailList = UIProcessManager.GetAllRouteView();
            if (_routeDetailList == null)
            {
                _routeDetailList = new List<VwRouteDetailDTO>();
            }

            List<RouteVM> rDtoList = new List<RouteVM>();
            foreach (var route in _routeDetailList)
            {
                RouteVM dto = new RouteVM();
                dto.Carrier = route.CarrierDescription;
                dto.Id = route.Id;
                dto.FlightCode =  route.Code;
                dto.TransportType = route.TransportTypeDescription;
                dto.From = string.Format("{0} [{1}], {2}", route.FromStationDescription, route.CityName,
                    route.FromCountryName);
                dto.To = string.Format("{0} [{1}], {2}", route.ToStationDescription, route.ToCityName,
                    route.ToCountryName);

                rDtoList.Add(dto);
            }

            sluRoute.Properties.DataSource = rDtoList;
        }
        private void PopulateTravelDetails(TravelDetailDTO td)
        {
            if (td == null) return;
            sluRoute.EditValue = td.Route;
            lukReqAction.EditValue = td.ActionRequired;
            deTravelDetail.EditValue = td.Traveldate;
            teTime.EditValue = td.Traveldate.TimeOfDay;
        }


        #endregion


        #region Event Handlers

        private void tabTravelType_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == tpArrival)
            {
                if (RegExtension != null)
                {
                    deTravelDetail.EditValue = RegExtension.Arrival;
                }
                deTravelDetail.EditValue = RegExtension.Arrival;
                if (_tdArrival != null)
                {
                    PopulateTravelDetails(_tdArrival);
                }
                else
                {
                    sluRoute.EditValue = null;
                }

                lciTime.Text = "Pickup Time";
                lukReqAction.EditValue = CNETConstantes.TD_PICK_UP;

            }
            else if (e.Page == tpDeparture)
            {
                if (RegExtension != null)
                {
                    deTravelDetail.EditValue = RegExtension.Departure;
                }
                deTravelDetail.EditValue = RegExtension.Departure;
                if (_tdDeparture != null)
                {
                    PopulateTravelDetails(_tdDeparture);
                }
                else
                {
                    sluRoute.EditValue = null;
                }


                lciTime.Text = "Dropoff Time";
                lukReqAction.EditValue = CNETConstantes.TD_DROP_OFF;
            }
        }

        private void sluRoute_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit view = sender as SearchLookUpEdit;

            if (view.EditValue == null || string.IsNullOrEmpty(view.EditValue.ToString()))
            {
                teFlightCode.Text = "";
                teCarrier.Text = "";
                teFromCity.Text = "";
                teFromStation.Text = "";
                teDistance.Text = "";
                teTransportType.Text = "";
                teToCity.Text = "";
                teToStation.Text = "";
                teDuration.Text = "";
            }
            else
            {
                var routeDetail = _routeDetailList.FirstOrDefault(r => r.Id == Convert.ToInt32(view.EditValue));
                if (routeDetail == null) return;

                teFlightCode.Text = routeDetail.Code;
                teCarrier.Text = routeDetail.CarrierDescription;
                teFromCity.Text = string.Format("{0}, {1}", routeDetail.CityName, routeDetail.FromCountryName);
                teFromStation.Text = routeDetail.FromStationDescription;
                teDistance.Text = routeDetail.Distance.ToString();
                teTransportType.Text = routeDetail.TransportTypeDescription;
                teToCity.Text = string.Format("{0}, {1}", routeDetail.ToCityName, routeDetail.ToCountryName);
                teToStation.Text = routeDetail.ToStationDescription;
                teDuration.Text = routeDetail.Duration.ToString();

            }



        }


        #endregion

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                List<Control> controls = new List<Control>
                {
                    sluRoute,
                    lukReqAction,
                    deTravelDetail,
                    teTime
                };


                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    SystemMessage.ShowModalInfoMessage("Please fill all required fields!", "ERROR");
                    return;
                }

                // Progress_Reporter.Show_Progress("Saving Travel Detail", "Please Wait...");

                /************* Arrival Travel Detail ****************/
                if (tabTravelType.SelectedTabPage == tpArrival)
                {
                    TravelDetailDTO travelDetail = new TravelDetailDTO();
                    travelDetail.ActionRequired = Convert.ToInt32(lukReqAction.EditValue);
                    travelDetail.Route = Convert.ToInt32(sluRoute.EditValue);
                    travelDetail.Type = CNETConstantes.TravelDetail_Arrival;
                    DateTime pickTime = Convert.ToDateTime(teTime.EditValue.ToString());
                    DateTime pickDate = deTravelDetail.DateTime;
                    DateTime travelDate = new DateTime(pickDate.Year, pickDate.Month, pickDate.Day, pickTime.Hour, pickTime.Minute, pickTime.Second);
                    travelDetail.Traveldate = travelDate;

                    if (!IsFromReservation)
                    {

                        travelDetail.Voucher = RegExtension.Id;

                        if (_tdArrival != null)
                        {
                            travelDetail.Id = _tdArrival.Id;
                            if (UIProcessManager.UpdateTravelDetail(travelDetail) != null)
                            {
                                SystemMessage.ShowModalInfoMessage("Edited Successfully!!!", "MESSAGE");
                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("Editing is not successful!!!", "ERROR");
                            }
                        }
                        else
                        {
                            TravelDetailDTO ExistTravelDetail = UIProcessManager.CreateTravelDetail(travelDetail);

                            if (ExistTravelDetail != null && ExistTravelDetail.Id > 0)
                            {
                                SystemMessage.ShowModalInfoMessage("Saved Successfully!!!", "MESSAGE");
                                _tdArrival = UIProcessManager.GetTravelDetailById(ExistTravelDetail.Id);
                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("Not Saved!!!", "ERROR");
                            }
                        }



                    }//end of if(!isFromReservation)
                    else
                    {

                        var td = AddedTravelDetails.FirstOrDefault(t => t.Type == CNETConstantes.TravelDetail_Arrival);
                        if (td == null)
                            AddedTravelDetails.Add(travelDetail);
                        SystemMessage.ShowModalInfoMessage("Arrival Travel Detail for the current registration is stored!", "MESSAGE");
                    }


                }
                /************* Departure Travel Detail ****************/
                else
                {
                    TravelDetailDTO travelDetail = new TravelDetailDTO();
                    travelDetail.ActionRequired = Convert.ToInt32(lukReqAction.EditValue);
                    travelDetail.Route = Convert.ToInt32(sluRoute.EditValue);
                    travelDetail.Type = CNETConstantes.TravelDetail_Departure;
                    DateTime pickTime = Convert.ToDateTime(teTime.EditValue.ToString());
                    DateTime pickDate = deTravelDetail.DateTime;
                    DateTime travelDate = new DateTime(pickDate.Year, pickDate.Month, pickDate.Day, pickTime.Hour, pickTime.Minute, pickTime.Second);
                    travelDetail.Traveldate = travelDate;

                    if (!IsFromReservation)
                    {

                        travelDetail.Voucher = RegExtension.Id;

                        if (_tdDeparture != null)
                        {
                            travelDetail.Id = _tdDeparture.Id;
                            if (UIProcessManager.UpdateTravelDetail(travelDetail) != null)
                            {
                                SystemMessage.ShowModalInfoMessage("Edited Successfully!!!", "MESSAGE");
                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("Editing is not successful!!!", "ERROR");
                            }
                        }
                        else
                        {
                            TravelDetailDTO ExistTravelDetail = UIProcessManager.CreateTravelDetail(travelDetail);

                            if (ExistTravelDetail != null)
                            {
                                SystemMessage.ShowModalInfoMessage("Saved Successfully!!!", "MESSAGE");
                                _tdDeparture = UIProcessManager.GetTravelDetailById(ExistTravelDetail.Id);
                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("Not Saved!!!", "ERROR");
                            }
                        }



                    }//end of if(!isFromReservation)
                    else
                    {

                        var td = AddedTravelDetails.FirstOrDefault(t => t.Type == CNETConstantes.TravelDetail_Departure);
                        if (td == null)
                            AddedTravelDetails.Add(travelDetail);
                        SystemMessage.ShowModalInfoMessage("Departure Travel Detail for the current registration is stored!", "MESSAGE");
                    }


                }

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving travel detail. Detail:: " + ex.Message, "ERROR");

            }
        }

        private void frmTravelDetail_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiAddRoute_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmRouteEdit frmRoute = new frmRouteEdit();
            if (frmRoute.ShowDialog() == DialogResult.OK)
            {
                PopulateRouteDetail();
            }
        }
    }
}
