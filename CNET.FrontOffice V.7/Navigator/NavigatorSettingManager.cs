using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static DevExpress.CodeParser.CodeStyle.Formatting.Rules.Spacing;

namespace CNET.ERP.Client.Common.UI.Library.Navigator
{
    public class NavigatorSettingManager
    {
        public NavigatorSettingManager()
        {

        }
        public CNET.ERP.Navigator.DTO.NavigatorDTO DTO;

        /// <summary>
        /// 
        /// /////////////////////// XML Navigator File Generation ////////////////////////////////
        /// </summary>
        public void GenerateNavigationXml()
        {
            try
            {
                GetNavigationDTO();

               
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in generating navigation::" + ex.Message);
            }

        }

        /// <summary>
        /// Get root Navigator DTO class
        /// </summary>
        /// <returns></returns>
        private CNET.ERP.Navigator.DTO.NavigatorDTO GetNavigationDTO()
        {
            CNET.ERP.Navigator.DTO.NavigatorDTO navigatorDto = new CNET.ERP.Navigator.DTO.NavigatorDTO();
            navigatorDto.Name = "Desktop Navigation";
            navigatorDto.Ancestors = new List<CNET.ERP.Navigator.DTO.CNETNavigatorAncestor>();

            // Ancestors 
            DTO = MapAncestors(navigatorDto);
            return DTO;

        }

        /////////////////////////////// ANCESTORS ///////////////////////////////////


        private CNET.ERP.Navigator.DTO.NavigatorDTO MapAncestors(CNET.ERP.Navigator.DTO.NavigatorDTO navigatorDTO)
        {
            #region PMS
            //main menu
            CNET.ERP.Navigator.DTO.CNETNavigatorAncestor PMSMenuAncestor = new CNET.ERP.Navigator.DTO.CNETNavigatorAncestor()
            {
                Name = "Main Menu",
                ToolTip = "",
                Childs = MapPMSMainCategories()
            };

            //house keeping
            CNET.ERP.Navigator.DTO.CNETNavigatorAncestor PMSHouseKeepingAncestor = new CNET.ERP.Navigator.DTO.CNETNavigatorAncestor()
            {
                Name = "House Keeping",
                ToolTip = "",
                Childs = MapPMSHousekeepingNodes()
            };

            //house keeping
            CNET.ERP.Navigator.DTO.CNETNavigatorAncestor PMSEventManagementAncestor = new CNET.ERP.Navigator.DTO.CNETNavigatorAncestor()
            {
                Name = "Event Management",
                ToolTip = "",
                Childs = MapPMSEventManagementNodes()
            };

            navigatorDTO.Ancestors.Add(PMSMenuAncestor);
            navigatorDTO.Ancestors.Add(PMSHouseKeepingAncestor);
            navigatorDTO.Ancestors.Add(PMSEventManagementAncestor);


            #endregion

            return navigatorDTO;
        }
        #region PMS Categories

        // Map PMS  Categories
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSMainCategories()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsCategories = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();

            // Categories
            #region Categories

            // Main Page 
            CNET.ERP.Navigator.DTO.CNETNavigatorNode mainPage = new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
            {
                Name = "Main Page",
                Code = "",
                FormName = "",
                ToolTip = "",
                IsSelected = false,
                Expanded = false,
                Childs = MapPMSMainNodes()
            };

            // Registration 
            CNET.ERP.Navigator.DTO.CNETNavigatorNode registration = new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
            {
                Name = "Registration",
                Code = "",
                FormName = "",
                ToolTip = "",
                IsSelected = false,
                Expanded = false,
                Childs = MapPMSRegistrationNodes()
            };


            // Profile
            CNET.ERP.Navigator.DTO.CNETNavigatorNode profile = new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
            {
                Name = "Profile",
                Code = "",
                FormName = "",
                ToolTip = "",
                IsSelected = false,
                Expanded = false,
                Childs = MapPMSProfileNodes()
            };


            // Night Audit
            CNET.ERP.Navigator.DTO.CNETNavigatorNode nightAudit = new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
            {
                Name = "Night Audit",
                Code = "",
                FormName = "",
                ToolTip = "",
                IsSelected = false,
                Expanded = false,
                Childs = MapPMSNightAuditNodes()
            };

            // Posting Routine 
           /* CNET.ERP.Navigator.DTO.CNETNavigatorNode postingRoutine = new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
            {
                Name = "Posting Routine",
                Code = "",
                FormName = "",
                ToolTip = "",
                IsSelected = false,
                Expanded = false,
                Childs = MapPostingRoutineNodes()
            };*/


            // Report
            CNET.ERP.Navigator.DTO.CNETNavigatorNode report = new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
            {
                Name = "Report",
                Code = "",
                FormName = "",
                ToolTip = "",
                IsSelected = false,
                Expanded = false
            };

            // Setting and Misc
            CNET.ERP.Navigator.DTO.CNETNavigatorNode settingMisc = new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
            {
                Name = "Setting And Miscellaneous",
                Code = "",
                FormName = "",
                ToolTip = "",
                IsSelected = false,
                Expanded = false,
                Childs = MapPMSSettingMiscNodes()
            };




            #endregion

            pmsCategories.Add(mainPage);
            pmsCategories.Add(registration);
            pmsCategories.Add(profile);
            pmsCategories.Add(nightAudit);
           // pmsCategories.Add(postingRoutine);
            pmsCategories.Add(report);
            pmsCategories.Add(settingMisc);


            return pmsCategories;


        }


        #endregion

        #region PMS Nodes

        /// <summary>
        /// Map PMS Main Page Nodes Manually
        /// </summary>
        /// <returns></returns>
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSMainNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsMainNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
            string[] nodes = { "Document Browser", "Registration Document", "Density Chart", "Room Status", "Room Inventory", "Package Audit" };

            for (int i = 0; i < nodes.Length; i++)
            {

                pmsMainNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }

            return pmsMainNodes;
        }

        /// <summary>
        /// Map PMS Reservation/Registration Nodes Manually
        /// </summary>
        /// <returns></returns>
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSRegistrationNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsRegNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
            string[] nodes = { "Reservation", "Check In", "Group Registration" };

            for (int i = 0; i < nodes.Length; i++)
            {

                pmsRegNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }

            return pmsRegNodes;
        }

        /// <summary>
        /// Map PMS Profile Nodes Manually
        /// </summary>
        /// <returns></returns>
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSProfileNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsProfileNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
            string[] nodes = { "Guest", "Contact", "Company", "Travel Agent", "Source", "Group" };

            for (int i = 0; i < nodes.Length; i++)
            {

                pmsProfileNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }

            return pmsProfileNodes;
        }

        /// <summary>
        /// Map PMS Night Audit Nodes Manually
        /// </summary>
        /// <returns></returns>
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSNightAuditNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsNightAuditNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
            string[] nodes = { "End of Day", "End of Month" };

            for (int i = 0; i < nodes.Length; i++)
            {

                pmsNightAuditNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }

            return pmsNightAuditNodes;
        }

        /// <summary>
        /// Map PMS Setting and Miscellaneous Nodes Manually
        /// </summary>
        /// <returns></returns>
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSSettingMiscNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsSettingMiscNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
            string[] nodes = {"Fiscal Printer", "Property", "Package", "Revenue Management", "Calendar", "Budget", "License"};

            if (LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName.ToLower().Contains("cnet admin"))
            {
                nodes = nodes.Append("ERP Update").ToArray();
            }

            for (int i = 0; i < nodes.Length; i++)
            {

                pmsSettingMiscNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }

            return pmsSettingMiscNodes;
        }


        /// <summary>
        /// Map PMS House Keeping Nodes Manually
        /// </summary>
        /// <returns></returns>
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSHousekeepingNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsProfileNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
            string[] nodes = { "Task Assignment", "Room Management", "Discrepancy", "Turndown Management" };

            for (int i = 0; i < nodes.Length; i++)
            {

                pmsProfileNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }

            return pmsProfileNodes;
        }
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPMSEventManagementNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> pmsProfileNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
            string[] nodes = { "Event Management"};

            for (int i = 0; i < nodes.Length; i++)
            {

                pmsProfileNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }

            return pmsProfileNodes;
        }

        
        private List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> MapPostingRoutineNodes()
        {
            List<CNET.ERP.Navigator.DTO.CNETNavigatorNode> postineRoutineNodes = new List<CNET.ERP.Navigator.DTO.CNETNavigatorNode>();
          /* List<PostingRoutineHeader> prHeaderList = ACCUIProcessManager.GetPostingRoutineHeadersByComponent(_module);

            if (prHeaderList == null && prHeaderList.Count == 0) return postineRoutineNodes;

            string[] nodes = prHeaderList.Select(p => p.description).ToArray();


            for (int i = 0; i < nodes.Length; i++)
            {

                postineRoutineNodes.Add(new CNET.ERP.Navigator.DTO.CNETNavigatorNode()
                {
                    Code = "",
                    IsDisabled = false,
                    Name = nodes[i],
                    FormName = nodes[i],
                    ToolTip = "",
                    IsSelected = false,
                    Expanded = false

                });
            }
            */
            return postineRoutineNodes;
        }


        #endregion

    }


}