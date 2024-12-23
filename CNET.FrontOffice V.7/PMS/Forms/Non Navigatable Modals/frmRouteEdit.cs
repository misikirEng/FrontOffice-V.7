
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Domain.ViewSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRouteEdit : XtraForm
    {
        private List<RouteVM> _routeDtoList = new List<RouteVM>();
        private int? _defCarrier = null;
        private int? _defTransType = null;
        private int? _defStation = null;
        private List<VwRouteDetailDTO> _routeDetailList = null;


        /*************  CONSTRUCTOR *********************/
        public frmRouteEdit()
        {
            InitializeComponent();

            InitializeUI();
        }

        #region Helper Methods

        private void InitializeUI()
        {

            // State
            GridColumn colState = sluState.Properties.View.Columns.AddField("Id");
            colState.Visible = true;
            colState = sluState.Properties.View.Columns.AddField("Description");
            colState.Visible = true;
            colState.Caption = "State";
            sluState.Properties.DisplayMember = "Description";
            sluState.Properties.ValueMember = "Id";

            // Carrier

            searchLookUpEdit1.Properties.DisplayMember = "Description";
            searchLookUpEdit1.Properties.ValueMember = "Id";

            //lukCarrier.Properties.DisplayMember = "Description";
            //lukCarrier.Properties.ValueMember = "Id";

            // Transportation Type 
            lukTransportType.Properties.DisplayMember = "Description";
            lukTransportType.Properties.ValueMember = "Id";


            //From Station
            GridColumn colFromStation = sluFromStation.Properties.View.Columns.AddField("Id");
            colFromStation.Visible = true;
            colFromStation = sluFromStation.Properties.View.Columns.AddField("Description");
            colFromStation.Caption = "Station Name";
            colFromStation.Visible = true;
            sluFromStation.Properties.DisplayMember = "Description";
            sluFromStation.Properties.ValueMember = "Id";

            //To Station
            GridColumn colToStation = sluToStation.Properties.View.Columns.AddField("Id");
            colToStation.Visible = true;
            colToStation = sluToStation.Properties.View.Columns.AddField("Description");
            colToStation.Caption = "Station Name";
            colToStation.Visible = true;
            sluToStation.Properties.DisplayMember = "Description";
            sluToStation.Properties.ValueMember = "Id";

            //From City
            GridColumn colCity = sluFromCity.Properties.View.Columns.AddField("CityCode");
            colCity.Visible = false;
            colCity = sluFromCity.Properties.View.Columns.AddField("Name");
            colCity.Caption = "Country Name";
            colCity.Visible = true;
            colCity = sluFromCity.Properties.View.Columns.AddField("CityName");
            colCity.Caption = "City Name";
            colCity.Visible = true;
            colCity = sluFromCity.Properties.View.Columns.AddField("continent");
            colCity.Visible = false;
            sluFromCity.Properties.DisplayMember = "CityName";
            sluFromCity.Properties.ValueMember = "CityCode";

            //To City
            GridColumn colToCity = sluToCity.Properties.View.Columns.AddField("City");
            colToCity.Visible = false;
            colToCity = sluToCity.Properties.View.Columns.AddField("Name");
            colToCity.Caption = "Country Name";
            colToCity.Visible = true;
            colToCity = sluToCity.Properties.View.Columns.AddField("CityName");
            colToCity.Caption = "City Name";
            colToCity.Visible = true;
            colToCity = sluToCity.Properties.View.Columns.AddField("continent");
            colToCity.Visible = false;
            sluToCity.Properties.DisplayMember = "CityName";
            sluToCity.Properties.ValueMember = "CityCode";


            //Route
            GridColumn colRoute = repoSluRoute.View.Columns.AddField("Id");
            colRoute.Visible = false;

            colRoute = repoSluRoute.View.Columns.AddField("FlightCode");
            colRoute.Visible = true;
            colRoute.Width = 150;
            colRoute = repoSluRoute.View.Columns.AddField("Carrier");
            colRoute.Visible = true;
            colRoute.Width = 150;
            colRoute = repoSluRoute.View.Columns.AddField("TransportType");
            colRoute.Visible = true;
            colRoute.Width = 150;
            colRoute = repoSluRoute.View.Columns.AddField("From");
            colRoute.Visible = true;
            colRoute.Width = 350;
            colRoute = repoSluRoute.View.Columns.AddField("To");
            colRoute.Visible = true;
            colRoute.Width = 350;
            repoSluRoute.DisplayMember = "FlightCode";
            repoSluRoute.ValueMember = "Id";

        }

        private bool InitializeData()
        {
            try
            {

                // Progress_Reporter.Show_Progress("Loading Data", "Please Wait...");

                //From and To City/Country Detail
                GetAndPopulateSubCity();

                //From and To Station Detail
                List<LookupDTO> stationList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.STATION).ToList();
                if (stationList == null)
                {
                    SystemMessage.ShowModalInfoMessage("Station data is not found!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                sluFromStation.Properties.DataSource = stationList;
                sluToStation.Properties.DataSource = stationList;
                var DefStation = stationList.FirstOrDefault(s => s.IsDefault);
                if (DefStation != null)
                {
                    _defStation = DefStation.Id;
                    sluFromStation.EditValue = _defStation;
                    sluToStation.EditValue = _defStation;
                }


                //Populate Route
                PopulateRoute();

                //Object State
                List<SystemConstantDTO> osdList = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList;
                if (osdList != null)
                {
                    sluState.Properties.DataSource = osdList;
                }

                //Carrier
                List<LookupDTO> carrierList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.CARRIER).ToList();
                if (carrierList != null)
                {
                    //lukCarrier.Properties.DataSource = carrierList;
                    searchLookUpEdit1.Properties.DataSource = carrierList;
                    var defCarrier = carrierList.FirstOrDefault(c => c.IsDefault);
                    if (defCarrier != null)
                    {
                        _defCarrier = defCarrier.Id;
                        //lukCarrier.EditValue = _defCarrier;
                        searchLookUpEdit1.EditValue = _defCarrier;
                    }
                }

                //Transportation Type
                List<LookupDTO> transTypeList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.TRANSPORTATION_TYPE).ToList();
                if (transTypeList != null)
                {
                    lukTransportType.Properties.DataSource = transTypeList;
                    var defTrans = transTypeList.FirstOrDefault(c => c.IsDefault);
                    if (defTrans != null)
                    {
                        _defTransType = defTrans.Id;
                        lukTransportType.EditValue = _defTransType;
                    }
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing form. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }
        List<VwCountryAndCityViewDTO> cityDetailList { get; set; }
        private void GetAndPopulateSubCity()
        {
            cityDetailList = UIProcessManager.GetAllCountryAndCityView();
            /* if (cityDetailList == null || cityDetailList.Count == 0)
             {
                 SystemMessage.ShowModalInfoMessage("Country and City detail is not found!", "ERROR");
                ////CNETInfoReporter.Hide();
                 return false;
             }*/

            sluFromCity.Properties.DataSource = cityDetailList;
            sluToCity.Properties.DataSource = cityDetailList;
        }


        private void PopulateRoute()
        {
            _routeDtoList.Clear();
            repoSluRoute.DataSource = null;

            _routeDetailList = UIProcessManager.GetAllRouteView();
            if (_routeDetailList == null)
            {
                _routeDetailList = new List<VwRouteDetailDTO>();
            }

            foreach (var route in _routeDetailList)
            {
                RouteVM dto = new RouteVM();
                dto.Carrier = route.CarrierDescription;
                dto.FlightCode = "";//route.Code;
                dto.Id = route.Id;
                dto.TransportType = route.TransportTypeDescription;
                dto.From = string.Format("{0} [{1}], {2}", route.FromStationDescription, route.CityName,
                    route.FromCountryName);
                dto.To = string.Format("{0} [{1}], {2}", route.ToStationDescription, route.ToCityName,
                    route.ToCountryName);

                _routeDtoList.Add(dto);
            }

            repoSluRoute.DataSource = _routeDtoList;
        }

        private void Reset()
        {
            beiRoute.EditValue = null;
            teFlightCode.EditValue = "";
            teDistance.EditValue = "";
            teDuration.EditValue = "";
            //lukCarrier.EditValue = _defCarrier;
            searchLookUpEdit1.EditValue = _defCarrier;
            lukTransportType.EditValue = _defTransType;
            sluFromStation.EditValue = _defStation;
            sluToStation.EditValue = _defStation;
            sluFromCity.EditValue = null;
            sluToCity.EditValue = null;
            sluState.EditValue = null;
            memoRemark.EditValue = null;
        }

        #endregion


        #region Event Handlers
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
        }
        private void frmRouteEdit_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }
        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                List<Control> controls = new List<Control>
                {
                    teFlightCode,
                    searchLookUpEdit1,
                    lukTransportType,
                    sluFromCity,
                    sluToCity,
                    sluFromStation,
                    sluToStation,
                    sluState,
                    teDuration,
                    teDistance
                };


                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    SystemMessage.ShowModalInfoMessage("Please fill all required fields!", "ERROR");
                    return;
                }

                // Progress_Reporter.Show_Progress("Saving Route", "Please Wait...");

                //Populate Route
                RouteDTO route = new RouteDTO()
                {
                    // Id = teFlightCode.EditValue.ToString(),
                    Code = teFlightCode.EditValue.ToString(),
                    Carrier = searchLookUpEdit1.EditValue.ToString(),
                    Distance = Convert.ToDecimal(teDistance.EditValue.ToString()),
                    Duration = Convert.ToDecimal(teDuration.EditValue.ToString()),
                    FromCity = Convert.ToInt32(sluFromCity.EditValue),
                    ToCity = Convert.ToInt32(sluToCity.EditValue),
                    FromStation = Convert.ToInt32(sluFromStation.EditValue),
                    ToStation = Convert.ToInt32(sluToStation.EditValue),
                    TransportType = Convert.ToInt32(lukTransportType.EditValue),
                    ObjectState = Convert.ToInt32(sluState.EditValue),
                    Remark = memoRemark.EditValue == null ? "" : memoRemark.EditValue.ToString()

                };


                bool isEdit = beiRoute.EditValue == null || string.IsNullOrEmpty(beiRoute.EditValue.ToString()) ? false : true;
                RouteDTO isSaved = null;
                if (isEdit)
                {
                    RouteDTO routeToEdit = UIProcessManager.GetRouteById(Convert.ToInt32(beiRoute.EditValue));
                    if (routeToEdit == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to get the route to update", "ERROR");
                        return;
                    }

                    route.Id = routeToEdit.Id;
                    isSaved = UIProcessManager.UpdateRoute(route);
                }
                else
                {
                    isSaved = UIProcessManager.CreateRoute(route);

                }

                if (isSaved != null)
                {
                    SystemMessage.ShowModalInfoMessage("Route is saved successfully!", "MESSAGE");
                    Reset();
                    PopulateRoute();
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Route is not saved", "ERROR");
                }

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving Route. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }


        #endregion

        private void beiRoute_EditValueChanged(object sender, EventArgs ee)
        {
            if (beiRoute.EditValue == null) return;
            VwRouteDetailDTO dto = _routeDetailList.FirstOrDefault(r => r.Id == Convert.ToInt32(beiRoute.EditValue));
            if (dto == null) return;
            teFlightCode.EditValue =  dto.Code;
            teDistance.EditValue = dto.Distance;
            teDuration.EditValue = dto.Duration;
            //searchLookUpEdit1.EditValue = dto.Carrier;
            searchLookUpEdit1.EditValue = dto.Carrier;
            lukTransportType.EditValue = dto.TransportType;
            sluFromCity.EditValue = dto.FromCity;
            sluToCity.EditValue = dto.ToCity;
            sluFromStation.EditValue = dto.FromStation;
            sluToStation.EditValue = dto.ToStation;
            sluState.EditValue = dto.State;
        }

        private void sluToCity_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmSubCountry coun = new frmSubCountry();
            coun.ShowDialog();
            if (frmSubCountry.SavedCountry != null)
            {
                GetAndPopulateSubCity();
                VwCountryAndCityViewDTO subcoun = cityDetailList.FirstOrDefault(x => x.Name == frmSubCountry.SavedCountry.Name);
                if (subcoun != null)
                    e.NewValue = subcoun.Id;
            }
        }

        private void sluFromCity_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmSubCountry coun = new frmSubCountry();
            coun.ShowDialog();
            if (frmSubCountry.SavedCountry != null)
            {
                GetAndPopulateSubCity();
                VwCountryAndCityViewDTO subcoun = cityDetailList.FirstOrDefault(x => x.Name == frmSubCountry.SavedCountry.Name);
                if (subcoun != null)
                    e.NewValue = subcoun.Id;
            }

        }



    }
}
