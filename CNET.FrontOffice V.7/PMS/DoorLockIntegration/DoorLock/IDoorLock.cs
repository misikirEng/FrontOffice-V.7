
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.SettingSchema;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    /// <summary>
    /// Implement this interface for different model and type of door locks that must provide
    /// the decalred functions
    /// Design Pattern: Factory 
    /// Developer: Melake
    /// Date: 02-10-2017
    /// </summary>
    public interface IDoorLock
    {
        /// <summary>
        /// Initialization of the door lock.
        /// </summary>
        /// <param name="deviceConfigs"> the door lock's device configurations</param>
        /// <returns>True: if success; False: otherwise</returns>
        bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device);

        /// <summary>
        /// This implementation should provide the name of the current object of the lock type
        /// </summary>
        /// <returns>name of the door lock</returns>
        string GetDoorLockName();

        /// <summary>
        /// Since the return status code of different door locks are different, implement this method to show the status message. 
        /// 
        /// </summary>
        /// <param name="status">status code of any operations result</param>
        void ShowStatusMessage(int status);

        /// <summary>
        /// To get the unique identifier of the card (RFID TAG)
        /// </summary>
        /// <param name="showStatusMessage"> if False: any status messages will not be shown </param>
        /// <returns>the SN number of the card </returns>
        string GetCardSN(bool showStatusMessage = true);

        /// <summary>
        /// Reads the current issue information from the card and wrap them on "CardInfo" object
        /// </summary>
        /// <param name="showStatusMessage">if False: any status messages will not be shown </param>
        /// <returns>"CardInfo" object containg the current issue information from the card</returns>
        CardInfo ReadCardData(bool showStatusMessage = true);

        /// <summary>
        /// Issuing the guest card. Note: the startDate and endDate should be converted to a string
        /// with the format gained from "GetStringFormatOfDate()" Implementation
        /// </summary>
        /// <param name="lockNumber">the lock number of the current room </param>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">expired date</param>
        /// <returns>True: if success and False: otherwise </returns>
        bool IssueGuestCard(string lockNumber, DateTime startDate, DateTime endDate, bool isDuplicate = false);


        /// <summary>
        /// Erases every data on the card
        /// </summary>
        /// <returns>True: if success and False: otherwise</returns>
        bool ClearCard(string lockNumber = null);

        /// <summary>
        /// Since the start date and end date formate of different lock types are different, this method 
        /// returns the string format of a date
        /// </summary>
        /// <returns>string format of date </returns>
        string GetStringFormatOfDate();
    }





}
