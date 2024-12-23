using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.PMS.DTO;
using CNET_V7_Domain.Domain.CommonSchema;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Vouchers
{
    public partial class frmForeignExTransaction : UILogicBase
    {
        private int? _defCurrency = null;
        private string _defCurrDesc = "";
        private string _voucherNo;
        private decimal _payment;

        private List<ForeignExTranDTO> _dtoList = new List<ForeignExTranDTO>();

        //only getter
        public List<ForeignExTranDTO> AddedForeignExTranList
        {
            get
            {
                return _dtoList.Where(d => d.CurrencyCode != null).ToList();
            }
        }

        public frmForeignExTransaction(string voucherNo, decimal payment)
        {
            InitializeComponent();

            _voucherNo = voucherNo;
            _payment = payment;

            InitializeUI();
        }

        #region Helper Methods

        private void InitializeUI()
        {
            teVoucherNo.Text = _voucherNo.ToString();
            tePayment.Text = _payment.ToString();
            teRemainingAmt.Text = _payment.ToString();
            teTotalAmt.Text = "0.0";
            bbiSave.Enabled = false;

            lukCurrency.EditValueChanged += lukCurrency_EditValueChanged;
            teAmount.EditValueChanged += teAmount_EditValueChanged;
            ceAssign.CheckedChanged += ceAssign_CheckedChanged;

            //Currency
            lukCurrency.Columns.Add(new LookUpColumnInfo("Description", "Name"));
            lukCurrency.DisplayMember = "Description";
            lukCurrency.ValueMember = "Id";
        }

        private bool InitializeData()
        {
            try
            {
                //currency
                List<CurrencyDTO> currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                lukCurrency.DataSource = currencyList;
                if (currencyList != null)
                {
                    var defCurrency = currencyList.FirstOrDefault(c => c.IsDefault == true);
                    if (defCurrency != null)
                    {
                        //lukCurrency.value = defCurrency.code;
                        _defCurrency = defCurrency.Id;
                        _defCurrDesc = defCurrency.Description;
                        //Get Exchange Rate

                    }
                }

                if (AddedForeignExTranList != null)
                {
                    gcForeEx.DataSource = null;
                    gcForeEx.DataSource = AddedForeignExTranList;
                    gcForeEx.RefreshDataSource();
                    gvForeExt.RefreshData();

                    ValidateAmount(AddedForeignExTranList);

                }


                AddLastRow();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ValidateAmount(List<ForeignExTranDTO> dtoList)
        {

            decimal totalAmount = Math.Round(dtoList.Sum(d => d.TotalAmount), 2);
            decimal remaining = Math.Round(_payment - totalAmount, 2);
            teTotalAmt.Text = totalAmount.ToString();
            teRemainingAmt.Text = remaining.ToString();

            if (totalAmount < _payment)
                bbiSave.Enabled = false;
            else
                bbiSave.Enabled = true;

        }

        private void AddLastRow(bool isDelete = false)
        {
            List<ForeignExTranDTO> dtoList = gcForeEx.DataSource as List<ForeignExTranDTO>;
            if (dtoList == null || dtoList.Count == 0)
            {
                dtoList = new List<ForeignExTranDTO>();
                ForeignExTranDTO dto = new ForeignExTranDTO()
                {
                    SN = 1,
                    Amount = 0,
                    CurrencyCode = null,
                    CurrencyDesc = "",
                    ExRate = 0,
                    Remark = ""
                };
                dtoList.Add(dto);
                gcForeEx.DataSource = dtoList;
                gvForeExt.RefreshData();

                _dtoList = dtoList;
                return;
            }

            ForeignExTranDTO lastEmptyDto = dtoList.OrderByDescending(l => l.SN).FirstOrDefault(f => f.CurrencyCode == null);

            if (isDelete)
            {
                if (lastEmptyDto != null)
                {
                    lastEmptyDto.SN = lastEmptyDto.SN - 1;
                    gcForeEx.DataSource = dtoList;
                    gvForeExt.RefreshData();
                    _dtoList = dtoList;

                    gvForeExt.FocusedColumn = gvForeExt.Columns[0];
                    return;
                }
            }
            else
            {
                if (lastEmptyDto != null)
                    return;
            }

            ForeignExTranDTO lastDto = dtoList.OrderByDescending(l => l.SN).FirstOrDefault(f => f.CurrencyCode != null);
            if (lastDto != null)
            {
                ForeignExTranDTO dto = new ForeignExTranDTO()
                {
                    SN = lastDto.SN + 1,
                    Amount = 0,
                    CurrencyCode = null,
                    CurrencyDesc = "",
                    ExRate = 0,
                    Remark = ""
                };
                dtoList.Add(dto);
                gcForeEx.DataSource = dtoList;
                gvForeExt.RefreshData();
            }

            _dtoList = dtoList;
        }

        #endregion

        #region Event Handlers

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();

        }

        private void teAmount_EditValueChanged(object sender, EventArgs e)
        {
            TextEdit view = sender as TextEdit;
            if (view == null || view.EditValue == null) return;

            //get dto
            ForeignExTranDTO dto = gvForeExt.GetFocusedRow() as ForeignExTranDTO;
            if (dto == null) return;
            try
            {
                var currentExRate = CommonLogics.GetLatestExchangeRate(dto.CurrencyCode.Value);
                dto.ExRate = currentExRate;
                dto.Amount = Math.Round(Convert.ToDecimal(view.EditValue.ToString()), 2);
                dto.TotalAmount = Math.Round(dto.Amount * dto.ExRate, 2);

                gvForeExt.RefreshRow(gvForeExt.FocusedRowHandle);

                List<ForeignExTranDTO> dtoList = gvForeExt.DataSource as List<ForeignExTranDTO>;
                ValidateAmount(dtoList);
            }
            catch (Exception ex)
            {

            }

        }

        private void lukCurrency_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit view = sender as LookUpEdit;
            if (view == null || view.EditValue == null) return;

            //get dto
            ForeignExTranDTO dto = gvForeExt.GetFocusedRow() as ForeignExTranDTO;
            if (dto == null) return;
            try
            {
                dto.CurrencyCode = Convert.ToInt32(view.EditValue);
                var currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == dto.CurrencyCode);
                dto.CurrencyDesc = currency == null ? "" : currency.Description;
                var currentExRate = CommonLogics.GetLatestExchangeRate(dto.CurrencyCode.Value);
                dto.ExRate = currentExRate;
                dto.Amount = Math.Round(Convert.ToDecimal(dto.Amount.ToString()), 2);
                dto.TotalAmount = Math.Round(dto.Amount * dto.ExRate, 2);

                gvForeExt.RefreshRow(gvForeExt.FocusedRowHandle);

                AddLastRow();

            }
            catch (Exception ex)
            {

            }
        }

        private void frmForeignExTransaction_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.Close();
        }

        private void bbiAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var currentExRate = CommonLogics.GetLatestExchangeRate(_defCurrency.Value);
                ForeignExTranDTO dto = new ForeignExTranDTO()
                {
                    SN = _dtoList.Count + 1,
                    Amount = 0,
                    CurrencyCode = _defCurrency,
                    CurrencyDesc = _defCurrDesc,
                    ExRate = currentExRate,
                    Remark = ""
                };

                _dtoList.Add(dto);



                gcForeEx.DataSource = _dtoList;
                gvForeExt.RefreshData();
            }
            catch (Exception ex)
            {

            }
        }

        private void gvForeExt_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            ForeignExTranDTO dto = view.GetRow(e.RowHandle) as ForeignExTranDTO;
            if (dto == null) return;
            dto.SN = (e.RowHandle + 1);
        }

        private void ceAssign_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit view = sender as CheckEdit;
            if (view == null) return;

            ForeignExTranDTO dto = gvForeExt.GetFocusedRow() as ForeignExTranDTO;
            if (dto == null || dto.CurrencyCode == null) return;

            if (view.Checked)
            {
                decimal remaining = Convert.ToDecimal(teRemainingAmt.Text);
                if (remaining > 0)
                {
                    var currentExRate = CommonLogics.GetLatestExchangeRate(dto.CurrencyCode.Value);
                    dto.ExRate = currentExRate;
                    dto.Amount = Math.Round(remaining / dto.ExRate, 2);
                    dto.TotalAmount = Math.Round(remaining, 2);
                    dto.IsAssigned = true;

                }
            }
            else
            {
                dto.Amount = 0;
                dto.ExRate = 1;
                dto.TotalAmount = 0;
                dto.IsAssigned = false;

            }


            //Refresh Row
            gvForeExt.RefreshRow(gvForeExt.FocusedRowHandle);
            List<ForeignExTranDTO> dtoList = gvForeExt.DataSource as List<ForeignExTranDTO>;
            ValidateAmount(dtoList);

        }

        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ForeignExTranDTO dto = gvForeExt.GetFocusedRow() as ForeignExTranDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select an entry!", "ERROR");
                return;
            }

            if (dto.CurrencyCode == null) return;

            _dtoList.Remove(dto);
            gcForeEx.DataSource = _dtoList;
            gcForeEx.RefreshDataSource();
            gvForeExt.RefreshData();

            ValidateAmount(_dtoList);

            AddLastRow(true);

        }

        private void bbiClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _dtoList.Clear();
            gcForeEx.DataSource = null;
            gcForeEx.RefreshDataSource();
            gvForeExt.RefreshData();

            ValidateAmount(_dtoList);

            AddLastRow();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //ReportGenerator rg = new ReportGenerator();
                //List<ForeignExTranDTO> dtoList = _dtoList.Where(d => !string.IsNullOrWhiteSpace(d.CurrencyCode)).ToList();
                //if (dtoList != null && dtoList.Count > 0)
                //{
                //    decimal total = dtoList.Sum(d => d.TotalAmount);
                //    rg.GenerateForeginCurrency(dtoList, _voucherNo, _payment, total, true);
                //}
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing Foreign Transaction. Detail:: " + ex.Message, "ERROR");
            }
        }

        #endregion







    }
}
