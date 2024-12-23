using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CNET.ERP.Navigator.DTO
{
    [Serializable]
    [XmlRoot("CnetNavigator")]
    public class NavigatorDTO
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("CNETNavigatorAncestor")]
        public List<CNETNavigatorAncestor> Ancestors { get; set; }


    }


    public class CNETNavigatorAncestor
    {

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ToolTip { get; set; }

        [XmlElement("CNETNavigatorNode")]
        public List<CNETNavigatorNode> Childs { get; set; }


        //public NavigatorDTO Creator { get; set; }



    }

   
    public class CNETNavigatorNode
    {
        [XmlAttribute]
        public string Code { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ToolTip { get; set; }

        [XmlAttribute]
        public bool Expanded { get; set; }

        [XmlAttribute]
        public string FormName { get; set; }

        [XmlAttribute]
        public string Tag { get; set; }

        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public bool IsSelected { get; set; }

        [XmlAttribute]
        public bool IsDisabled { get; set; }

        [NonSerialized]
        private string _category;
        [XmlIgnore]
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }

      
      //  public CNETNavigatorAncestor Ancestor { get; set; }

      
       // public CNETNavigatorNode Parent { get; set; }

        [XmlElement("CNETNavigatorNode")]
        public List<CNETNavigatorNode> Childs { get; set; }

    }
}
