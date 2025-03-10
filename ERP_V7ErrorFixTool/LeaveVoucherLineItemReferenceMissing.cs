using CNET_V7_Domain;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using DevExpress.CodeParser;
using ProcessManager;
using System.Windows.Media.Animation;

namespace ERP_V7ErrorFixTool
{
    public partial class LeaveVoucherLineItemReferenceMissing : Form
    {
        public LeaveVoucherLineItemReferenceMissing()
        {
            InitializeComponent();
        }

        public static List<LeaveVoucherMissingDTO> DistributeQuantities(List<VwVoucherLineItemDetailDTO> LeaveDefition, List<VwVoucherLineItemDetailDTO> LeaveVoucher)
        {
            List<LeaveVoucherMissingDTO> result = new List<LeaveVoucherMissingDTO>();
            int aIndex = 0, bIndex = 0;
            int count = 0;
            while (aIndex < LeaveDefition.Count && bIndex < LeaveVoucher.Count)
            {
                var giver = LeaveDefition[aIndex];
                var taker = LeaveVoucher[bIndex];

                decimal transferValue = Math.Min(giver.Quantity, taker.Quantity);
                result.Add(new LeaveVoucherMissingDTO
                {
                    sn = count,
                    Name = giver.Name,
                    LeaveDefinitionLineItemId = giver.LineItemId,
                    LeaveDefinitionVoucherId = giver.Voucher,
                    LeaveDefinitionLineItemQuantity = giver.Quantity,
                    LeaveDefinitionVoucherdefintion = giver.VoucherDefinition,
                    LeaveDefinitionVoucherCode = giver.VoucherCode,
                    LeaveVoucherId = taker.Voucher,
                    LeaveLineItemId = taker.LineItemId,
                    LeaveLineItemQuantity = taker.Quantity,
                    LeaveVoucherdefinition = taker.VoucherDefinition,
                    LeaveVoucherCode = taker.VoucherCode,
                    LineItemReferenceValue = transferValue

                    //ACode = giver.Code,
                    //BCode = taker.Code,
                    //Value = transferValue,
                    //AQuantity = giver.Quantity,
                    //BQuantity = taker.Quantity 
                });
                count++;
                giver.Quantity -= transferValue;
                taker.Quantity -= transferValue;

                if (giver.Quantity == 0) aIndex++;
                if (taker.Quantity == 0) bIndex++;
            }

            return result;
        }
        public static List<LeaveVoucherMissingDTO> DistributeQuantitie(List<VwVoucherLineItemDetailDTO> LeaveDefition, List<VwVoucherLineItemDetailDTO> LeaveVoucher)
        {
            List<LeaveVoucherMissingDTO> result = new List<LeaveVoucherMissingDTO>();
            int aIndex = 0, bIndex = 0;
            int count = 1;
            while (aIndex < LeaveDefition.Count && bIndex < LeaveVoucher.Count)
            {

                var giver = LeaveDefition[aIndex];
                var taker = LeaveVoucher[bIndex];

                decimal transferValue = Math.Min(giver.Quantity, taker.Quantity);
                result.Add(new LeaveVoucherMissingDTO
                {
                    sn = count,
                    Name = giver.Name,
                    LeaveDefinitionLineItemId = giver.LineItemId,
                    LeaveDefinitionVoucherId = giver.Voucher,
                    LeaveDefinitionLineItemQuantity = LeaveDefition[aIndex].Quantity,
                    LeaveDefinitionVoucherdefintion = giver.VoucherDefinition,
                    LeaveDefinitionVoucherCode = giver.VoucherCode,
                    LeaveVoucherId = taker.Voucher,
                    LeaveLineItemId = taker.LineItemId,
                    LeaveLineItemQuantity = LeaveVoucher[bIndex].Quantity,
                    LeaveVoucherdefinition = taker.VoucherDefinition,
                    LeaveVoucherCode = taker.VoucherCode,
                    LineItemReferenceValue = transferValue

                });
                count++;
                giver.Quantity -= transferValue;
                taker.Quantity -= transferValue;

                if (giver.Quantity == 0) aIndex++;
                if (taker.Quantity == 0) bIndex++;
            }

            return result;
        }



        private void btnShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<VwVoucherLineItemDetailDTO> LeaveVoucherdata = UIProcessManager.GetVoucherLightViewByDefinition(CNETConstants.LEAVE_VOUCHER);
            List<VwVoucherLineItemDetailDTO> LeaveDefintiondata = UIProcessManager.GetVoucherLightViewByDefinition(CNETConstants.LEAVE_DEFINITION_VOUCHER);
            List<LeaveVoucherMissingDTO> datalist = new List<LeaveVoucherMissingDTO>();
            List<LeaveVoucherMissingDTO> result = new List<LeaveVoucherMissingDTO>();
            if (LeaveVoucherdata != null && LeaveVoucherdata.Count > 0)
                LeaveVoucherdata = LeaveVoucherdata.Where(x => x.ArticleCode == "HRMS-028" && x.IsVoid == false).ToList();

            if (LeaveDefintiondata != null && LeaveDefintiondata.Count > 0)
                LeaveDefintiondata = LeaveDefintiondata.Where(x =>x.IsVoid == false).ToList();
            if (LeaveVoucherdata != null && LeaveVoucherdata.Count > 0)
            {
                List<int> consigneeidlist = LeaveVoucherdata.Where(c => c.ConsigneeId != null).Select(x => x.ConsigneeId.Value).Distinct().ToList();

                foreach (int consignee in consigneeidlist)
                {
                    List<VwVoucherLineItemDetailDTO> empolyeeleave = LeaveVoucherdata.Where(x => x.ConsigneeId != null && x.ConsigneeId.Value == consignee).ToList();
                    List<VwVoucherLineItemDetailDTO> empolyeeleavedefinition = LeaveDefintiondata.Where(x => x.ConsigneeId != null && x.ConsigneeId == consignee).ToList();

                    if (empolyeeleave != null && empolyeeleave.Count > 0 && empolyeeleavedefinition != null && empolyeeleavedefinition.Count > 0)
                    {
                        datalist = DistributeQuantities(empolyeeleavedefinition, empolyeeleave);
                        if (datalist != null && datalist.Count > 0)
                            result.AddRange(datalist);
                    }
                }
            }
            gridControl1.DataSource = result;
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<LeaveVoucherMissingDTO> datasourcedata = (List<LeaveVoucherMissingDTO>)gridControl1.DataSource;
            if(datasourcedata != null)
            {
                List<LineItemReferenceDTO> allLineItemreference = UIProcessManager.SelectAllLineItemReference();
                allLineItemreference = allLineItemreference.Where(x=> x.ReferingVouDfn == 239).ToList();
                foreach (LeaveVoucherMissingDTO missingleave in datasourcedata)
                {
                    LineItemReferenceDTO newref = new LineItemReferenceDTO()
                    {
                        LineItem = missingleave.LeaveLineItemId,
                        Voucher = missingleave.LeaveVoucherId,
                        ReferingVouDfn = missingleave.LeaveVoucherdefinition,
                        ReferencedVouDfn = missingleave.LeaveDefinitionVoucherdefintion,
                        Referenced = missingleave.LeaveDefinitionLineItemId,
                        Value = missingleave.LineItemReferenceValue,
                        Remark = "LeaveFix"

                    }; 
                    LineItemReferenceDTO checkexist = allLineItemreference.FirstOrDefault(x=> x.LineItem == newref.LineItem &&x.Referenced == newref.Referenced);
                    if (checkexist == null)
                    {
                        LineItemReferenceDTO create = UIProcessManager.CreateLineItemReference(newref);
                    }
                }

            }
        }
    }
    public class LeaveVoucherMissingDTO
    {
        public int sn{ get; set; }
        public string Name { get; set; }
        public string LeaveDefinitionVoucherCode { get; set; }
        public int LeaveDefinitionVoucherdefintion { get; set; }
        public int LeaveDefinitionVoucherId { get; set; }
        public int LeaveDefinitionLineItemId { get; set; }
        public decimal LeaveDefinitionLineItemQuantity{ get; set; }
        public string LeaveVoucherCode { get; set; }
        public int LeaveVoucherId { get; set; }
        public int LeaveVoucherdefinition { get; set; }
        public int LeaveLineItemId { get; set; }
        public decimal LeaveLineItemQuantity { get; set; }
        public decimal LineItemReferenceValue { get; set; } 
    }
}
