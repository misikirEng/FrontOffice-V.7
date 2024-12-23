using CNET.ERP.Client.UI_Logic.PMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.Logics
{
    public class SplitWindow
    {
        private int _windowNumber = -1;
        private List<SplitDTO> _splitDtoList = new List<SplitDTO>();
        private string _balance = "0.0";
        private bool _enabled = true;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }
        public string Balance
        {
            set
            {
                _balance = value;
            }
            get
            {
                return _balance;
            }
        }

        public int WindowNumber
        {
            set
            {
                _windowNumber = value;
            }
            get
            {
                return _windowNumber;
            }
        }

        public void AddSplitDTO(SplitDTO splitDto)
        {
            if (splitDto == null) return;
            _splitDtoList.Add(splitDto);
        }

        public void RemoveSplitDTO(SplitDTO splitDto)
        {
            if (splitDto == null) return;
            _splitDtoList.Remove(splitDto);
        }

        public List<SplitDTO> GetSplitDtoList()
        {
            return _splitDtoList.OrderBy(s => s.VoucherCode).ToList();
        }

        public List<SplitDTO> GetSplitDtoListByPrintStatus(int status)
        {
            return _splitDtoList.Where(s => s.PrintStatus == status).ToList();
        }


    }
}
