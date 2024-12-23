using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CNETNavigator.Navigator
{
    public class AllowedNavigatorMenus
    {
        private string navigatorMenu;

        public string NavigatorMenu
        {
            get { return navigatorMenu; }
            set { navigatorMenu = value; }
        }
        private List<String> allowedFunctionalities;

        public List<String> AllowedFunctionalities
        {
            get { return allowedFunctionalities; }
            set { allowedFunctionalities = value; }
        }

        public AllowedNavigatorMenus(String navigatorMenu, List<String> allowedFunctionality)
        {
            this.navigatorMenu = navigatorMenu;
            this.allowedFunctionalities = allowedFunctionality;
        }

        public AllowedNavigatorMenus()
        { 
            
        }

        public static List<String> readallDataFromXmlFile(String navigator)//to read the xml File and load to allcontant field
        {
            XmlDocument xmlFile = new XmlDocument();//xml reader

            xmlFile.Load("AllowedPrivillages.xml");//load the xml file into my xml doument
            List<String> listSecTabs = new List<String>();
            foreach (XmlNode node in xmlFile.DocumentElement.ChildNodes)
            {
                foreach (XmlNode childNode in node)
                {
                    bool x = childNode.InnerText.Equals(navigator,StringComparison.OrdinalIgnoreCase);
                    if (x)
                    { 
                        if (childNode.Name == "Operation")
                        {
                            XmlNode child2Node;
                            for (int i = 1; i < node.ChildNodes.Count; i++)
                            {
                                child2Node = node.ChildNodes[i];
                                if (i > 1 && (child2Node.Name == "Operation"))
                                    return listSecTabs;
                           
                                listSecTabs.Add(child2Node.InnerText);
                           
                            }
                        }
                    }
                    
                }
            }

            return listSecTabs;
        }

    }
}
