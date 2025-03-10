using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET.API.Manager; 
using System.IO.Packaging;
using CNET.API.Manager.PMS;
using CNET.API.Manager.Common; 
using CNET.API.Manager.Security;
using CNET.API.Manager.Article;
using CNET.API.Manager.Consignee;
using CNET.API.Manager.Setting;
using CNET.API.Manager.Transaction;
using CNET.API.Manager.Accounting;
using CNET_V7_Domain.Domain.TransactionSchema; 
using CNET.API.Manager.Filter;
using System.Net.Http;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.AccountingSchema;
using CNET_V7_Domain.Misc; 
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;  
using System.Data;
using CNET_V7_Domain.Misc.CommonTypes; 
using System.Windows.Forms; 
using System.Reflection.PortableExecutable;
using static CNET.API.Manager.Common.commonRequest;

namespace ProcessManager
{
    public class UIProcessManager
    {
        // List<string> schemalist = new List<string>() { "security", "pms", "common", "setting", "article", "consignee", "transaction" };

        #region common Schema




        public static ClosingValidationDTO CreateClosingValidation(ClosingValidationDTO ClosingValidation)
        {
            return Task.Run(async () => await commonRequest.CreateClosingValidation(ClosingValidation)).Result;
        }

        #region Activity
        public static ActivityDTO CreateActivity(ActivityDTO Activity)
        {
            return Task.Run(async () => await commonRequest.CreateActivity(Activity)).Result;
        }
        public static List<ActivityDTO> SelectAllActivity()
        {
            return Task.Run(async () => await commonRequest.SelectAllActivity()).Result;
        }
        public static ActivityDTO GetActivityById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetActivityById(Id)).Result;
        }
        public static ActivityDTO UpdateActivity(ActivityDTO Activity)
        {
            return Task.Run(async () => await commonRequest.UpdateActivity(Activity)).Result;
        }
        public static bool DeleteActivityById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteActivityById(Id)).Result;
        }
        #endregion
        #region Attachment
        public static AttachmentDTO CreateAttachment(AttachmentDTO Attachment)
        {
            return Task.Run(async () => await commonRequest.CreateAttachment(Attachment)).Result;
        }
        public static List<AttachmentDTO> SelectAllAttachment()
        {
            return Task.Run(async () => await commonRequest.SelectAllAttachment()).Result;
        }
        public static AttachmentDTO GetAttachmentById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetAttachmentById(Id)).Result;
        }
        public static AttachmentDTO UpdateAttachment(AttachmentDTO Attachment)
        {
            return Task.Run(async () => await commonRequest.UpdateAttachment(Attachment)).Result;
        }
        public static bool DeleteAttachmentById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteAttachmentById(Id)).Result;
        }
        #endregion
        #region BeginingBalance
        public static BeginingBalanceDTO CreateBeginingBalance(BeginingBalanceDTO BeginingBalance)
        {
            return Task.Run(async () => await commonRequest.CreateBeginingBalance(BeginingBalance)).Result;
        }
        public static List<BeginingBalanceDTO> SelectAllBeginingBalance()
        {
            return Task.Run(async () => await commonRequest.SelectAllBeginingBalance()).Result;
        }
        public static BeginingBalanceDTO GetBeginingBalanceById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetBeginingBalanceById(Id)).Result;
        }
        public static BeginingBalanceDTO UpdateBeginingBalance(BeginingBalanceDTO BeginingBalance)
        {
            return Task.Run(async () => await commonRequest.UpdateBeginingBalance(BeginingBalance)).Result;
        }
        public static bool DeleteBeginingBalanceById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteBeginingBalanceById(Id)).Result;
        }
        #endregion
        #region CloudSync
        public static CloudSyncDTO CreateCloudSync(CloudSyncDTO CloudSync)
        {
            return Task.Run(async () => await commonRequest.CreateCloudSync(CloudSync)).Result;
        }
        public static List<CloudSyncDTO> SelectAllCloudSync()
        {
            return Task.Run(async () => await commonRequest.SelectAllCloudSync()).Result;
        }
        public static CloudSyncDTO GetCloudSyncById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetCloudSyncById(Id)).Result;
        }
        public static CloudSyncDTO UpdateCloudSync(CloudSyncDTO CloudSync)
        {
            return Task.Run(async () => await commonRequest.UpdateCloudSync(CloudSync)).Result;
        }
        public static bool DeleteCloudSyncById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteCloudSyncById(Id)).Result;
        }
        #endregion
        #region CNETMedia

        //public static CNETMediaDTO CreateCNETMedia(CNETMediaDTO CNETMedia)
        //{
        //    return Task.Run(async () => await commonRequest.CreateCNETMedia(CNETMedia)).Result;
        //}

        //public static List<CNETMediaDTO> SelectAllCNETMedia()
        //{
        //    return Task.Run(async () => await commonRequest.SelectAllCNETMedia()).Result;
        //}

        //public static CNETMediaDTO GetCNETMediaById(int Id)
        //{
        //    return Task.Run(async () => await commonRequest.GetCNETMediaById(Id)).Result;
        //}

        //public static CNETMediaDTO UpdateCNETMedia(CNETMediaDTO CNETMedia)
        //{
        //    return Task.Run(async () => await commonRequest.UpdateCNETMedia(CNETMedia)).Result;
        //}

        //public static bool DeleteCNETMediaById(int Id)
        //{
        //    return Task.Run(async () => await commonRequest.DeleteCNETMediaById(Id)).Result;
        //}

        #endregion
        #region Country
        public static CountryDTO CreateCountry(CountryDTO Country)
        {
            return Task.Run(async () => await commonRequest.CreateCountry(Country)).Result;
        }

        public static List<CountryDTO> SelectAllCountry()
        {
            return Task.Run(async () => await commonRequest.SelectAllCountry()).Result;
        }

        public static CountryDTO GetCountryById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetCountryById(Id)).Result;
        }

        public static CountryDTO UpdateCountry(CountryDTO Country)
        {
            return Task.Run(async () => await commonRequest.UpdateCountry(Country)).Result;
        }

        public static bool DeleteCountryById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteCountryById(Id)).Result;
        }

        #endregion
        #region Currency

        public static CurrencyDTO CreateCurrency(CurrencyDTO Currency)
        {
            return Task.Run(async () => await commonRequest.CreateCurrency(Currency)).Result;
        }

        public static List<CurrencyDTO> SelectAllCurrency()
        {
            return Task.Run(async () => await commonRequest.SelectAllCurrency()).Result;
        }

        public static CurrencyDTO GetCurrencyById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetCurrencyById(Id)).Result;
        }

        public static CurrencyDTO UpdateCurrency(CurrencyDTO Currency)
        {
            return Task.Run(async () => await commonRequest.UpdateCurrency(Currency)).Result;
        }

        public static bool DeleteCurrencyById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteCurrencyById(Id)).Result;
        }

        #endregion
        #region Delegate

        public static DelegateDTO CreateDelegate(DelegateDTO Delegate)
        {
            return Task.Run(async () => await commonRequest.CreateDelegates(Delegate)).Result;
        }

        public static List<DelegateDTO> SelectAllDelegate()
        {
            return Task.Run(async () => await commonRequest.SelectAllDelegates()).Result;
        }

        public static DelegateDTO GetDelegateById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetDelegatesById(Id)).Result;
        }

        public static DelegateDTO UpdateDelegate(DelegateDTO Delegate)
        {
            return Task.Run(async () => await commonRequest.UpdateDelegates(Delegate)).Result;
        }

        public static bool DeleteDelegateById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteDelegatesById(Id)).Result;
        }

        #endregion
        #region Denomination

        public static DenominationDTO CreateDenomination(DenominationDTO Denomination)
        {
            return Task.Run(async () => await commonRequest.CreateDenomination(Denomination)).Result;
        }

        public static List<DenominationDTO> SelectAllDenomination()
        {
            return Task.Run(async () => await commonRequest.SelectAllDenomination()).Result;
        }

        public static DenominationDTO GetDenominationById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetDenominationById(Id)).Result;
        }

        public static DenominationDTO UpdateDenomination(DenominationDTO Denomination)
        {
            return Task.Run(async () => await commonRequest.UpdateDenomination(Denomination)).Result;
        }

        public static bool DeleteDenominationById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteDenominationById(Id)).Result;
        }

        #endregion
        #region ExchangeRate

        public static ExchangeRateDTO CreateExchangeRate(ExchangeRateDTO ExchangeRate)
        {
            return Task.Run(async () => await commonRequest.CreateExchangeRate(ExchangeRate)).Result;
        }

        public static List<ExchangeRateDTO> SelectAllExchangeRate()
        {
            return Task.Run(async () => await commonRequest.SelectAllExchangeRate()).Result;
        }

        public static ExchangeRateDTO GetExchangeRateById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetExchangeRateById(Id)).Result;
        }

        public static ExchangeRateDTO UpdateExchangeRate(ExchangeRateDTO ExchangeRate)
        {
            return Task.Run(async () => await commonRequest.UpdateExchangeRate(ExchangeRate)).Result;
        }

        public static bool DeleteExchangeRateById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteExchangeRateById(Id)).Result;
        }

        #endregion
        #region GSLTax

        public static GsltaxDTO CreateGSLTax(GsltaxDTO GSLTax)
        {
            return Task.Run(async () => await commonRequest.CreateGSLTax(GSLTax)).Result;
        }

        public static List<GsltaxDTO> SelectAllGSLTax()
        {
            return Task.Run(async () => await commonRequest.SelectAllGSLTax()).Result;
        }

        public static GsltaxDTO GetGSLTaxById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetGSLTaxById(Id)).Result;
        }

        public static GsltaxDTO UpdateGSLTax(GsltaxDTO GSLTax)
        {
            return Task.Run(async () => await commonRequest.UpdateGSLTax(GSLTax)).Result;
        }

        public static bool DeleteGSLTaxById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteGSLTaxById(Id)).Result;
        }

        #endregion
        #region GSLUser

        public static GsluserDTO CreateGSLUser(GsluserDTO GSLUser)
        {
            return Task.Run(async () => await commonRequest.CreateGSLUser(GSLUser)).Result;
        }

        public static List<GsluserDTO> SelectAllGSLUser()
        {
            return Task.Run(async () => await commonRequest.SelectAllGSLUser()).Result;
        }

        public static GsluserDTO GetGSLUserById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetGSLUserById(Id)).Result;
        }

        public static GsluserDTO UpdateGSLUser(GsluserDTO GSLUser)
        {
            return Task.Run(async () => await commonRequest.UpdateGSLUser(GSLUser)).Result;
        }

        public static bool DeleteGSLUserById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteGSLUserById(Id)).Result;
        }

        #endregion
        #region Holiday

        public static HolidayDTO CreateHoliday(HolidayDTO Holiday)
        {
            return Task.Run(async () => await commonRequest.CreateHoliday(Holiday)).Result;
        }

        public static List<HolidayDTO> SelectAllHoliday()
        {
            return Task.Run(async () => await commonRequest.SelectAllHoliday()).Result;
        }

        public static HolidayDTO GetHolidayById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetHolidayById(Id)).Result;
        }

        public static HolidayDTO UpdateHoliday(HolidayDTO Holiday)
        {
            return Task.Run(async () => await commonRequest.UpdateHoliday(Holiday)).Result;
        }

        public static bool DeleteHolidayById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteHolidayById(Id)).Result;
        }

        #endregion
        #region HolidayDefinition

        public static HolidayDefinitionDTO CreateHolidayDefinition(HolidayDefinitionDTO HolidayDefinition)
        {
            return Task.Run(async () => await commonRequest.CreateHolidayDefinition(HolidayDefinition)).Result;
        }

        public static List<HolidayDefinitionDTO> SelectAllHolidayDefinition()
        {
            return Task.Run(async () => await commonRequest.SelectAllHolidayDefinition()).Result;
        }

        public static HolidayDefinitionDTO GetHolidayDefinitionById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetHolidayDefinitionById(Id)).Result;
        }

        public static HolidayDefinitionDTO UpdateHolidayDefinition(HolidayDefinitionDTO HolidayDefinition)
        {
            return Task.Run(async () => await commonRequest.UpdateHolidayDefinition(HolidayDefinition)).Result;
        }

        public static bool DeleteHolidayDefinitionById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteHolidayDefinitionById(Id)).Result;
        }

        #endregion
        #region Language

        public static LanguageDTO CreateLanguage(LanguageDTO Language)
        {
            return Task.Run(async () => await commonRequest.CreateLanguage(Language)).Result;
        }

        public static List<LanguageDTO> SelectAllLanguage()
        {
            return Task.Run(async () => await commonRequest.SelectAllLanguage()).Result;
        }

        public static LanguageDTO GetLanguageById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetLanguageById(Id)).Result;
        }

        public static LanguageDTO UpdateLanguage(LanguageDTO Language)
        {
            return Task.Run(async () => await commonRequest.UpdateLanguage(Language)).Result;
        }

        public static bool DeleteLanguageById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteLanguageById(Id)).Result;
        }

        #endregion
        #region Location

        public static LocationDTO CreateLocation(LocationDTO Location)
        {
            return Task.Run(async () => await commonRequest.CreateLocation(Location)).Result;
        }

        public static List<LocationDTO> SelectAllLocation()
        {
            return Task.Run(async () => await commonRequest.SelectAllLocation()).Result;
        }

        public static LocationDTO GetLocationById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetLocationById(Id)).Result;
        }

        public static LocationDTO UpdateLocation(LocationDTO Location)
        {
            return Task.Run(async () => await commonRequest.UpdateLocation(Location)).Result;
        }

        public static bool DeleteLocationById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteLocationById(Id)).Result;
        }

        #endregion
        #region Lookup

        public static LookupDTO CreateLookup(LookupDTO Lookup)
        {
            return Task.Run(async () => await commonRequest.CreateLookup(Lookup)).Result;
        }

        public static List<LookupDTO> SelectAllLookup()
        {
            return Task.Run(async () => await commonRequest.SelectAllLookup()).Result;
        }

        public static LookupDTO GetLookupById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetLookupById(Id)).Result;
        }

        public static LookupDTO UpdateLookup(LookupDTO Lookup)
        {
            return Task.Run(async () => await commonRequest.UpdateLookup(Lookup)).Result;
        }

        public static bool DeleteLookupById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteLookupById(Id)).Result;
        }

        #endregion  
        #region ObjectState

        public static ObjectStateDTO CreateObjectState(ObjectStateDTO ObjectState)
        {
            return Task.Run(async () => await commonRequest.CreateObjectState(ObjectState)).Result;
        }

        public static List<ObjectStateDTO> SelectAllObjectState()
        {
            return Task.Run(async () => await commonRequest.SelectAllObjectState()).Result;
        }

        public static ObjectStateDTO GetObjectStateById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetObjectStateById(Id)).Result;
        }

        public static ObjectStateDTO UpdateObjectState(ObjectStateDTO ObjectState)
        {
            return Task.Run(async () => await commonRequest.UpdateObjectState(ObjectState)).Result;
        }

        public static bool DeleteObjectStateById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteObjectStateById(Id)).Result;
        }

        #endregion
        #region Period

        public static PeriodDTO CreatePeriod(PeriodDTO Period)
        {
            return Task.Run(async () => await commonRequest.CreatePeriod(Period)).Result;
        }

        public static List<PeriodDTO> SelectAllPeriod()
        {
            return Task.Run(async () => await commonRequest.SelectAllPeriod()).Result;
        }

        public static PeriodDTO GetPeriodById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetPeriodById(Id)).Result;
        }

        public static PeriodDTO UpdatePeriod(PeriodDTO Period)
        {
            return Task.Run(async () => await commonRequest.UpdatePeriod(Period)).Result;
        }

        public static bool DeletePeriodById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeletePeriodById(Id)).Result;
        }

        #endregion

        #region Relation

        public static RelationDTO CreateRelation(RelationDTO Relation)
        {
            return Task.Run(async () => await commonRequest.CreateRelation(Relation)).Result;
        }

        public static List<RelationDTO> SelectAllRelation()
        {
            return Task.Run(async () => await commonRequest.SelectAllRelation()).Result;
        }

        public static RelationDTO GetRelationById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetRelationById(Id)).Result;
        }

        public static RelationDTO UpdateRelation(RelationDTO Relation)
        {
            return Task.Run(async () => await commonRequest.UpdateRelation(Relation)).Result;
        }

        public static bool DeleteRelationById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteRelationById(Id)).Result;
        }

        #endregion 
        #region Route

        public static RouteDTO CreateRoute(RouteDTO Route)
        {
            return Task.Run(async () => await commonRequest.CreateRoute(Route)).Result;
        }

        public static List<RouteDTO> SelectAllRoute()
        {
            return Task.Run(async () => await commonRequest.SelectAllRoute()).Result;
        }

        public static RouteDTO GetRouteById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetRouteById(Id)).Result;
        }

        public static RouteDTO UpdateRoute(RouteDTO Route)
        {
            return Task.Run(async () => await commonRequest.UpdateRoute(Route)).Result;
        }

        public static bool DeleteRouteById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteRouteById(Id)).Result;
        }

        #endregion
        #region RouteAssignment

        public static RouteAssignmentDTO CreateRouteAssignment(RouteAssignmentDTO RouteAssignment)
        {
            return Task.Run(async () => await commonRequest.CreateRouteAssignment(RouteAssignment)).Result;
        }

        public static List<RouteAssignmentDTO> SelectAllRouteAssignment()
        {
            return Task.Run(async () => await commonRequest.SelectAllRouteAssignment()).Result;
        }

        public static RouteAssignmentDTO GetRouteAssignmentById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetRouteAssignmentById(Id)).Result;
        }

        public static RouteAssignmentDTO UpdateRouteAssignment(RouteAssignmentDTO RouteAssignment)
        {
            return Task.Run(async () => await commonRequest.UpdateRouteAssignment(RouteAssignment)).Result;
        }

        public static bool DeleteRouteAssignmentById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteRouteAssignmentById(Id)).Result;
        }

        #endregion
        #region Schedule

        public static ScheduleDTO CreateSchedule(ScheduleDTO Schedule)
        {
            return Task.Run(async () => await commonRequest.CreateSchedule(Schedule)).Result;
        }

        public static List<ScheduleDTO> SelectAllSchedule()
        {
            return Task.Run(async () => await commonRequest.SelectAllSchedule()).Result;
        }

        public static ScheduleDTO GetScheduleById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetScheduleById(Id)).Result;
        }

        public static ScheduleDTO UpdateSchedule(ScheduleDTO Schedule)
        {
            return Task.Run(async () => await commonRequest.UpdateSchedule(Schedule)).Result;
        }

        public static bool DeleteScheduleById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteScheduleById(Id)).Result;
        }

        #endregion
        #region ScheduleDetail

        public static ScheduleDetailDTO CreateScheduleDetail(ScheduleDetailDTO ScheduleDetail)
        {
            return Task.Run(async () => await commonRequest.CreateScheduleDetail(ScheduleDetail)).Result;
        }

        public static List<ScheduleDetailDTO> SelectAllScheduleDetail()
        {
            return Task.Run(async () => await commonRequest.SelectAllScheduleDetail()).Result;
        }

        public static ScheduleDetailDTO GetScheduleDetailById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetScheduleDetailById(Id)).Result;
        }

        public static ScheduleDetailDTO UpdateScheduleDetail(ScheduleDetailDTO ScheduleDetail)
        {
            return Task.Run(async () => await commonRequest.UpdateScheduleDetail(ScheduleDetail)).Result;
        }

        public static bool DeleteScheduleDetailById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteScheduleDetailById(Id)).Result;
        }

        #endregion
        #region ScheduleHeader

        public static ScheduleHeaderDTO CreateScheduleHeader(ScheduleHeaderDTO ScheduleHeader)
        {
            return Task.Run(async () => await commonRequest.CreateScheduleHeader(ScheduleHeader)).Result;
        }

        public static List<ScheduleHeaderDTO> SelectAllScheduleHeader()
        {
            return Task.Run(async () => await commonRequest.SelectAllScheduleHeader()).Result;
        }

        public static ScheduleHeaderDTO GetScheduleHeaderById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetScheduleHeaderById(Id)).Result;
        }

        public static ScheduleHeaderDTO UpdateScheduleHeader(ScheduleHeaderDTO ScheduleHeader)
        {
            return Task.Run(async () => await commonRequest.UpdateScheduleHeader(ScheduleHeader)).Result;
        }

        public static bool DeleteScheduleHeaderById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteScheduleHeaderById(Id)).Result;
        }

        #endregion
        #region SeasonalMessage

        public static SeasonalMessageDTO CreateSeasonalMessage(SeasonalMessageDTO SeasonalMessage)
        {
            return Task.Run(async () => await commonRequest.CreateSeasonalMessage(SeasonalMessage)).Result;
        }

        public static List<SeasonalMessageDTO> SelectAllSeasonalMessage()
        {
            return Task.Run(async () => await commonRequest.SelectAllSeasonalMessage()).Result;
        }

        public static SeasonalMessageDTO GetSeasonalMessageById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetSeasonalMessageById(Id)).Result;
        }

        public static SeasonalMessageDTO UpdateSeasonalMessage(SeasonalMessageDTO SeasonalMessage)
        {
            return Task.Run(async () => await commonRequest.UpdateSeasonalMessage(SeasonalMessage)).Result;
        }

        public static bool DeleteSeasonalMessageById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteSeasonalMessageById(Id)).Result;
        }

        #endregion 
        #region Space

        public static SpaceDTO CreateSpace(SpaceDTO Space)
        {
            return Task.Run(async () => await commonRequest.CreateSpace(Space)).Result;
        }

        public static List<SpaceDTO> SelectAllSpace()
        {
            return Task.Run(async () => await commonRequest.SelectAllSpace()).Result;
        }

        public static SpaceDTO GetSpaceById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetSpaceById(Id)).Result;
        }

        public static SpaceDTO UpdateSpace(SpaceDTO Space)
        {
            return Task.Run(async () => await commonRequest.UpdateSpace(Space)).Result;
        }

        public static bool DeleteSpaceById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteSpaceById(Id)).Result;
        }

        #endregion
        #region SpaceDirection

        public static SpaceDirectionDTO CreateSpaceDirection(SpaceDirectionDTO SpaceDirection)
        {
            return Task.Run(async () => await commonRequest.CreateSpaceDirection(SpaceDirection)).Result;
        }

        public static List<SpaceDirectionDTO> SelectAllSpaceDirection()
        {
            return Task.Run(async () => await commonRequest.SelectAllSpaceDirection()).Result;
        }

        public static SpaceDirectionDTO GetSpaceDirectionById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetSpaceDirectionById(Id)).Result;
        }

        public static SpaceDirectionDTO UpdateSpaceDirection(SpaceDirectionDTO SpaceDirection)
        {
            return Task.Run(async () => await commonRequest.UpdateSpaceDirection(SpaceDirection)).Result;
        }

        public static bool DeleteSpaceDirectionById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteSpaceDirectionById(Id)).Result;
        }

        #endregion
        #region SubCountry

        public static SubCountryDTO CreateSubCountry(SubCountryDTO SubCountry)
        {
            return Task.Run(async () => await commonRequest.CreateSubCountry(SubCountry)).Result;
        }

        public static List<SubCountryDTO> SelectAllSubCountry()
        {
            return Task.Run(async () => await commonRequest.SelectAllSubCountry()).Result;
        }

        public static SubCountryDTO GetSubCountryById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetSubCountryById(Id)).Result;
        }

        public static SubCountryDTO UpdateSubCountry(SubCountryDTO SubCountry)
        {
            return Task.Run(async () => await commonRequest.UpdateSubCountry(SubCountry)).Result;
        }

        public static bool DeleteSubCountryById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteSubCountryById(Id)).Result;
        }

        #endregion
        #region Subtitle

        public static SubtitleDTO CreateSubtitle(SubtitleDTO Subtitle)
        {
            return Task.Run(async () => await commonRequest.CreateSubtitle(Subtitle)).Result;
        }

        public static List<SubtitleDTO> SelectAllSubtitle()
        {
            return Task.Run(async () => await commonRequest.SelectAllSubtitle()).Result;
        }

        public static SubtitleDTO GetSubtitleById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetSubtitleById(Id)).Result;
        }

        public static SubtitleDTO UpdateSubtitle(SubtitleDTO Subtitle)
        {
            return Task.Run(async () => await commonRequest.UpdateSubtitle(Subtitle)).Result;
        }

        public static bool DeleteSubtitleById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteSubtitleById(Id)).Result;
        }

        #endregion
        #region ValueFactor

        public static ValueFactorDTO CreateValueFactor(ValueFactorDTO ValueFactor)
        {
            return Task.Run(async () => await commonRequest.CreateValueFactor(ValueFactor)).Result;
        }

        public static List<ValueFactorDTO> SelectAllValueFactor()
        {
            return Task.Run(async () => await commonRequest.SelectAllValueFactor()).Result;
        }

        public static ValueFactorDTO GetValueFactorById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetValueFactorById(Id)).Result;
        }

        public static ValueFactorDTO UpdateValueFactor(ValueFactorDTO ValueFactor)
        {
            return Task.Run(async () => await commonRequest.UpdateValueFactor(ValueFactor)).Result;
        }

        public static bool DeleteValueFactorById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteValueFactorById(Id)).Result;
        }

        #endregion
        #region VoucherExtension

        public static VoucherExtensionDefinitionDTO CreateVoucherExtension(VoucherExtensionDefinitionDTO VoucherExtension)
        {
            return Task.Run(async () => await commonRequest.CreateVoucherExtension(VoucherExtension)).Result;
        }

        public static List<VoucherExtensionDefinitionDTO> SelectAllVoucherExtension()
        {
            return Task.Run(async () => await commonRequest.SelectAllVoucherExtension()).Result;
        }

        public static VoucherExtensionDefinitionDTO GetVoucherExtensionById(int Id)
        {
            return Task.Run(async () => await commonRequest.GetVoucherExtensionById(Id)).Result;
        }

        public static VoucherExtensionDefinitionDTO UpdateVoucherExtension(VoucherExtensionDefinitionDTO VoucherExtension)
        {
            return Task.Run(async () => await commonRequest.UpdateVoucherExtension(VoucherExtension)).Result;
        }

        public static bool DeleteVoucherExtensionById(int Id)
        {
            return Task.Run(async () => await commonRequest.DeleteVoucherExtensionById(Id)).Result;
        }

        #endregion
        #endregion


        #region security

        #region AccessMatrix

        public static AccessMatrixDTO CreateAccessMatrix(AccessMatrixDTO AccessMatrix)
        {
            return Task.Run(async () => await securityRequest.CreateAccessMatrix(AccessMatrix)).Result;
        }

        public static List<AccessMatrixDTO> SelectAllAccessMatrix()
        {
            return Task.Run(async () => await securityRequest.SelectAllAccessMatrix()).Result;
        }

        public static AccessMatrixDTO GetAccessMatrixById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetAccessMatrixById(Id)).Result;
        }

        public static AccessMatrixDTO UpdateAccessMatrix(AccessMatrixDTO AccessMatrix)
        {
            return Task.Run(async () => await securityRequest.UpdateAccessMatrix(AccessMatrix)).Result;
        }

        public static bool DeleteAccessMatrixById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteAccessMatrixById(Id)).Result;
        }

        #endregion
        #region AcLog

        public static AcLogDTO CreateAcLog(AcLogDTO AcLog)
        {
            return Task.Run(async () => await securityRequest.CreateAcLog(AcLog)).Result;
        }

        public static List<AcLogDTO> SelectAllAcLog()
        {
            return Task.Run(async () => await securityRequest.SelectAllAcLog()).Result;
        }

        public static AcLogDTO GetAcLogById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetAcLogById(Id)).Result;
        }

        public static AcLogDTO UpdateAcLog(AcLogDTO AcLog)
        {
            return Task.Run(async () => await securityRequest.UpdateAcLog(AcLog)).Result;
        }

        public static bool DeleteAcLogById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteAcLogById(Id)).Result;
        }

        #endregion
        #region Card

        public static CardDTO CreateCard(CardDTO Card)
        {
            return Task.Run(async () => await securityRequest.CreateCard(Card)).Result;
        }

        public static List<CardDTO> SelectAllCard()
        {
            return Task.Run(async () => await securityRequest.SelectAllCard()).Result;
        }

        public static CardDTO GetCardById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetCardById(Id)).Result;
        }

        public static CardDTO UpdateCard(CardDTO Card)
        {
            return Task.Run(async () => await securityRequest.UpdateCard(Card)).Result;
        }

        public static bool DeleteCardById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteCardById(Id)).Result;
        }

        #endregion
        #region Functionality

        public static FunctionalityDTO CreateFunctionality(FunctionalityDTO Functionality)
        {
            return Task.Run(async () => await securityRequest.CreateFunctionality(Functionality)).Result;
        }

        public static List<FunctionalityDTO> SelectAllFunctionality()
        {
            return Task.Run(async () => await securityRequest.SelectAllFunctionality()).Result;
        }

        public static FunctionalityDTO GetFunctionalityById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetFunctionalityById(Id)).Result;
        }

        public static FunctionalityDTO UpdateFunctionality(FunctionalityDTO Functionality)
        {
            return Task.Run(async () => await securityRequest.UpdateFunctionality(Functionality)).Result;
        }

        public static bool DeleteFunctionalityById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteFunctionalityById(Id)).Result;
        }

        #endregion
        #region IssuedCard

        public static IssuedCardDTO CreateIssuedCard(IssuedCardDTO IssuedCard)
        {
            return Task.Run(async () => await securityRequest.CreateIssuedCard(IssuedCard)).Result;
        }

        public static List<IssuedCardDTO> SelectAllIssuedCard()
        {
            return Task.Run(async () => await securityRequest.SelectAllIssuedCard()).Result;
        }

        public static IssuedCardDTO GetIssuedCardById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetIssuedCardById(Id)).Result;
        }

        public static IssuedCardDTO UpdateIssuedCard(IssuedCardDTO IssuedCard)
        {
            return Task.Run(async () => await securityRequest.UpdateIssuedCard(IssuedCard)).Result;
        }

        public static bool DeleteIssuedCardById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteIssuedCardById(Id)).Result;
        }

        #endregion
        #region RoleActivity

        public static RoleActivityDTO CreateRoleActivity(RoleActivityDTO RoleActivity)
        {
            return Task.Run(async () => await securityRequest.CreateRoleActivity(RoleActivity)).Result;
        }

        public static List<RoleActivityDTO> SelectAllRoleActivity()
        {
            return Task.Run(async () => await securityRequest.SelectAllRoleActivity()).Result;
        }

        public static RoleActivityDTO GetRoleActivityById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetRoleActivityById(Id)).Result;
        }

        public static RoleActivityDTO UpdateRoleActivity(RoleActivityDTO RoleActivity)
        {
            return Task.Run(async () => await securityRequest.UpdateRoleActivity(RoleActivity)).Result;
        }

        public static bool DeleteRoleActivityById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteRoleActivityById(Id)).Result;
        }

        #endregion
        #region User

        public static UserDTO CreateUser(UserDTO User)
        {
            return Task.Run(async () => await securityRequest.CreateUser(User)).Result;
        }

        public static List<UserDTO> SelectAllUser()
        {
            return Task.Run(async () => await securityRequest.SelectAllUser()).Result;
        }

        public static UserDTO GetUserById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetUserById(Id)).Result;
        }

        public static ResponseModel<UserDTO> UpdateUser(UserUpdateDTO User)
        {
            return Task.Run(async () => await securityRequest.UpdateUser(User)).Result;
        }

        public static bool DeleteUserById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteUserById(Id)).Result;
        }

        #endregion
        #region UserRoleMapper

        public static UserRoleMapperDTO CreateUserRoleMapper(UserRoleMapperDTO UserRoleMapper)
        {
            return Task.Run(async () => await securityRequest.CreateUserRoleMapper(UserRoleMapper)).Result;
        }

        public static List<UserRoleMapperDTO> SelectAllUserRoleMapper()
        {
            try
            {
                return Task.Run(async () => await securityRequest.SelectAllUserRoleMapper()).Result;
            }
            catch (Exception io)
            {
                MessageBox.Show("Error:- SelectAllUserRoleMapper" + Environment.NewLine + io.Message);
                return new List<UserRoleMapperDTO>();
            }
        }

        public static UserRoleMapperDTO GetUserRoleMapperById(int Id)
        {
            return Task.Run(async () => await securityRequest.GetUserRoleMapperById(Id)).Result;
        }

        public static UserRoleMapperDTO UpdateUserRoleMapper(UserRoleMapperDTO UserRoleMapper)
        {
            return Task.Run(async () => await securityRequest.UpdateUserRoleMapper(UserRoleMapper)).Result;
        }

        public static bool DeleteUserRoleMapperById(int Id)
        {
            return Task.Run(async () => await securityRequest.DeleteUserRoleMapperById(Id)).Result;
        }

        #endregion
        #endregion


        #region pms Schema


        public static EventDetailDTO CreateEventDetail(EventDetailDTO EventDetail)
        {
            return Task.Run(async () => await pmsRequest.CreateEventDetail(EventDetail)).Result;
        }
        public static EventDetailDTO UpdateEventDetail(EventDetailDTO EventDetail)
        {
            return Task.Run(async () => await pmsRequest.UpdateEventDetail(EventDetail)).Result;
        }
        public static EventDetailDTO GetEventDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetEventDetailById(Id)).Result;
        }
        public static List<EventDetailDTO> GetEventDetailByIdlist(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetEventDetailByIdlist(Id)).Result;
        }
        #region RateAdjustment

        public static RateAdjustmentDTO CreateRateAdjustment(RateAdjustmentDTO RateAdjustment)
        {
            return Task.Run(async () => await pmsRequest.CreateRateAdjustment(RateAdjustment)).Result;
        }

        public static List<RateAdjustmentDTO> SelectAllRateAdjustment()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateAdjustment()).Result;
        }

        public static RateAdjustmentDTO GetRateAdjustmentById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateAdjustmentById(Id)).Result;
        }

        public static RateAdjustmentDTO UpdateRateAdjustment(RateAdjustmentDTO RateAdjustment)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateAdjustment(RateAdjustment)).Result;
        }

        public static bool DeleteRateAdjustmentById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateAdjustmentById(Id)).Result;
        }

        #endregion

        #region AvailabilityCalendar

        public static AvailabilityCalendarDTO CreateAvailabilityCalendar(AvailabilityCalendarDTO AvailabilityCalendar)
        {
            return Task.Run(async () => await pmsRequest.CreateAvailabilityCalendar(AvailabilityCalendar)).Result;
        }

        public static List<AvailabilityCalendarDTO> SelectAllAvailabilityCalendar()
        {
            return Task.Run(async () => await pmsRequest.SelectAllAvailabilityCalendar()).Result;
        }

        public static AvailabilityCalendarDTO GetAvailabilityCalendarById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetAvailabilityCalendarById(Id)).Result;
        }

        public static AvailabilityCalendarDTO UpdateAvailabilityCalendar(AvailabilityCalendarDTO AvailabilityCalendar)
        {
            return Task.Run(async () => await pmsRequest.UpdateAvailabilityCalendar(AvailabilityCalendar)).Result;
        }

        public static bool DeleteAvailabilityCalendarById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteAvailabilityCalendarById(Id)).Result;
        }

        #endregion
        #region Block

        public static BlockDTO CreateBlock(BlockDTO Block)
        {
            return Task.Run(async () => await pmsRequest.CreateBlock(Block)).Result;
        }

        public static List<BlockDTO> SelectAllBlock()
        {
            return Task.Run(async () => await pmsRequest.SelectAllBlock()).Result;
        }

        public static BlockDTO GetBlockById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetBlockById(Id)).Result;
        }

        public static BlockDTO UpdateBlock(BlockDTO Block)
        {
            return Task.Run(async () => await pmsRequest.UpdateBlock(Block)).Result;
        }

        public static bool DeleteBlockById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteBlockById(Id)).Result;
        }

        #endregion
        #region DailyRateChargeVoucher

        public static DailyRateChargeVoucherDTO CreateDailyRateChargeVoucher(DailyRateChargeVoucherDTO DailyRateChargeVoucher)
        {
            return Task.Run(async () => await pmsRequest.CreateDailyRateChargeVoucher(DailyRateChargeVoucher)).Result;
        }

        public static List<DailyRateChargeVoucherDTO> SelectAllDailyRateChargeVoucher()
        {
            return Task.Run(async () => await pmsRequest.SelectAllDailyRateChargeVoucher()).Result;
        }

        public static DailyRateChargeVoucherDTO GetDailyRateChargeVoucherById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetDailyRateChargeVoucherById(Id)).Result;
        }

        public static DailyRateChargeVoucherDTO UpdateDailyRateChargeVoucher(DailyRateChargeVoucherDTO DailyRateChargeVoucher)
        {
            return Task.Run(async () => await pmsRequest.UpdateDailyRateChargeVoucher(DailyRateChargeVoucher)).Result;
        }

        public static bool DeleteDailyRateChargeVoucherById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteDailyRateChargeVoucherById(Id)).Result;
        }

        #endregion
        #region DailyResidentSummary

        public static DailyResidentSummaryDTO CreateDailyResidentSummary(DailyResidentSummaryDTO DailyResidentSummary)
        {
            return Task.Run(async () => await pmsRequest.CreateDailyResidentSummary(DailyResidentSummary)).Result;
        }

        public static List<DailyResidentSummaryDTO> SelectAllDailyResidentSummary()
        {
            return Task.Run(async () => await pmsRequest.SelectAllDailyResidentSummary()).Result;
        }

        public static DailyResidentSummaryDTO GetDailyResidentSummaryById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetDailyResidentSummaryById(Id)).Result;
        }

        public static DailyResidentSummaryDTO UpdateDailyResidentSummary(DailyResidentSummaryDTO DailyResidentSummary)
        {
            return Task.Run(async () => await pmsRequest.UpdateDailyResidentSummary(DailyResidentSummary)).Result;
        }

        public static bool DeleteDailyResidentSummaryById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteDailyResidentSummaryById(Id)).Result;
        }

        #endregion
        #region Discrepancy

        public static DiscrepancyDTO CreateDiscrepancy(DiscrepancyDTO Discrepancy)
        {
            return Task.Run(async () => await pmsRequest.CreateDiscrepancy(Discrepancy)).Result;
        }

        public static List<DiscrepancyDTO> SelectAllDiscrepancy()
        {
            return Task.Run(async () => await pmsRequest.SelectAllDiscrepancy()).Result;
        }

        public static DiscrepancyDTO GetDiscrepancyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetDiscrepancyById(Id)).Result;
        }

        public static DiscrepancyDTO UpdateDiscrepancy(DiscrepancyDTO Discrepancy)
        {
            return Task.Run(async () => await pmsRequest.UpdateDiscrepancy(Discrepancy)).Result;
        }

        public static bool DeleteDiscrepancyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteDiscrepancyById(Id)).Result;
        }

        #endregion
        #region HKAssignment

        public static HkassignmentDTO CreateHKAssignment(HkassignmentDTO HKAssignment)
        {
            return Task.Run(async () => await pmsRequest.CreateHKAssignment(HKAssignment)).Result;
        }

        public static List<HkassignmentDTO> SelectAllHKAssignment()
        {
            return Task.Run(async () => await pmsRequest.SelectAllHKAssignment()).Result;
        }

        public static HkassignmentDTO GetHKAssignmentById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetHKAssignmentById(Id)).Result;
        }

        public static HkassignmentDTO UpdateHKAssignment(HkassignmentDTO HKAssignment)
        {
            return Task.Run(async () => await pmsRequest.UpdateHKAssignment(HKAssignment)).Result;
        }

        public static bool DeleteHKAssignmentById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteHKAssignmentById(Id)).Result;
        }

        #endregion
        #region HKValue

        public static HkvalueDTO CreateHKValue(HkvalueDTO HKValue)
        {
            return Task.Run(async () => await pmsRequest.CreateHKValue(HKValue)).Result;
        }

        public static List<HkvalueDTO> SelectAllHKValue()
        {
            return Task.Run(async () => await pmsRequest.SelectAllHKValue()).Result;
        }

        public static HkvalueDTO GetHKValueById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetHKValueById(Id)).Result;
        }

        public static HkvalueDTO UpdateHKValue(HkvalueDTO HKValue)
        {
            return Task.Run(async () => await pmsRequest.UpdateHKValue(HKValue)).Result;
        }

        public static bool DeleteHKValueById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteHKValueById(Id)).Result;
        }

        #endregion
        #region KeyDefinition

        public static KeyDefinitionDTO CreateKeyDefinition(KeyDefinitionDTO KeyDefinition)
        {
            return Task.Run(async () => await pmsRequest.CreateKeyDefinition(KeyDefinition)).Result;
        }

        public static List<KeyDefinitionDTO> SelectAllKeyDefinition()
        {
            return Task.Run(async () => await pmsRequest.SelectAllKeyDefinition()).Result;
        }

        public static KeyDefinitionDTO GetKeyDefinitionById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetKeyDefinitionById(Id)).Result;
        }

        public static KeyDefinitionDTO UpdateKeyDefinition(KeyDefinitionDTO KeyDefinition)
        {
            return Task.Run(async () => await pmsRequest.UpdateKeyDefinition(KeyDefinition)).Result;
        }

        public static bool DeleteKeyDefinitionById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteKeyDefinitionById(Id)).Result;
        }

        #endregion
        #region KeyOption

        public static KeyOptionDTO CreateKeyOption(KeyOptionDTO KeyOption)
        {
            return Task.Run(async () => await pmsRequest.CreateKeyOption(KeyOption)).Result;
        }

        public static List<KeyOptionDTO> SelectAllKeyOption()
        {
            return Task.Run(async () => await pmsRequest.SelectAllKeyOption()).Result;
        }

        public static KeyOptionDTO GetKeyOptionById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetKeyOptionById(Id)).Result;
        }

        public static KeyOptionDTO UpdateKeyOption(KeyOptionDTO KeyOption)
        {
            return Task.Run(async () => await pmsRequest.UpdateKeyOption(KeyOption)).Result;
        }

        public static bool DeleteKeyOptionById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteKeyOptionById(Id)).Result;
        }

        #endregion
        #region NegotiationRate

        public static NegotiationRateDTO CreateNegotiationRate(NegotiationRateDTO NegotiationRate)
        {
            return Task.Run(async () => await pmsRequest.CreateNegotiationRate(NegotiationRate)).Result;
        }

        public static List<NegotiationRateDTO> SelectAllNegotiationRate()
        {
            return Task.Run(async () => await pmsRequest.SelectAllNegotiationRate()).Result;
        }

        public static NegotiationRateDTO GetNegotiationRateById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetNegotiationRateById(Id)).Result;
        }

        public static NegotiationRateDTO UpdateNegotiationRate(NegotiationRateDTO NegotiationRate)
        {
            return Task.Run(async () => await pmsRequest.UpdateNegotiationRate(NegotiationRate)).Result;
        }

        public static bool DeleteNegotiationRateById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteNegotiationRateById(Id)).Result;
        }

        #endregion
        #region PackageDetail

        public static PackageDetailDTO CreatePackageDetail(PackageDetailDTO PackageDetail)
        {
            return Task.Run(async () => await pmsRequest.CreatePackageDetail(PackageDetail)).Result;
        }

        public static List<PackageDetailDTO> SelectAllPackageDetail()
        {
            return Task.Run(async () => await pmsRequest.SelectAllPackageDetail()).Result;
        }

        public static PackageDetailDTO GetPackageDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetPackageDetailById(Id)).Result;
        }

        public static PackageDetailDTO UpdatePackageDetail(PackageDetailDTO PackageDetail)
        {
            return Task.Run(async () => await pmsRequest.UpdatePackageDetail(PackageDetail)).Result;
        }

        public static bool DeletePackageDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeletePackageDetailById(Id)).Result;
        }

        #endregion
        #region PackageHeader

        public static PackageHeaderDTO CreatePackageHeader(PackageHeaderDTO PackageHeader)
        {
            return Task.Run(async () => await pmsRequest.CreatePackageHeader(PackageHeader)).Result;
        }

        public static List<PackageHeaderDTO> SelectAllPackageHeader()
        {
            return Task.Run(async () => await pmsRequest.SelectAllPackageHeader()).Result;
        }

        public static PackageHeaderDTO GetPackageHeaderById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetPackageHeaderById(Id)).Result;
        }

        public static PackageHeaderDTO UpdatePackageHeader(PackageHeaderDTO PackageHeader)
        {
            return Task.Run(async () => await pmsRequest.UpdatePackageHeader(PackageHeader)).Result;
        }

        public static bool DeletePackageHeaderById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeletePackageHeaderById(Id)).Result;
        }

        #endregion
        #region PackagesToPost

        public static PackagesToPostDTO CreatePackagesToPost(PackagesToPostDTO PackagesToPost)
        {
            return Task.Run(async () => await pmsRequest.CreatePackagesToPost(PackagesToPost)).Result;
        }

        public static List<PackagesToPostDTO> SelectAllPackagesToPost()
        {
            return Task.Run(async () => await pmsRequest.SelectAllPackagesToPost()).Result;
        }

        public static PackagesToPostDTO GetPackagesToPostById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetPackagesToPostById(Id)).Result;
        }

        public static PackagesToPostDTO UpdatePackagesToPost(PackagesToPostDTO PackagesToPost)
        {
            return Task.Run(async () => await pmsRequest.UpdatePackagesToPost(PackagesToPost)).Result;
        }

        public static bool DeletePackagesToPostById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeletePackagesToPostById(Id)).Result;
        }

        #endregion
        #region PostingSchedule

        public static PostingScheduleDTO CreatePostingSchedule(PostingScheduleDTO PostingSchedule)
        {
            return Task.Run(async () => await pmsRequest.CreatePostingSchedule(PostingSchedule)).Result;
        }

        public static List<PostingScheduleDTO> SelectAllPostingSchedule()
        {
            return Task.Run(async () => await pmsRequest.SelectAllPostingSchedule()).Result;
        }

        public static PostingScheduleDTO GetPostingScheduleById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetPostingScheduleById(Id)).Result;
        }

        public static PostingScheduleDTO UpdatePostingSchedule(PostingScheduleDTO PostingSchedule)
        {
            return Task.Run(async () => await pmsRequest.UpdatePostingSchedule(PostingSchedule)).Result;
        }

        public static bool DeletePostingScheduleById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeletePostingScheduleById(Id)).Result;
        }

        #endregion
        #region Rate

        public static RateDTO CreateRate(RateDTO Rate)
        {
            return Task.Run(async () => await pmsRequest.CreateRate(Rate)).Result;
        }

        public static List<RateDTO> SelectAllRate()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRate()).Result;
        }

        public static RateDTO GetRateById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateById(Id)).Result;
        }

        public static RateDTO UpdateRate(RateDTO Rate)
        {
            return Task.Run(async () => await pmsRequest.UpdateRate(Rate)).Result;
        }

        public static bool DeleteRateById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateById(Id)).Result;
        }

        #endregion
        #region RateCategory

        public static RateCategoryDTO CreateRateCategory(RateCategoryDTO RateCategory)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCategory(RateCategory)).Result;
        }

        public static List<RateCategoryDTO> SelectAllRateCategory()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCategory()).Result;
        }

        public static RateCategoryDTO GetRateCategoryById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCategoryById(Id)).Result;
        }

        public static RateCategoryDTO UpdateRateCategory(RateCategoryDTO RateCategory)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCategory(RateCategory)).Result;
        }

        public static bool DeleteRateCategoryById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCategoryById(Id)).Result;
        }

        #endregion
        #region RateCategoryRateStrategy

        public static RateCategoryRateStrategyDTO CreateRateCategoryRateStrategy(RateCategoryRateStrategyDTO RateCategoryRateStrategy)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCategoryRateStrategy(RateCategoryRateStrategy)).Result;
        }

        public static List<RateCategoryRateStrategyDTO> SelectAllRateCategoryRateStrategy()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCategoryRateStrategy()).Result;
        }

        public static RateCategoryRateStrategyDTO GetRateCategoryRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCategoryRateStrategyById(Id)).Result;
        }

        public static RateCategoryRateStrategyDTO UpdateRateCategoryRateStrategy(RateCategoryRateStrategyDTO RateCategoryRateStrategy)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCategoryRateStrategy(RateCategoryRateStrategy)).Result;
        }

        public static bool DeleteRateCategoryRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCategoryRateStrategyById(Id)).Result;
        }

        #endregion
        #region RateCodeDetail

        public static RateCodeDetailDTO CreateRateCodeDetail(RateCodeDetailDTO RateCodeDetail)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCodeDetail(RateCodeDetail)).Result;
        }

        public static List<RateCodeDetailDTO> SelectAllRateCodeDetail()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCodeDetail()).Result;
        }

        public static RateCodeDetailDTO GetRateCodeDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeDetailById(Id)).Result;
        }

        public static RateCodeDetailDTO UpdateRateCodeDetail(RateCodeDetailDTO RateCodeDetail)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCodeDetail(RateCodeDetail)).Result;
        }

        public static bool DeleteRateCodeDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCodeDetailById(Id)).Result;
        }

        #endregion
        #region RateCodeDetailGuestCount

        public static RateCodeDetailGuestCountDTO CreateRateCodeDetailGuestCount(RateCodeDetailGuestCountDTO RateCodeDetailGuestCount)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCodeDetailGuestCount(RateCodeDetailGuestCount)).Result;
        }

        public static List<RateCodeDetailGuestCountDTO> SelectAllRateCodeDetailGuestCount()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCodeDetailGuestCount()).Result;
        }

        public static RateCodeDetailGuestCountDTO GetRateCodeDetailGuestCountById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeDetailGuestCountById(Id)).Result;
        }

        public static RateCodeDetailGuestCountDTO UpdateRateCodeDetailGuestCount(RateCodeDetailGuestCountDTO RateCodeDetailGuestCount)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCodeDetailGuestCount(RateCodeDetailGuestCount)).Result;
        }

        public static bool DeleteRateCodeDetailGuestCountById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCodeDetailGuestCountById(Id)).Result;
        }

        #endregion
        #region RateCodeDetailRoomType

        public static RateCodeDetailRoomTypeDTO CreateRateCodeDetailRoomType(RateCodeDetailRoomTypeDTO RateCodeDetailRoomType)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCodeDetailRoomType(RateCodeDetailRoomType)).Result;
        }

        public static List<RateCodeDetailRoomTypeDTO> SelectAllRateCodeDetailRoomType()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCodeDetailRoomType()).Result;
        }

        public static RateCodeDetailRoomTypeDTO GetRateCodeDetailRoomTypeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeDetailRoomTypeById(Id)).Result;
        }

        public static RateCodeDetailRoomTypeDTO UpdateRateCodeDetailRoomType(RateCodeDetailRoomTypeDTO RateCodeDetailRoomType)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCodeDetailRoomType(RateCodeDetailRoomType)).Result;
        }

        public static bool DeleteRateCodeDetailRoomTypeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCodeDetailRoomTypeById(Id)).Result;
        }

        #endregion
        #region RateCodeHeader

        public static RateCodeHeaderDTO CreateRateCodeHeader(RateCodeHeaderDTO RateCodeHeader)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCodeHeader(RateCodeHeader)).Result;
        }

        public static List<RateCodeHeaderDTO> SelectAllRateCodeHeader()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCodeHeader()).Result;
        }

        public static RateCodeHeaderDTO GetRateCodeHeaderById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeHeaderById(Id)).Result;
        }

        public static RateCodeHeaderDTO UpdateRateCodeHeader(RateCodeHeaderDTO RateCodeHeader)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCodeHeader(RateCodeHeader)).Result;
        }

        public static bool DeleteRateCodeHeaderById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCodeHeaderById(Id)).Result;
        }

        #endregion
        #region RateCodePackage

        public static RateCodePackageDTO CreateRateCodePackage(RateCodePackageDTO RateCodePackage)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCodePackage(RateCodePackage)).Result;
        }

        public static List<RateCodePackageDTO> SelectAllRateCodePackage()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCodePackage()).Result;
        }

        public static RateCodePackageDTO GetRateCodePackageById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodePackageById(Id)).Result;
        }

        public static RateCodePackageDTO UpdateRateCodePackage(RateCodePackageDTO RateCodePackage)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCodePackage(RateCodePackage)).Result;
        }

        public static bool DeleteRateCodePackageById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCodePackageById(Id)).Result;
        }

        #endregion
        #region RateCodeRateStrategy

        public static RateCodeRateStrategyDTO CreateRateCodeRateStrategy(RateCodeRateStrategyDTO RateCodeRateStrategy)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCodeRateStrategy(RateCodeRateStrategy)).Result;
        }

        public static List<RateCodeRateStrategyDTO> SelectAllRateCodeRateStrategy()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCodeRateStrategy()).Result;
        }

        public static RateCodeRateStrategyDTO GetRateCodeRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeRateStrategyById(Id)).Result;
        }

        public static RateCodeRateStrategyDTO UpdateRateCodeRateStrategy(RateCodeRateStrategyDTO RateCodeRateStrategy)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCodeRateStrategy(RateCodeRateStrategy)).Result;
        }
        public static List<RateCodeRateStrategyDTO> GetRateCodeRateStrategyByrateCode(int rateCode)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeRateStrategyByrateCode(rateCode)).Result;
        }

        public static bool DeleteRateCodeRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCodeRateStrategyById(Id)).Result;
        }

        #endregion
        #region RateCodeRoomType

        public static RateCodeRoomTypeDTO CreateRateCodeRoomType(RateCodeRoomTypeDTO RateCodeRoomType)
        {
            return Task.Run(async () => await pmsRequest.CreateRateCodeRoomType(RateCodeRoomType)).Result;
        }

        public static List<RateCodeRoomTypeDTO> SelectAllRateCodeRoomType()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateCodeRoomType()).Result;
        }

        public static RateCodeRoomTypeDTO GetRateCodeRoomTypeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeRoomTypeById(Id)).Result;
        }

        public static RateCodeRoomTypeDTO UpdateRateCodeRoomType(RateCodeRoomTypeDTO RateCodeRoomType)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateCodeRoomType(RateCodeRoomType)).Result;
        }

        public static bool DeleteRateCodeRoomTypeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateCodeRoomTypeById(Id)).Result;
        }

        #endregion
        #region RateComponent

        public static RateComponentDTO CreateRateComponent(RateComponentDTO RateComponent)
        {
            return Task.Run(async () => await pmsRequest.CreateRateComponent(RateComponent)).Result;
        }

        public static List<RateComponentDTO> SelectAllRateComponent()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateComponent()).Result;
        }

        public static RateComponentDTO GetRateComponentById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateComponentById(Id)).Result;
        }

        public static RateComponentDTO UpdateRateComponent(RateComponentDTO RateComponent)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateComponent(RateComponent)).Result;
        }

        public static bool DeleteRateComponentById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateComponentById(Id)).Result;
        }

        public static List<RateComponentDTO> GetRateComponentByrateCode(int rateCode)
        {
            return Task.Run(async () => await pmsRequest.GetRateComponentByrateCode(rateCode)).Result;
        }



        #endregion
        #region RateStrategy

        public static RateStrategyDTO CreateRateStrategy(RateStrategyDTO RateStrategy)
        {
            return Task.Run(async () => await pmsRequest.CreateRateStrategy(RateStrategy)).Result;
        }

        public static List<RateStrategyDTO> SelectAllRateStrategy()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRateStrategy()).Result;
        }

        public static RateStrategyDTO GetRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateStrategyById(Id)).Result;
        }

        public static RateStrategyDTO UpdateRateStrategy(RateStrategyDTO RateStrategy)
        {
            return Task.Run(async () => await pmsRequest.UpdateRateStrategy(RateStrategy)).Result;
        }

        public static bool DeleteRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRateStrategyById(Id)).Result;
        }

        #endregion
        #region RegistrationDetail

        public static RegistrationDetailDTO CreateRegistrationDetail(RegistrationDetailDTO RegistrationDetail)
        {
            return Task.Run(async () => await pmsRequest.CreateRegistrationDetail(RegistrationDetail)).Result;
        }

        public static List<RegistrationDetailDTO> SelectAllRegistrationDetail()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRegistrationDetail()).Result;
        }

        public static RegistrationDetailDTO GetRegistrationDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRegistrationDetailById(Id)).Result;
        }

        public static RegistrationDetailDTO UpdateRegistrationDetail(RegistrationDetailDTO RegistrationDetail)
        {
            return Task.Run(async () => await pmsRequest.UpdateRegistrationDetail(RegistrationDetail)).Result;
        }

        public static bool DeleteRegistrationDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRegistrationDetailById(Id)).Result;
        }

        #endregion
        #region RegistrationPrivllege

        public static RegistrationPrivllegeDTO CreateRegistrationPrivllege(RegistrationPrivllegeDTO RegistrationPrivllege)
        {
            return Task.Run(async () => await pmsRequest.CreateRegistrationPrivllege(RegistrationPrivllege)).Result;
        }

        public static List<RegistrationPrivllegeDTO> SelectAllRegistrationPrivllege()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRegistrationPrivllege()).Result;
        }

        public static RegistrationPrivllegeDTO GetRegistrationPrivllegeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRegistrationPrivllegeById(Id)).Result;
        }

        public static RegistrationPrivllegeDTO UpdateRegistrationPrivllege(RegistrationPrivllegeDTO RegistrationPrivllege)
        {
            return Task.Run(async () => await pmsRequest.UpdateRegistrationPrivllege(RegistrationPrivllege)).Result;
        }

        public static bool DeleteRegistrationPrivllegeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRegistrationPrivllegeById(Id)).Result;
        }

        #endregion
        #region ReportHistory

        public static ReportHistoryDTO CreateReportHistory(ReportHistoryDTO ReportHistory)
        {
            return null;// Task.Run(async () => await pmsRequest.createre(ReportHistory)).Result;
        }

        public static List<ReportHistoryDTO> SelectAllReportHistory()
        {
            return null;// Task.Run(async () => await pmsRequest.SelectAllReportHistory()).Result;
        }

        public static ReportHistoryDTO GetReportHistoryById(int Id)
        {
            return null;// Task.Run(async () => await pmsRequest.GetReportHistoryById(Id)).Result;
        }

        public static ReportHistoryDTO UpdateReportHistory(ReportHistoryDTO ReportHistory)
        {
            return null;// Task.Run(async () => await pmsRequest.UpdateReportHistory(ReportHistory)).Result;
        }

        public static bool DeleteReportHistoryById(int Id)
        {
            return false;// Task.Run(async () => await pmsRequest.DeleteReportHistoryById(Id)).Result;
        }

        #endregion
        #region RoomDetail

        public static RoomDetailDTO CreateRoomDetail(RoomDetailDTO RoomDetail)
        {
            return Task.Run(async () => await pmsRequest.CreateRoomDetail(RoomDetail)).Result;
        }

        public static List<RoomDetailDTO> SelectAllRoomDetail()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRoomDetail()).Result;
        }

        public static RoomDetailDTO GetRoomDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRoomDetailById(Id)).Result;
        }

        public static RoomDetailDTO UpdateRoomDetail(RoomDetailDTO RoomDetail)
        {
            return Task.Run(async () => await pmsRequest.UpdateRoomDetail(RoomDetail)).Result;
        }

        public static bool DeleteRoomDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRoomDetailById(Id)).Result;
        }

        #endregion
        #region RoomFeature

        public static RoomFeatureDTO CreateRoomFeature(RoomFeatureDTO RoomFeature)
        {
            return Task.Run(async () => await pmsRequest.CreateRoomFeatures(RoomFeature)).Result;
        }

        public static List<RoomFeatureDTO> SelectAllRoomFeature()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRoomFeatures()).Result;
        }

        public static RoomFeatureDTO GetRoomFeatureById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRoomFeaturesById(Id)).Result;
        }

        public static RoomFeatureDTO UpdateRoomFeature(RoomFeatureDTO RoomFeature)
        {
            return Task.Run(async () => await pmsRequest.UpdateRoomFeatures(RoomFeature)).Result;
        }

        public static bool DeleteRoomFeatureById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRoomFeaturesById(Id)).Result;
        }

        #endregion
        #region RoomType

        public static RoomTypeDTO CreateRoomType(RoomTypeDTO RoomType)
        {
            return Task.Run(async () => await pmsRequest.CreateRoomType(RoomType)).Result;
        }

        public static List<RoomTypeDTO> SelectAllRoomType()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRoomType()).Result;
        }

        public static RoomTypeDTO GetRoomTypeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRoomTypeById(Id)).Result;
        }

        public static RoomTypeDTO UpdateRoomType(RoomTypeDTO RoomType)
        {
            return Task.Run(async () => await pmsRequest.UpdateRoomType(RoomType)).Result;
        }

        public static bool DeleteRoomTypeById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRoomTypeById(Id)).Result;
        }

        #endregion
        #region RoomTypeRateStrategy

        public static RoomTypeRateStrategyDTO CreateRoomTypeRateStrategy(RoomTypeRateStrategyDTO RoomTypeRateStrategy)
        {
            return Task.Run(async () => await pmsRequest.CreateRoomTypeRateStrategy(RoomTypeRateStrategy)).Result;
        }

        public static List<RoomTypeRateStrategyDTO> SelectAllRoomTypeRateStrategy()
        {
            return Task.Run(async () => await pmsRequest.SelectAllRoomTypeRateStrategy()).Result;
        }

        public static RoomTypeRateStrategyDTO GetRoomTypeRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRoomTypeRateStrategyById(Id)).Result;
        }

        public static RoomTypeRateStrategyDTO UpdateRoomTypeRateStrategy(RoomTypeRateStrategyDTO RoomTypeRateStrategy)
        {
            return Task.Run(async () => await pmsRequest.UpdateRoomTypeRateStrategy(RoomTypeRateStrategy)).Result;
        }

        public static bool DeleteRoomTypeRateStrategyById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteRoomTypeRateStrategyById(Id)).Result;
        }

        #endregion
        #region TravelDetail

        public static TravelDetailDTO CreateTravelDetail(TravelDetailDTO TravelDetail)
        {
            return Task.Run(async () => await pmsRequest.CreateTravelDetail(TravelDetail)).Result;
        }

        public static List<TravelDetailDTO> SelectAllTravelDetail()
        {
            return Task.Run(async () => await pmsRequest.SelectAllTravelDetail()).Result;
        }

        public static TravelDetailDTO GetTravelDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetTravelDetailById(Id)).Result;
        }

        public static TravelDetailDTO UpdateTravelDetail(TravelDetailDTO TravelDetail)
        {
            return Task.Run(async () => await pmsRequest.UpdateTravelDetail(TravelDetail)).Result;
        }

        public static bool DeleteTravelDetailById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteTravelDetailById(Id)).Result;
        }

        #endregion
        #region Turndown

        public static TurndownDTO CreateTurndown(TurndownDTO Turndown)
        {
            return Task.Run(async () => await pmsRequest.CreateTurndown(Turndown)).Result;
        }

        public static List<TurndownDTO> SelectAllTurndown()
        {
            return Task.Run(async () => await pmsRequest.SelectAllTurndown()).Result;
        }

        public static TurndownDTO GetTurndownById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetTurndownById(Id)).Result;
        }

        public static TurndownDTO UpdateTurndown(TurndownDTO Turndown)
        {
            return Task.Run(async () => await pmsRequest.UpdateTurndown(Turndown)).Result;
        }

        public static bool DeleteTurndownById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteTurndownById(Id)).Result;
        }

        #endregion
        #region WeekDay

        public static WeekDayDTO CreateWeekDay(WeekDayDTO WeekDay)
        {
            return Task.Run(async () => await pmsRequest.CreateWeekDays(WeekDay)).Result;
        }

        public static List<WeekDayDTO> SelectAllWeekDay()
        {
            return Task.Run(async () => await pmsRequest.SelectAllWeekDays()).Result;
        }

        public static WeekDayDTO GetWeekDayById(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetWeekDaysById(Id)).Result;
        }

        public static WeekDayDTO UpdateWeekDay(WeekDayDTO WeekDay)
        {
            return Task.Run(async () => await pmsRequest.UpdateWeekDays(WeekDay)).Result;
        }

        public static bool DeleteWeekDayById(int Id)
        {
            return Task.Run(async () => await pmsRequest.DeleteWeekDaysById(Id)).Result;
        }
      

        #endregion
        #endregion


        #region article Schema
        #region Article

        public static ArticleDTO CreateArticle(ArticleDTO Article)
        {
            return Task.Run(async () => await articleRequest.CreateArticle(Article)).Result;
        }

        public static List<ArticleDTO> SelectAllArticle()
        {
            return Task.Run(async () => await articleRequest.SelectAllArticle()).Result;
        }

        public static ArticleDTO GetArticleById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetArticleById(Id)).Result;
        }

        public static ArticleDTO UpdateArticle(ArticleDTO Article)
        {
            return Task.Run(async () => await articleRequest.UpdateArticle(Article)).Result;
        }

        public static bool DeleteArticleById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteArticleById(Id)).Result;
        }

        #endregion
        #region ArticleUser

        public static ArticleUserDTO CreateArticleUser(ArticleUserDTO ArticleUser)
        {
            return Task.Run(async () => await articleRequest.CreateArticleUser(ArticleUser)).Result;
        }

        public static List<ArticleUserDTO> SelectAllArticleUser()
        {
            return Task.Run(async () => await articleRequest.SelectAllArticleUser()).Result;
        }

        public static ArticleUserDTO GetArticleUserById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetArticleUserById(Id)).Result;
        }

        public static ArticleUserDTO UpdateArticleUser(ArticleUserDTO ArticleUser)
        {
            return Task.Run(async () => await articleRequest.UpdateArticleUser(ArticleUser)).Result;
        }

        public static bool DeleteArticleUserById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteArticleUserById(Id)).Result;
        }

        #endregion
        #region ConversionDefinition

        public static ConversionDefinitionDTO CreateConversionDefinition(ConversionDefinitionDTO ConversionDefinition)
        {
            return Task.Run(async () => await articleRequest.CreateConversionDefinition(ConversionDefinition)).Result;
        }

        public static List<ConversionDefinitionDTO> SelectAllConversionDefinition()
        {
            return Task.Run(async () => await articleRequest.SelectAllConversionDefinition()).Result;
        }

        public static ConversionDefinitionDTO GetConversionDefinitionById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetConversionDefinitionById(Id)).Result;
        }

        public static ConversionDefinitionDTO UpdateConversionDefinition(ConversionDefinitionDTO ConversionDefinition)
        {
            return Task.Run(async () => await articleRequest.UpdateConversionDefinition(ConversionDefinition)).Result;
        }

        public static bool DeleteConversionDefinitionById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteConversionDefinitionById(Id)).Result;
        }

        #endregion
        #region SerialDefinition

        public static SerialDefinitionDTO CreateSerialDefinition(SerialDefinitionDTO SerialDefinition)
        {
            return Task.Run(async () => await articleRequest.CreateSerialDefinition(SerialDefinition)).Result;
        }

        public static List<SerialDefinitionDTO> SelectAllSerialDefinition()
        {
            return Task.Run(async () => await articleRequest.SelectAllSerialDefinition()).Result;
        }

        public static SerialDefinitionDTO GetSerialDefinitionById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetSerialDefinitionById(Id)).Result;
        }

        public static SerialDefinitionDTO UpdateSerialDefinition(SerialDefinitionDTO SerialDefinition)
        {
            return Task.Run(async () => await articleRequest.UpdateSerialDefinition(SerialDefinition)).Result;
        }

        public static bool DeleteSerialDefinitionById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteSerialDefinitionById(Id)).Result;
        }

        #endregion
        #region Specification

        public static SpecificationDTO CreateSpecification(SpecificationDTO Specification)
        {
            return Task.Run(async () => await articleRequest.CreateSpecification(Specification)).Result;
        }

        public static List<SpecificationDTO> SelectAllSpecification()
        {
            return Task.Run(async () => await articleRequest.SelectAllSpecification()).Result;
        }

        public static SpecificationDTO GetSpecificationById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetSpecificationById(Id)).Result;
        }

        public static SpecificationDTO UpdateSpecification(SpecificationDTO Specification)
        {
            return Task.Run(async () => await articleRequest.UpdateSpecification(Specification)).Result;
        }

        public static bool DeleteSpecificationById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteSpecificationById(Id)).Result;
        }

        #endregion
        #region StockBalance

        public static StockBalanceDTO CreateStockBalance(StockBalanceDTO StockBalance)
        {
            return Task.Run(async () => await articleRequest.CreateStockBalance(StockBalance)).Result;
        }

        public static List<StockBalanceDTO> SelectAllStockBalance()
        {
            return Task.Run(async () => await articleRequest.SelectAllStockBalance()).Result;
        }

        public static StockBalanceDTO GetStockBalanceById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetStockBalanceById(Id)).Result;
        }

        public static StockBalanceDTO UpdateStockBalance(StockBalanceDTO StockBalance)
        {
            return Task.Run(async () => await articleRequest.UpdateStockBalance(StockBalance)).Result;
        }

        public static bool DeleteStockBalanceById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteStockBalanceById(Id)).Result;
        }

        #endregion
        #region StockLevel

        public static StockLevelDTO CreateStockLevel(StockLevelDTO StockLevel)
        {
            return Task.Run(async () => await articleRequest.CreateStockLevel(StockLevel)).Result;
        }

        public static List<StockLevelDTO> SelectAllStockLevel()
        {
            return Task.Run(async () => await articleRequest.SelectAllStockLevel()).Result;
        }

        public static StockLevelDTO GetStockLevelById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetStockLevelById(Id)).Result;
        }

        public static StockLevelDTO UpdateStockLevel(StockLevelDTO StockLevel)
        {
            return Task.Run(async () => await articleRequest.UpdateStockLevel(StockLevel)).Result;
        }

        public static bool DeleteStockLevelById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteStockLevelById(Id)).Result;
        }

        #endregion
        #region Value

        public static ValueDTO CreateValue(ValueDTO Value)
        {
            return Task.Run(async () => await articleRequest.CreateValue(Value)).Result;
        }

        public static List<ValueDTO> SelectAllValue()
        {
            return Task.Run(async () => await articleRequest.SelectAllValue()).Result;
        }

        public static ValueDTO GetValueById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetValueById(Id)).Result;
        }

        public static ValueDTO UpdateValue(ValueDTO Value)
        {
            return Task.Run(async () => await articleRequest.UpdateValue(Value)).Result;
        }

        public static bool DeleteValueById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteValueById(Id)).Result;
        }

        #endregion
        #region ValueChangeLog

        public static ValueChangeLogDTO CreateValueChangeLog(ValueChangeLogDTO ValueChangeLog)
        {
            return Task.Run(async () => await articleRequest.CreateValueChangeLog(ValueChangeLog)).Result;
        }

        public static List<ValueChangeLogDTO> SelectAllValueChangeLog()
        {
            return Task.Run(async () => await articleRequest.SelectAllValueChangeLog()).Result;
        }

        public static ValueChangeLogDTO GetValueChangeLogById(int Id)
        {
            return Task.Run(async () => await articleRequest.GetValueChangeLogById(Id)).Result;
        }

        public static ValueChangeLogDTO UpdateValueChangeLog(ValueChangeLogDTO ValueChangeLog)
        {
            return Task.Run(async () => await articleRequest.UpdateValueChangeLog(ValueChangeLog)).Result;
        }

        public static bool DeleteValueChangeLogById(int Id)
        {
            return Task.Run(async () => await articleRequest.DeleteValueChangeLogById(Id)).Result;
        }

        #endregion
        #endregion


        #region consignee Schema
        #region BankAccountDetail

        public static BankAccountDetailDTO CreateBankAccountDetail(BankAccountDetailDTO BankAccountDetail)
        {
            return Task.Run(async () => await consigneeRequest.CreateBankAccountDetail(BankAccountDetail)).Result;
        }

        public static List<BankAccountDetailDTO> SelectAllBankAccountDetail()
        {
            return Task.Run(async () => await consigneeRequest.SelectAllBankAccountDetail()).Result;
        }

        public static BankAccountDetailDTO GetBankAccountDetailById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.GetBankAccountDetailById(Id)).Result;
        }

        public static BankAccountDetailDTO UpdateBankAccountDetail(BankAccountDetailDTO BankAccountDetail)
        {
            return Task.Run(async () => await consigneeRequest.UpdateBankAccountDetail(BankAccountDetail)).Result;
        }

        public static bool DeleteBankAccountDetailById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.DeleteBankAccountDetailById(Id)).Result;
        }

        #endregion
        #region Consignee

        public static ConsigneeDTO CreateConsignee(ConsigneeDTO Consignee)
        {
            return Task.Run(async () => await consigneeRequest.CreateConsignee(Consignee)).Result;
        }

        public static List<ConsigneeDTO> SelectAllConsignee()
        {
            return Task.Run(async () => await consigneeRequest.SelectAllConsignee()).Result;
        }
        public static List<ConsigneeDTO> SelectAllConsigneeWithRequiredfield()
        {
            return Task.Run(async () => await consigneeRequest.SelectAllConsigneeWithRequiredfield()).Result;
        }
        public static ConsigneeDTO GetConsigneeById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.GetConsigneeById(Id)).Result;
        }

        public static ConsigneeDTO UpdateConsignee(ConsigneeDTO Consignee)
        {
            return Task.Run(async () => await consigneeRequest.UpdateConsignee(Consignee)).Result;
        }

        public static bool DeleteConsigneeById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.DeleteConsigneeById(Id)).Result;
        }

        #endregion


        #region Consignee Buffer
        public static ConsigneeBuffer CreateConsigneeBuffer(ConsigneeBuffer Consignee)
        {
            return Task.Run(async () => await consigneeRequest.CreateConsigneeBuffer(Consignee)).Result;
        }
        public static ConsigneeBuffer GetConsigneeBufferById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.GetConsigneeBufferById(Id)).Result;
        }

        public static ConsigneeBuffer UpdateConsigneeBuffer(ConsigneeBuffer Consignee)
        {
            return Task.Run(async () => await consigneeRequest.UpdateConsigneeBuffer(Consignee)).Result;
        }
        #endregion
        #region ConsigneeUnit

        public static ConsigneeUnitDTO CreateConsigneeUnit(ConsigneeUnitDTO ConsigneeUnit)
        {
            return Task.Run(async () => await consigneeRequest.CreateConsigneeUnit(ConsigneeUnit)).Result;
        }

        public static List<ConsigneeUnitDTO> SelectAllConsigneeUnit()
        {
            return Task.Run(async () => await consigneeRequest.SelectAllConsigneeUnit()).Result;
        }

        public static ConsigneeUnitDTO GetConsigneeUnitById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.GetConsigneeUnitById(Id)).Result;
        }
 
        public static ConsigneeUnitDTO UpdateConsigneeUnit(ConsigneeUnitDTO ConsigneeUnit)
        {
            return Task.Run(async () => await consigneeRequest.UpdateConsigneeUnit(ConsigneeUnit)).Result;
        }

        public static bool DeleteConsigneeUnitById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.DeleteConsigneeUnitById(Id)).Result;
        }

        #endregion
        #region ConsigneeUser

        public static ConsigneeUserDTO CreateConsigneeUser(ConsigneeUserDTO ConsigneeUser)
        {
            return Task.Run(async () => await consigneeRequest.CreateConsigneeUser(ConsigneeUser)).Result;
        }

        public static List<ConsigneeUserDTO> SelectAllConsigneeUser()
        {
            return Task.Run(async () => await consigneeRequest.SelectAllConsigneeUser()).Result;
        }

        public static ConsigneeUserDTO GetConsigneeUserById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.GetConsigneeUserById(Id)).Result;
        }

        public static ConsigneeUserDTO UpdateConsigneeUser(ConsigneeUserDTO ConsigneeUser)
        {
            return Task.Run(async () => await consigneeRequest.UpdateConsigneeUser(ConsigneeUser)).Result;
        }

        public static bool DeleteConsigneeUserById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.DeleteConsigneeUserById(Id)).Result;
        }

        #endregion
        #region Identification

        public static IdentificationDTO CreateIdentification(IdentificationDTO Identification)
        {
            return Task.Run(async () => await consigneeRequest.CreateIdentification(Identification)).Result;
        }

        public static List<IdentificationDTO> SelectAllIdentification()
        {
            return Task.Run(async () => await consigneeRequest.SelectAllIdentification()).Result;
        }

        public static IdentificationDTO GetIdentificationById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.GetIdentificationById(Id)).Result;
        }

        public static IdentificationDTO UpdateIdentification(IdentificationDTO Identification)
        {
            return Task.Run(async () => await consigneeRequest.UpdateIdentification(Identification)).Result;
        }

        public static bool DeleteIdentificationById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.DeleteIdentificationById(Id)).Result;
        }

        #endregion
        #region LanguagePreference

        public static LanguagePreferenceDTO CreateLanguagePreference(LanguagePreferenceDTO LanguagePreference)
        {
            return Task.Run(async () => await consigneeRequest.CreateLanguagePreference(LanguagePreference)).Result;
        }

        public static List<LanguagePreferenceDTO> SelectAllLanguagePreference()
        {
            return Task.Run(async () => await consigneeRequest.SelectAllLanguagePreference()).Result;
        }

        public static LanguagePreferenceDTO GetLanguagePreferenceById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.GetLanguagePreferenceById(Id)).Result;
        }

        public static LanguagePreferenceDTO UpdateLanguagePreference(LanguagePreferenceDTO LanguagePreference)
        {
            return Task.Run(async () => await consigneeRequest.UpdateLanguagePreference(LanguagePreference)).Result;
        }

        public static bool DeleteLanguagePreferenceById(int Id)
        {
            return Task.Run(async () => await consigneeRequest.DeleteLanguagePreferenceById(Id)).Result;
        }

        #endregion

        #endregion


        #region setting Schema
        #region ActivityDefinition

        public static ActivityDefinitionDTO CreateActivityDefinition(ActivityDefinitionDTO ActivityDefinition)
        {
            return Task.Run(async () => await settingRequest.CreateActivityDefinition(ActivityDefinition)).Result;
        }

        public static List<ActivityDefinitionDTO> SelectAllActivityDefinition()
        {
            return Task.Run(async () => await settingRequest.SelectAllActivityDefinition()).Result;
        }

        public static ActivityDefinitionDTO GetActivityDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetActivityDefinitionById(Id)).Result;
        }

        public static ActivityDefinitionDTO UpdateActivityDefinition(ActivityDefinitionDTO ActivityDefinition)
        {
            return Task.Run(async () => await settingRequest.UpdateActivityDefinition(ActivityDefinition)).Result;
        }

        public static bool DeleteActivityDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteActivityDefinitionById(Id)).Result;
        }

        #endregion
        #region CNETLicense

        public static CnetlicenseDTO CreateCNETLicense(CnetlicenseDTO CNETLicense)
        {
            return Task.Run(async () => await settingRequest.CreateCNETLicense(CNETLicense)).Result;
        }

        public static List<CnetlicenseDTO> SelectAllCNETLicense()
        {
            return Task.Run(async () => await settingRequest.SelectAllCNETLicense()).Result;
        }

        public static CnetlicenseDTO GetCNETLicenseById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetCNETLicenseById(Id)).Result;
        }

        public static CnetlicenseDTO UpdateCNETLicense(CnetlicenseDTO CNETLicense)
        {
            return Task.Run(async () => await settingRequest.UpdateCNETLicense(CNETLicense)).Result;
        }

        public static bool DeleteCNETLicenseById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteCNETLicenseById(Id)).Result;
        }

        #endregion
        #region Configuration

        public static ConfigurationDTO CreateConfiguration(ConfigurationDTO Configuration)
        {
            return Task.Run(async () => await settingRequest.CreateConfiguration(Configuration)).Result;
        }

        public static List<ConfigurationDTO> SelectAllConfiguration()
        {
            return Task.Run(async () => await settingRequest.SelectAllConfiguration()).Result;
        }

        public static ConfigurationDTO GetConfigurationById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetConfigurationById(Id)).Result;
        }

        public static ConfigurationDTO UpdateConfiguration(ConfigurationDTO Configuration)
        {
            return Task.Run(async () => await settingRequest.UpdateConfiguration(Configuration)).Result;
        }

        public static bool DeleteConfigurationById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteConfigurationById(Id)).Result;
        }

        #endregion
        #region Device

        public static DeviceDTO CreateDevice(DeviceDTO Device)
        {
            return Task.Run(async () => await settingRequest.CreateDevice(Device)).Result;
        }

        public static List<DeviceDTO> SelectAllDevice()
        {
            return Task.Run(async () => await settingRequest.SelectAllDevice()).Result;
        }

        public static DeviceDTO GetDeviceById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetDeviceById(Id)).Result;
        }

        public static DeviceDTO UpdateDevice(DeviceDTO Device)
        {
            return Task.Run(async () => await settingRequest.UpdateDevice(Device)).Result;
        }

        public static bool DeleteDeviceById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteDeviceById(Id)).Result;
        }

        #endregion
        #region DiscountFactor

        public static DiscountFactorDTO CreateDiscountFactor(DiscountFactorDTO DiscountFactor)
        {
            return Task.Run(async () => await settingRequest.CreateDiscountFactor(DiscountFactor)).Result;
        }

        public static List<DiscountFactorDTO> SelectAllDiscountFactor()
        {
            return Task.Run(async () => await settingRequest.SelectAllDiscountFactor()).Result;
        }

        public static DiscountFactorDTO GetDiscountFactorById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetDiscountFactorById(Id)).Result;
        }

        public static DiscountFactorDTO UpdateDiscountFactor(DiscountFactorDTO DiscountFactor)
        {
            return Task.Run(async () => await settingRequest.UpdateDiscountFactor(DiscountFactor)).Result;
        }

        public static bool DeleteDiscountFactorById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteDiscountFactorById(Id)).Result;
        }

        #endregion
        #region Distribution

        public static DistributionDTO CreateDistribution(DistributionDTO Distribution)
        {
            return Task.Run(async () => await settingRequest.CreateDistribution(Distribution)).Result;
        }

        public static List<DistributionDTO> SelectAllDistribution()
        {
            return Task.Run(async () => await settingRequest.SelectAllDistribution()).Result;
        }

        public static DistributionDTO GetDistributionById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetDistributionById(Id)).Result;
        }

        public static DistributionDTO UpdateDistribution(DistributionDTO Distribution)
        {
            return Task.Run(async () => await settingRequest.UpdateDistribution(Distribution)).Result;
        }

        public static bool DeleteDistributionById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteDistributionById(Id)).Result;
        }

        #endregion
        #region FieldFormat

        public static FieldFormatDTO CreateFieldFormat(FieldFormatDTO FieldFormat)
        {
            return Task.Run(async () => await settingRequest.CreateFieldFormat(FieldFormat)).Result;
        }

        public static List<FieldFormatDTO> SelectAllFieldFormat()
        {
            return Task.Run(async () => await settingRequest.SelectAllFieldFormat()).Result;
        }

        public static FieldFormatDTO GetFieldFormatById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetFieldFormatById(Id)).Result;
        }

        public static FieldFormatDTO UpdateFieldFormat(FieldFormatDTO FieldFormat)
        {
            return Task.Run(async () => await settingRequest.UpdateFieldFormat(FieldFormat)).Result;
        }

        public static bool DeleteFieldFormatById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteFieldFormatById(Id)).Result;
        }

        #endregion
        #region IDDefinition

        public static IddefinitionDTO CreateIDDefinition(IddefinitionDTO IDDefinition)
        {
            return Task.Run(async () => await settingRequest.CreateIDDefinition(IDDefinition)).Result;
        }

        public static List<IddefinitionDTO> SelectAllIDDefinition()
        {
            return Task.Run(async () => await settingRequest.SelectAllIDDefinition()).Result;
        }

        public static IddefinitionDTO GetIDDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetIDDefinitionById(Id)).Result;
        }

        public static IddefinitionDTO UpdateIDDefinition(IddefinitionDTO IDDefinition)
        {
            return Task.Run(async () => await settingRequest.UpdateIDDefinition(IDDefinition)).Result;
        }

        public static bool DeleteIDDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteIDDefinitionById(Id)).Result;
        }

        #endregion
        #region IDSetting

        public static IdsettingDTO CreateIDSetting(IdsettingDTO IDSetting)
        {
            return Task.Run(async () => await settingRequest.CreateIDSetting(IDSetting)).Result;
        }

        public static List<IdsettingDTO> SelectAllIDSetting()
        {
            return Task.Run(async () => await settingRequest.SelectAllIDSetting()).Result;
        }

        public static IdsettingDTO GetIDSettingById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetIDSettingById(Id)).Result;
        }

        public static IdsettingDTO UpdateIDSetting(IdsettingDTO IDSetting)
        {
            return Task.Run(async () => await settingRequest.UpdateIDSetting(IDSetting)).Result;
        }

        public static bool DeleteIDSettingById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteIDSettingById(Id)).Result;
        }

        #endregion
        #region MenuDesigner

        public static MenuDesignerDTO CreateMenuDesigner(MenuDesignerDTO MenuDesigner)
        {
            return Task.Run(async () => await settingRequest.CreateMenuDesigner(MenuDesigner)).Result;
        }

        public static List<MenuDesignerDTO> SelectAllMenuDesigner()
        {
            return Task.Run(async () => await settingRequest.SelectAllMenuDesigner()).Result;
        }

        public static MenuDesignerDTO GetMenuDesignerById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetMenuDesignerById(Id)).Result;
        }

        public static MenuDesignerDTO UpdateMenuDesigner(MenuDesignerDTO MenuDesigner)
        {
            return Task.Run(async () => await settingRequest.UpdateMenuDesigner(MenuDesigner)).Result;
        }

        public static bool DeleteMenuDesignerById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteMenuDesignerById(Id)).Result;
        }

        #endregion

        #region OrderStationMap

        public static OrderStationMapDTO CreateOrderStationMap(OrderStationMapDTO OrderStationMap)
        {
            return Task.Run(async () => await settingRequest.CreateOrderStationMap(OrderStationMap)).Result;
        }

        public static List<OrderStationMapDTO> SelectAllOrderStationMap()
        {
            return Task.Run(async () => await settingRequest.SelectAllOrderStationMap()).Result;
        }

        public static OrderStationMapDTO GetOrderStationMapById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetOrderStationMapById(Id)).Result;
        }

        public static OrderStationMapDTO UpdateOrderStationMap(OrderStationMapDTO OrderStationMap)
        {
            return Task.Run(async () => await settingRequest.UpdateOrderStationMap(OrderStationMap)).Result;
        }

        public static bool DeleteOrderStationMapById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteOrderStationMapById(Id)).Result;
        }

        #endregion
        #region Preference

        public static PreferenceDTO CreatePreference(PreferenceDTO Preference)
        {
            return Task.Run(async () => await settingRequest.CreatePreference(Preference)).Result;
        }

        public static List<PreferenceDTO> SelectAllPreference()
        {
            return Task.Run(async () => await settingRequest.SelectAllPreference()).Result;
        }

        public static PreferenceDTO GetPreferenceById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetPreferenceById(Id)).Result;
        }

        public static PreferenceDTO UpdatePreference(PreferenceDTO Preference)
        {
            return Task.Run(async () => await settingRequest.UpdatePreference(Preference)).Result;
        }

        public static bool DeletePreferenceById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeletePreferenceById(Id)).Result;
        }

        #endregion
        #region PreferenceAccess

        public static PreferenceAccessDTO CreatePreferenceAccess(PreferenceAccessDTO PreferenceAccess)
        {
            return Task.Run(async () => await settingRequest.CreatePreferenceAccess(PreferenceAccess)).Result;
        }

        public static List<PreferenceAccessDTO> SelectAllPreferenceAccess()
        {
            return Task.Run(async () => await settingRequest.SelectAllPreferenceAccess()).Result;
        }

        public static PreferenceAccessDTO GetPreferenceAccessById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetPreferenceAccessById(Id)).Result;
        }

        public static PreferenceAccessDTO UpdatePreferenceAccess(PreferenceAccessDTO PreferenceAccess)
        {
            return Task.Run(async () => await settingRequest.UpdatePreferenceAccess(PreferenceAccess)).Result;
        }

        public static bool DeletePreferenceAccessById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeletePreferenceAccessById(Id)).Result;
        }

        #endregion
        #region ProgressTaxRate

        public static ProgressTaxRateDTO CreateProgressTaxRate(ProgressTaxRateDTO ProgressTaxRate)
        {
            return Task.Run(async () => await settingRequest.CreateProgressTaxRate(ProgressTaxRate)).Result;
        }

        public static List<ProgressTaxRateDTO> SelectAllProgressTaxRate()
        {
            return Task.Run(async () => await settingRequest.SelectAllProgressTaxRate()).Result;
        }

        public static ProgressTaxRateDTO GetProgressTaxRateById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetProgressTaxRateById(Id)).Result;
        }

        public static ProgressTaxRateDTO UpdateProgressTaxRate(ProgressTaxRateDTO ProgressTaxRate)
        {
            return Task.Run(async () => await settingRequest.UpdateProgressTaxRate(ProgressTaxRate)).Result;
        }

        public static bool DeleteProgressTaxRateById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteProgressTaxRateById(Id)).Result;
        }

        #endregion
        #region ReconciliationDetail

        public static ReconciliationDetailDTO CreateReconciliationDetail(ReconciliationDetailDTO ReconciliationDetail)
        {
            return Task.Run(async () => await settingRequest.CreateReconciliationDetail(ReconciliationDetail)).Result;
        }

        public static List<ReconciliationDetailDTO> SelectAllReconciliationDetail()
        {
            return Task.Run(async () => await settingRequest.SelectAllReconciliationDetail()).Result;
        }

        public static ReconciliationDetailDTO GetReconciliationDetailById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetReconciliationDetailById(Id)).Result;
        }

        public static ReconciliationDetailDTO UpdateReconciliationDetail(ReconciliationDetailDTO ReconciliationDetail)
        {
            return Task.Run(async () => await settingRequest.UpdateReconciliationDetail(ReconciliationDetail)).Result;
        }

        public static bool DeleteReconciliationDetailById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteReconciliationDetailById(Id)).Result;
        }

        #endregion
        #region ReconciliationSummary

        public static ReconciliationSummaryDTO CreateReconciliationSummary(ReconciliationSummaryDTO ReconciliationSummary)
        {
            return Task.Run(async () => await settingRequest.CreateReconciliationSummary(ReconciliationSummary)).Result;
        }

        public static List<ReconciliationSummaryDTO> SelectAllReconciliationSummary()
        {
            return Task.Run(async () => await settingRequest.SelectAllReconciliationSummary()).Result;
        }

        public static ReconciliationSummaryDTO GetReconciliationSummaryById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetReconciliationSummaryById(Id)).Result;
        }

        public static ReconciliationSummaryDTO UpdateReconciliationSummary(ReconciliationSummaryDTO ReconciliationSummary)
        {
            return Task.Run(async () => await settingRequest.UpdateReconciliationSummary(ReconciliationSummary)).Result;
        }

        public static bool DeleteReconciliationSummaryById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteReconciliationSummaryById(Id)).Result;
        }

        #endregion
        #region RelationalState

        public static RelationalStateDTO CreateRelationalState(RelationalStateDTO RelationalState)
        {
            return Task.Run(async () => await settingRequest.CreateRelationalState(RelationalState)).Result;
        }

        public static List<RelationalStateDTO> SelectAllRelationalState()
        {
            return Task.Run(async () => await settingRequest.SelectAllRelationalState()).Result;
        }

        public static RelationalStateDTO GetRelationalStateById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetRelationalStateById(Id)).Result;
        }

        public static RelationalStateDTO UpdateRelationalState(RelationalStateDTO RelationalState)
        {
            return Task.Run(async () => await settingRequest.UpdateRelationalState(RelationalState)).Result;
        }

        public static bool DeleteRelationalStateById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteRelationalStateById(Id)).Result;
        }

        #endregion
        #region Report

        public static ReportDTO CreateReport(ReportDTO Report)
        {
            return Task.Run(async () => await settingRequest.CreateReport(Report)).Result;
        }

        public static List<ReportDTO> SelectAllReport()
        {
            return Task.Run(async () => await settingRequest.SelectAllReport()).Result;
        }

        public static ReportDTO GetReportById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetReportById(Id)).Result;
        }

        public static ReportDTO UpdateReport(ReportDTO Report)
        {
            return Task.Run(async () => await settingRequest.UpdateReport(Report)).Result;
        }

        public static bool DeleteReportById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteReportById(Id)).Result;
        }

        #endregion
        #region RequiredGSL

        public static RequiredGslDTO CreateRequiredGSL(RequiredGslDTO RequiredGSL)
        {
            return Task.Run(async () => await settingRequest.CreateRequiredGSL(RequiredGSL)).Result;
        }

        public static List<RequiredGslDTO> SelectAllRequiredGSL()
        {
            return Task.Run(async () => await settingRequest.SelectAllRequiredGSL()).Result;
        }

        public static RequiredGslDTO GetRequiredGSLById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetRequiredGSLById(Id)).Result;
        }

        public static RequiredGslDTO UpdateRequiredGSL(RequiredGslDTO RequiredGSL)
        {
            return Task.Run(async () => await settingRequest.UpdateRequiredGSL(RequiredGSL)).Result;
        }

        public static bool DeleteRequiredGSLById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteRequiredGSLById(Id)).Result;
        }

        #endregion
        #region RequiredGSLDetail

        public static CNET_V7_Domain.Domain.SettingSchema.RequiredGsldetailDTO CreateRequiredGSLDetail(CNET_V7_Domain.Domain.SettingSchema.RequiredGsldetailDTO RequiredGSLDetail)
        {
            return Task.Run(async () => await settingRequest.CreateRequiredGSLDetail(RequiredGSLDetail)).Result;
        }

        public static List<CNET_V7_Domain.Domain.SettingSchema.RequiredGsldetailDTO> SelectAllRequiredGSLDetail()
        {
            return Task.Run(async () => await settingRequest.SelectAllRequiredGSLDetail()).Result;
        }

        public static CNET_V7_Domain.Domain.SettingSchema.RequiredGsldetailDTO GetRequiredGSLDetailById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetRequiredGSLDetailById(Id)).Result;
        }

        public static CNET_V7_Domain.Domain.SettingSchema.RequiredGsldetailDTO UpdateRequiredGSLDetail(CNET_V7_Domain.Domain.SettingSchema.RequiredGsldetailDTO RequiredGSLDetail)
        {
            return Task.Run(async () => await settingRequest.UpdateRequiredGSLDetail(RequiredGSLDetail)).Result;
        }

        public static bool DeleteRequiredGSLDetailById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteRequiredGSLDetailById(Id)).Result;
        }

        #endregion
        #region SystemConstant

        public static SystemConstantDTO CreateSystemConstant(SystemConstantDTO SystemConstant)
        {
            return Task.Run(async () => await settingRequest.CreateSystemConstant(SystemConstant)).Result;
        }

        public static List<SystemConstantDTO> SelectAllSystemConstant()
        {
            try
            {
                return Task.Run(async () => await settingRequest.SelectAllSystemConstant()).Result;
            }
            catch (Exception io)
            {
                MessageBox.Show("Error:- SelectAllSystemConstant" + Environment.NewLine + io.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<SystemConstantDTO>();
            }
        }

        public static SystemConstantDTO GetSystemConstantById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetSystemConstantById(Id)).Result;
        }

        public static SystemConstantDTO UpdateSystemConstant(SystemConstantDTO SystemConstant)
        {
            return Task.Run(async () => await settingRequest.UpdateSystemConstant(SystemConstant)).Result;
        }

        public static bool DeleteSystemConstantById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteSystemConstantById(Id)).Result;
        }

        #endregion
        #region Tax

        public static TaxDTO CreateTax(TaxDTO Tax)
        {
            return Task.Run(async () => await settingRequest.CreateTax(Tax)).Result;
        }

        public static List<TaxDTO> SelectAllTax()
        {
            return Task.Run(async () => await settingRequest.SelectAllTax()).Result;
        }

        public static TaxDTO GetTaxById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetTaxById(Id)).Result;
        }

        public static TaxDTO UpdateTax(TaxDTO Tax)
        {
            return Task.Run(async () => await settingRequest.UpdateTax(Tax)).Result;
        }

        public static bool DeleteTaxById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteTaxById(Id)).Result;
        }

        #endregion
        #region TermDefinition

        public static TermDefinitionDTO CreateTermDefinition(TermDefinitionDTO TermDefinition)
        {
            return Task.Run(async () => await settingRequest.CreateTermDefinition(TermDefinition)).Result;
        }

        public static List<TermDefinitionDTO> SelectAllTermDefinition()
        {
            return Task.Run(async () => await settingRequest.SelectAllTermDefinition()).Result;
        }

        public static TermDefinitionDTO GetTermDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetTermDefinitionById(Id)).Result;
        }

        public static TermDefinitionDTO UpdateTermDefinition(TermDefinitionDTO TermDefinition)
        {
            return Task.Run(async () => await settingRequest.UpdateTermDefinition(TermDefinition)).Result;
        }

        public static bool DeleteTermDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteTermDefinitionById(Id)).Result;
        }

        #endregion
        #region ValueFactorDefinition

        public static ValueFactorDefinitionDTO CreateValueFactorDefinition(ValueFactorDefinitionDTO ValueFactorDefinition)
        {
            return Task.Run(async () => await settingRequest.CreateValueFactorDefinition(ValueFactorDefinition)).Result;
        }

        public static List<ValueFactorDefinitionDTO> SelectAllValueFactorDefinition()
        {
            return Task.Run(async () => await settingRequest.SelectAllValueFactorDefinition()).Result;
        }

        public static ValueFactorDefinitionDTO GetValueFactorDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetValueFactorDefinitionById(Id)).Result;
        }

        public static ValueFactorDefinitionDTO UpdateValueFactorDefinition(ValueFactorDefinitionDTO ValueFactorDefinition)
        {
            return Task.Run(async () => await settingRequest.UpdateValueFactorDefinition(ValueFactorDefinition)).Result;
        }

        public static bool DeleteValueFactorDefinitionById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteValueFactorDefinitionById(Id)).Result;
        }

        #endregion
        #region ValueFactorSetup

        public static ValueFactorSetupDTO CreateValueFactorSetup(ValueFactorSetupDTO ValueFactorSetup)
        {
            return Task.Run(async () => await settingRequest.CreateValueFactorSetup(ValueFactorSetup)).Result;
        }

        public static List<ValueFactorSetupDTO> SelectAllValueFactorSetup()
        {
            return Task.Run(async () => await settingRequest.SelectAllValueFactorSetup()).Result;
        }

        public static ValueFactorSetupDTO GetValueFactorSetupById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetValueFactorSetupById(Id)).Result;
        }

        public static ValueFactorSetupDTO UpdateValueFactorSetup(ValueFactorSetupDTO ValueFactorSetup)
        {
            return Task.Run(async () => await settingRequest.UpdateValueFactorSetup(ValueFactorSetup)).Result;
        }

        public static bool DeleteValueFactorSetupById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteValueFactorSetupById(Id)).Result;
        }

        #endregion
        #region VoucherStoreMapping

        public static VoucherStoreMappingDTO CreateVoucherStoreMapping(VoucherStoreMappingDTO VoucherStoreMapping)
        {
            return Task.Run(async () => await settingRequest.CreateVoucherStoreMapping(VoucherStoreMapping)).Result;
        }

        public static List<VoucherStoreMappingDTO> SelectAllVoucherStoreMapping()
        {
            return Task.Run(async () => await settingRequest.SelectAllVoucherStoreMapping()).Result;
        }

        public static VoucherStoreMappingDTO GetVoucherStoreMappingById(int Id)
        {
            return Task.Run(async () => await settingRequest.GetVoucherStoreMappingById(Id)).Result;
        }

        public static VoucherStoreMappingDTO UpdateVoucherStoreMapping(VoucherStoreMappingDTO VoucherStoreMapping)
        {
            return Task.Run(async () => await settingRequest.UpdateVoucherStoreMapping(VoucherStoreMapping)).Result;
        }

        public static bool DeleteVoucherStoreMappingById(int Id)
        {
            return Task.Run(async () => await settingRequest.DeleteVoucherStoreMappingById(Id)).Result;
        }

        #endregion
        #endregion


        #region transaction Schema
        #region ClosedRelation

        public static ClosedRelationDTO CreateClosedRelation(ClosedRelationDTO ClosedRelation)
        {
            return Task.Run(async () => await transactionRequest.CreateClosedRelation(ClosedRelation)).Result;
        }

        public static List<ClosedRelationDTO> SelectAllClosedRelation()
        {
            return Task.Run(async () => await transactionRequest.SelectAllClosedRelation()).Result;
        }

        public static ClosedRelationDTO GetClosedRelationById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetClosedRelationById(Id)).Result;
        }

        public static ClosedRelationDTO UpdateClosedRelation(ClosedRelationDTO ClosedRelation)
        {
            return Task.Run(async () => await transactionRequest.UpdateClosedRelation(ClosedRelation)).Result;
        }

        public static bool DeleteClosedRelationById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteClosedRelationById(Id)).Result;
        }

        #endregion
        #region DenominationDetail

        public static DenominationDetailDTO CreateDenominationDetail(DenominationDetailDTO DenominationDetail)
        {
            return Task.Run(async () => await transactionRequest.CreateDenominationDetail(DenominationDetail)).Result;
        }

        public static List<DenominationDetailDTO> SelectAllDenominationDetail()
        {
            return Task.Run(async () => await transactionRequest.SelectAllDenominationDetail()).Result;
        }

        public static DenominationDetailDTO GetDenominationDetailById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetDenominationDetailById(Id)).Result;
        }

        public static DenominationDetailDTO UpdateDenominationDetail(DenominationDetailDTO DenominationDetail)
        {
            return Task.Run(async () => await transactionRequest.UpdateDenominationDetail(DenominationDetail)).Result;
        }

        public static bool DeleteDenominationDetailById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteDenominationDetailById(Id)).Result;
        }

        #endregion
        #region LineItem

        public static LineItemDTO CreateLineItem(LineItemDTO LineItem)
        {
            return Task.Run(async () => await transactionRequest.CreateLineItem(LineItem)).Result;
        }

        public static List<LineItemDTO> SelectAllLineItem()
        {
            return Task.Run(async () => await transactionRequest.SelectAllLineItem()).Result;
        }

        public static LineItemDTO GetLineItemById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetLineItemById(Id)).Result;
        }

        public static LineItemDTO UpdateLineItem(LineItemDTO LineItem)
        {
            return Task.Run(async () => await transactionRequest.UpdateLineItem(LineItem)).Result;
        }

        public static bool DeleteLineItemById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteLineItemById(Id)).Result;
        }

        #endregion
        #region LineItemConversion

        public static LineItemConversionDTO CreateLineItemConversion(LineItemConversionDTO LineItemConversion)
        {
            return Task.Run(async () => await transactionRequest.CreateLineItemConversion(LineItemConversion)).Result;
        }

        public static List<LineItemConversionDTO> SelectAllLineItemConversion()
        {
            return Task.Run(async () => await transactionRequest.SelectAllLineItemConversion()).Result;
        }

        public static LineItemConversionDTO GetLineItemConversionById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetLineItemConversionById(Id)).Result;
        }

        public static LineItemConversionDTO UpdateLineItemConversion(LineItemConversionDTO LineItemConversion)
        {
            return Task.Run(async () => await transactionRequest.UpdateLineItemConversion(LineItemConversion)).Result;
        }

        public static bool DeleteLineItemConversionById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteLineItemConversionById(Id)).Result;
        }

        #endregion
        #region LineItemReference

        public static LineItemReferenceDTO CreateLineItemReference(LineItemReferenceDTO LineItemReference)
        {
            return Task.Run(async () => await transactionRequest.CreateLineItemReference(LineItemReference)).Result;
        }

        public static List<LineItemReferenceDTO> SelectAllLineItemReference()
        {
            return Task.Run(async () => await transactionRequest.SelectAllLineItemReference()).Result;
        }

        public static LineItemReferenceDTO GetLineItemReferenceById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetLineItemReferenceById(Id)).Result;
        }

        public static LineItemReferenceDTO UpdateLineItemReference(LineItemReferenceDTO LineItemReference)
        {
            return Task.Run(async () => await transactionRequest.UpdateLineItemReference(LineItemReference)).Result;
        }

        public static bool DeleteLineItemReferenceById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteLineItemReferenceById(Id)).Result;
        }

        #endregion
        #region LineItemValueFactor

        public static LineItemValueFactorDTO CreateLineItemValueFactor(LineItemValueFactorDTO LineItemValueFactor)
        {
            return Task.Run(async () => await transactionRequest.CreateLineItemValueFactor(LineItemValueFactor)).Result;
        }

        public static List<LineItemValueFactorDTO> SelectAllLineItemValueFactor()
        {
            return Task.Run(async () => await transactionRequest.SelectAllLineItemValueFactor()).Result;
        }

        public static LineItemValueFactorDTO GetLineItemValueFactorById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetLineItemValueFactorById(Id)).Result;
        }

        public static LineItemValueFactorDTO UpdateLineItemValueFactor(LineItemValueFactorDTO LineItemValueFactor)
        {
            return Task.Run(async () => await transactionRequest.UpdateLineItemValueFactor(LineItemValueFactor)).Result;
        }

        public static bool DeleteLineItemValueFactorById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteLineItemValueFactorById(Id)).Result;
        }

        #endregion

        #region PreferentialValueFactor

        public static PreferentialValueFactorDTO CreatePreferentialValueFactor(PreferentialValueFactorDTO PreferentialValueFactor)
        {
            return Task.Run(async () => await transactionRequest.CreatePreferentialValueFactor(PreferentialValueFactor)).Result;
        }

        public static List<PreferentialValueFactorDTO> SelectAllPreferentialValueFactor()
        {
            return Task.Run(async () => await transactionRequest.SelectAllPreferentialValueFactor()).Result;
        }

        public static PreferentialValueFactorDTO GetPreferentialValueFactorById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetPreferentialValueFactorById(Id)).Result;
        }

        public static PreferentialValueFactorDTO UpdatePreferentialValueFactor(PreferentialValueFactorDTO PreferentialValueFactor)
        {
            return Task.Run(async () => await transactionRequest.UpdatePreferentialValueFactor(PreferentialValueFactor)).Result;
        }

        public static bool DeletePreferentialValueFactorById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeletePreferentialValueFactorById(Id)).Result;
        }

        #endregion
        #region TaxTransaction

        public static TaxTransactionDTO CreateTaxTransaction(TaxTransactionDTO TaxTransaction)
        {
            return Task.Run(async () => await transactionRequest.CreateTaxTransaction(TaxTransaction)).Result;
        }

        public static List<TaxTransactionDTO> SelectAllTaxTransaction()
        {
            return Task.Run(async () => await transactionRequest.SelectAllTaxTransaction()).Result;
        }

        public static TaxTransactionDTO GetTaxTransactionById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetTaxTransactionById(Id)).Result;
        }

        public static TaxTransactionDTO UpdateTaxTransaction(TaxTransactionDTO TaxTransaction)
        {
            return Task.Run(async () => await transactionRequest.UpdateTaxTransaction(TaxTransaction)).Result;
        }

        public static bool DeleteTaxTransactionById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteTaxTransactionById(Id)).Result;
        }

        #endregion
        #region TransactionCurrency

        public static TransactionCurrencyDTO CreateTransactionCurrency(TransactionCurrencyDTO TransactionCurrency)
        {
            return Task.Run(async () => await transactionRequest.CreateTransactionCurrency(TransactionCurrency)).Result;
        }

        public static List<TransactionCurrencyDTO> SelectAllTransactionCurrency()
        {
            return Task.Run(async () => await transactionRequest.SelectAllTransactionCurrency()).Result;
        }

        public static TransactionCurrencyDTO GetTransactionCurrencyById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetTransactionCurrencyById(Id)).Result;
        }

        public static TransactionCurrencyDTO UpdateTransactionCurrency(TransactionCurrencyDTO TransactionCurrency)
        {
            return Task.Run(async () => await transactionRequest.UpdateTransactionCurrency(TransactionCurrency)).Result;
        }

        public static bool DeleteTransactionCurrencyById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteTransactionCurrencyById(Id)).Result;
        }

        #endregion
        #region TransactionReference

        public static TransactionReferenceDTO CreateTransactionReference(TransactionReferenceDTO TransactionReference)
        {
            return Task.Run(async () => await transactionRequest.CreateTransactionReference(TransactionReference)).Result;
        }

        public static List<TransactionReferenceDTO> SelectAllTransactionReference()
        {
            return Task.Run(async () => await transactionRequest.SelectAllTransactionReference()).Result;
        }

        public static TransactionReferenceDTO GetTransactionReferenceById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetTransactionReferenceById(Id)).Result;
        }

        public static TransactionReferenceDTO UpdateTransactionReference(TransactionReferenceDTO TransactionReference)
        {
            return Task.Run(async () => await transactionRequest.UpdateTransactionReference(TransactionReference)).Result;
        }

        public static bool DeleteTransactionReferenceById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteTransactionReferenceById(Id)).Result;
        }

        #endregion
        #region Voucher

        public static VoucherDTO CreateVoucher(VoucherDTO Voucher)
        {
            return Task.Run(async () => await transactionRequest.CreateVoucher(Voucher)).Result;
        }

        public static List<VoucherDTO> SelectAllVoucher()
        {
            return Task.Run(async () => await transactionRequest.SelectAllVoucher()).Result;
        }

        public static VoucherDTO GetVoucherById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetVoucherById(Id)).Result;
        }

        public static VoucherDTO UpdateVoucher(VoucherDTO Voucher)
        {
            return Task.Run(async () => await transactionRequest.UpdateVoucher(Voucher)).Result;
        }

        public static bool DeleteVoucherById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteVoucherById(Id)).Result;
        }
        #endregion
        #region Voucher Buffer
        public static ResponseModel<VoucherBuffer> CreateVoucherBuffer(VoucherBuffer Voucher)
        {
            return Task.Run(async () => await transactionRequest.CreateVoucherBuffer(Voucher)).Result;
        }
        public static VoucherDTO Patch_FS_No(int id, string FsNo, string MRC)
        {
            return Task.Run(async () => await transactionRequest.Patch_FS_No(id, FsNo, MRC)).Result;
        }
        public static VoucherDTO Patch_Voucher_LastState(int id, int LastState)
        {
            return Task.Run(async () => await transactionRequest.Patch_Voucher_LastState(id, LastState)).Result;
        }
        
        public static ResponseModel<VoucherBuffer> GetVoucherBufferById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetVoucherBufferById(Id)).Result;
        }

        public static ResponseModel<VoucherBuffer> UpdateVoucherBuffer(VoucherBuffer Voucher)
        {
            return Task.Run(async () => await transactionRequest.UpdateVoucherBuffer(Voucher)).Result;

        } 
        public static ResponseModel<bool> DeleteVoucherObjects(int Voucherid)
        {
            return Task.Run(async () => await transactionRequest.DeleteVoucherObjects(Voucherid)).Result;

        }

        #endregion
        #region VoucherAccount

        public static VoucherAccountDTO CreateVoucherAccount(VoucherAccountDTO VoucherAccount)
        {
            return Task.Run(async () => await transactionRequest.CreateVoucherAccount(VoucherAccount)).Result;
        }

        public static List<VoucherAccountDTO> SelectAllVoucherAccount()
        {
            return Task.Run(async () => await transactionRequest.SelectAllVoucherAccount()).Result;
        }

        public static VoucherAccountDTO GetVoucherAccountById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetVoucherAccountById(Id)).Result;
        }

        public static VoucherAccountDTO UpdateVoucherAccount(VoucherAccountDTO VoucherAccount)
        {
            return Task.Run(async () => await transactionRequest.UpdateVoucherAccount(VoucherAccount)).Result;
        }

        public static bool DeleteVoucherAccountById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteVoucherAccountById(Id)).Result;
        }

        #endregion
        #region VoucherConsigneeList

        public static VoucherConsigneeListDTO CreateVoucherConsigneeList(VoucherConsigneeListDTO VoucherConsigneeList)
        {
            return Task.Run(async () => await transactionRequest.CreateVoucherConsigneeList(VoucherConsigneeList)).Result;
        }

        public static List<VoucherConsigneeListDTO> SelectAllVoucherConsigneeList()
        {
            return Task.Run(async () => await transactionRequest.SelectAllVoucherConsigneeList()).Result;
        }

        public static VoucherConsigneeListDTO GetVoucherConsigneeListById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetVoucherConsigneeListById(Id)).Result;
        }

        public static VoucherConsigneeListDTO UpdateVoucherConsigneeList(VoucherConsigneeListDTO VoucherConsigneeList)
        {
            return Task.Run(async () => await transactionRequest.UpdateVoucherConsigneeList(VoucherConsigneeList)).Result;
        }

        public static bool DeleteVoucherConsigneeListById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteVoucherConsigneeListById(Id)).Result;
        }

        #endregion
        #region VoucherLookupList

        public static VoucherLookupListDTO CreateVoucherLookupList(VoucherLookupListDTO VoucherLookupList)
        {
            return Task.Run(async () => await transactionRequest.CreateVoucherLookupList(VoucherLookupList)).Result;
        }

        public static List<VoucherLookupListDTO> SelectAllVoucherLookupList()
        {
            return Task.Run(async () => await transactionRequest.SelectAllVoucherLookupList()).Result;
        }

        public static VoucherLookupListDTO GetVoucherLookupListById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetVoucherLookupListById(Id)).Result;
        }

        public static VoucherLookupListDTO UpdateVoucherLookupList(VoucherLookupListDTO VoucherLookupList)
        {
            return Task.Run(async () => await transactionRequest.UpdateVoucherLookupList(VoucherLookupList)).Result;
        }

        public static bool DeleteVoucherLookupListById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteVoucherLookupListById(Id)).Result;
        }

        #endregion
        #region VoucherTerm

        public static VoucherTermDTO CreateVoucherTerm(VoucherTermDTO VoucherTerm)
        {
            return Task.Run(async () => await transactionRequest.CreateVoucherTerm(VoucherTerm)).Result;
        }

        public static List<VoucherTermDTO> SelectAllVoucherTerm()
        {
            return Task.Run(async () => await transactionRequest.SelectAllVoucherTerm()).Result;
        }

        public static VoucherTermDTO GetVoucherTermById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetVoucherTermById(Id)).Result;
        }

        public static VoucherTermDTO UpdateVoucherTerm(VoucherTermDTO VoucherTerm)
        {
            return Task.Run(async () => await transactionRequest.UpdateVoucherTerm(VoucherTerm)).Result;
        }

        public static bool DeleteVoucherTermById(int Id)
        {
            return Task.Run(async () => await transactionRequest.DeleteVoucherTermById(Id)).Result;
        }

        #endregion
        #endregion


        #region accounting Schema
        #region Account

        public static AccountDTO CreateAccount(AccountDTO Account)
        {
            return Task.Run(async () => await accountingRequest.CreateAccount(Account)).Result;
        }

        public static List<AccountDTO> SelectAllAccount()
        {
            return Task.Run(async () => await accountingRequest.SelectAllAccount()).Result;
        }

        public static AccountDTO GetAccountById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetAccountById(Id)).Result;
        }

        public static AccountDTO UpdateAccount(AccountDTO Account)
        {
            return Task.Run(async () => await accountingRequest.UpdateAccount(Account)).Result;
        }

        public static bool DeleteAccountById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteAccountById(Id)).Result;
        }

        #endregion
        #region AccountMap

        public static AccountMapDTO CreateAccountMap(AccountMapDTO AccountMap)
        {
            return Task.Run(async () => await accountingRequest.CreateAccountMap(AccountMap)).Result;
        }

        public static List<AccountMapDTO> SelectAllAccountMap()
        {
            return Task.Run(async () => await accountingRequest.SelectAllAccountMap()).Result;
        }

        public static AccountMapDTO GetAccountMapById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetAccountMapById(Id)).Result;
        }

        public static AccountMapDTO UpdateAccountMap(AccountMapDTO AccountMap)
        {
            return Task.Run(async () => await accountingRequest.UpdateAccountMap(AccountMap)).Result;
        }

        public static bool DeleteAccountMapById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteAccountMapById(Id)).Result;
        }

        #endregion
        #region BeginingTransaction

        public static BeginingTransactionDTO CreateBeginingTransaction(BeginingTransactionDTO BeginingTransaction)
        {
            return Task.Run(async () => await accountingRequest.CreateBeginingTransaction(BeginingTransaction)).Result;
        }

        public static List<BeginingTransactionDTO> SelectAllBeginingTransaction()
        {
            return Task.Run(async () => await accountingRequest.SelectAllBeginingTransaction()).Result;
        }

        public static BeginingTransactionDTO GetBeginingTransactionById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetBeginingTransactionById(Id)).Result;
        }

        public static BeginingTransactionDTO UpdateBeginingTransaction(BeginingTransactionDTO BeginingTransaction)
        {
            return Task.Run(async () => await accountingRequest.UpdateBeginingTransaction(BeginingTransaction)).Result;
        }

        public static bool DeleteBeginingTransactionById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteBeginingTransactionById(Id)).Result;
        }

        #endregion
        #region ControlAccount

        public static ControlAccountDTO CreateControlAccount(ControlAccountDTO ControlAccount)
        {
            return Task.Run(async () => await accountingRequest.CreateControlAccount(ControlAccount)).Result;
        }

        public static List<ControlAccountDTO> SelectAllControlAccount()
        {
            return Task.Run(async () => await accountingRequest.SelectAllControlAccount()).Result;
        }

        public static ControlAccountDTO GetControlAccountById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetControlAccountById(Id)).Result;
        }

        public static ControlAccountDTO UpdateControlAccount(ControlAccountDTO ControlAccount)
        {
            return Task.Run(async () => await accountingRequest.UpdateControlAccount(ControlAccount)).Result;
        }

        public static bool DeleteControlAccountById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteControlAccountById(Id)).Result;
        }

        #endregion
        #region DepreciationRule

        public static DepreciationRuleDTO CreateDepreciationRule(DepreciationRuleDTO DepreciationRule)
        {
            return Task.Run(async () => await accountingRequest.CreateDepreciationRule(DepreciationRule)).Result;
        }

        public static List<DepreciationRuleDTO> SelectAllDepreciationRule()
        {
            return Task.Run(async () => await accountingRequest.SelectAllDepreciationRule()).Result;
        }

        public static DepreciationRuleDTO GetDepreciationRuleById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetDepreciationRuleById(Id)).Result;
        }

        public static DepreciationRuleDTO UpdateDepreciationRule(DepreciationRuleDTO DepreciationRule)
        {
            return Task.Run(async () => await accountingRequest.UpdateDepreciationRule(DepreciationRule)).Result;
        }

        public static bool DeleteDepreciationRuleById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteDepreciationRuleById(Id)).Result;
        }

        #endregion
        #region GSLAcctRequirement

        public static GslacctRequirementDTO CreateGSLAcctRequirement(GslacctRequirementDTO GSLAcctRequirement)
        {
            return Task.Run(async () => await accountingRequest.CreateGSLAcctRequirement(GSLAcctRequirement)).Result;
        }

        public static List<GslacctRequirementDTO> SelectAllGSLAcctRequirement()
        {
            return Task.Run(async () => await accountingRequest.SelectAllGSLAcctRequirement()).Result;
        }

        public static GslacctRequirementDTO GetGSLAcctRequirementById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetGSLAcctRequirementById(Id)).Result;
        }

        public static GslacctRequirementDTO UpdateGSLAcctRequirement(GslacctRequirementDTO GSLAcctRequirement)
        {
            return Task.Run(async () => await accountingRequest.UpdateGSLAcctRequirement(GSLAcctRequirement)).Result;
        }

        public static bool DeleteGSLAcctRequirementById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteGSLAcctRequirementById(Id)).Result;
        }

        #endregion
        #region JournalDetail

        public static JournalDetailDTO CreateJournalDetail(JournalDetailDTO JournalDetail)
        {
            return Task.Run(async () => await accountingRequest.CreateJournalDetail(JournalDetail)).Result;
        }

        public static List<JournalDetailDTO> SelectAllJournalDetail()
        {
            return Task.Run(async () => await accountingRequest.SelectAllJournalDetail()).Result;
        }

        public static JournalDetailDTO GetJournalDetailById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetJournalDetailById(Id)).Result;
        }

        public static JournalDetailDTO UpdateJournalDetail(JournalDetailDTO JournalDetail)
        {
            return Task.Run(async () => await accountingRequest.UpdateJournalDetail(JournalDetail)).Result;
        }

        public static bool DeleteJournalDetailById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteJournalDetailById(Id)).Result;
        }

        #endregion
        #region TrialBalance

        public static TrialBalanceDTO CreateTrialBalance(TrialBalanceDTO TrialBalance)
        {
            return Task.Run(async () => await accountingRequest.CreateTrialBalance(TrialBalance)).Result;
        }

        public static List<TrialBalanceDTO> SelectAllTrialBalance()
        {
            return Task.Run(async () => await accountingRequest.SelectAllTrialBalance()).Result;
        }

        public static TrialBalanceDTO GetTrialBalanceById(int Id)
        {
            return Task.Run(async () => await accountingRequest.GetTrialBalanceById(Id)).Result;
        }

        public static TrialBalanceDTO UpdateTrialBalance(TrialBalanceDTO TrialBalance)
        {
            return Task.Run(async () => await accountingRequest.UpdateTrialBalance(TrialBalance)).Result;
        }

        public static bool DeleteTrialBalanceById(int Id)
        {
            return Task.Run(async () => await accountingRequest.DeleteTrialBalanceById(Id)).Result;
        }

        #endregion
        #endregion


        public static DateTime? GetServiceTime()
        {
            ResponseModel<DateTime> response = Task.Run(async () => await commonRequest.GetServiceTime()).Result;
            if (response != null)
                return response.Data;
            else
                return null;
        }

        public static List<ArticleDTO> GetArticleByGSLType(int gsltype)
        {
            return Task.Run(async () => await articleRequest.GetArticleByGslType(gsltype)).Result;
        }
        public static List<VwArticleViewDTO> GetArticleViewByGslType(int gsltype)
        {
            return Task.Run(async () => await articleRequest.GetArticleViewByGslType(gsltype)).Result;
        }

        public static List<VwAccompanyingViewDTO> GetAccompanyingGuestByVoucher(int voucher)
        {
            return Task.Run(async () => await articleRequest.GetAccompanyingGuestByVoucher(voucher)).Result;
        }
        public static List<VwVoucherLineItemDetailDTO> GetVoucherLightViewByDefinition(int defintion)
        {
            return Task.Run(async () => await articleRequest.GetVoucherLightViewByDefinition(defintion)).Result;
        }
        

        public static List<PackageDetailDTO> GetPackageDetailByHeader(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetPackageDetailByHeader(Id)).Result;
        }
        public static List<PackagesToPostDTO> GetPackagesToPostByHeader(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetPackagesToPostByHeader(Id)).Result;
        }
        public static List<RateCodePackageDTO> GetRateCodePackageBypackageHeader(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodePackageBypackageHeader(Id)).Result;
        }
        public static List<RateCodePackageDTO> GetRateCodePackageByRateCodeHeader(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodePackageByRateCodeHeader(Id)).Result;
        }



        public static List<PackageHeaderDTO> GetAllPackageHeaderByConsigneeUnit(int ConsigneeUnit)
        {
            return Task.Run(async () => await pmsRequest.GetAllPackageHeaderByConsigneeUnit(ConsigneeUnit)).Result;
        }

        public static List<RoomTypeDTO> GetRoomTypeByConsigneeUnit(int consigneeUnit)
        {
            return Task.Run(async () => await pmsRequest.GetRoomTypeByConsigneeUnit(consigneeUnit)).Result;
        }

        public static List<PostingScheduleDTO> GetPostingScheduleByPackageHeaderId(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetPostingScheduleByPackageHeaderId(Id)).Result;
        }
        public static NegotiationRateDTO UpdateNegotiatedRate(NegotiationRateDTO NegotiationRate)
        {
            return Task.Run(async () => await pmsRequest.UpdateNegotiationRate(NegotiationRate)).Result;
        }

        public static List<RateCodeDetailGuestCountDTO> GetRateCodeDetailGuestCountsByRateCodeDetail(int Id)
        {
            return Task.Run(async () => await pmsRequest.GetRateCodeDetailGuestCountsByRateCodeDetail(Id)).Result;
        }

        public static List<WeekDayDTO> GetWeekDaysByReferenceandPointer(int id, int tABLE_RATE_CODE_DETAIL)
        {
            return Task.Run(async () => await pmsRequest.GetWeekDaysByReferenceandPointer(id, tABLE_RATE_CODE_DETAIL)).Result;
        }

        public static List<RateCodeDetailRoomTypeDTO> GetRateCodeDetailRoomTypeByrateCodeDetail(int rateCodeDetail)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCodeDetail", rateCodeDetail.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeDetailRoomTypeDTO>>("RateCodeDetailRoomType", Dictionaryvalue)).Result;
        }
        public static List<RateCodePackageDTO> GetRateCodePackageByRateCodeHeader2(int RateCodeHeader)
        {


            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCodeHeader", RateCodeHeader.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodePackageDTO>>("RateCodePackage", Dictionaryvalue)).Result;
        }

        public static List<ActivityViewDTO> GetActivityViewByUserandActivitDesctiption(int user, int ActivityDescription)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("user", user.ToString());
            Dictionaryvalue.Add("ActivitiyDefinitionLookup", ActivityDescription.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityViewDTO>>("ActivityView", Dictionaryvalue)).Result;
        }


        public static List<RelationDTO> GetRelationalStateByvoucher(int voucher)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("ReferencedObject", voucher.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RelationDTO>>("Relation", Dictionaryvalue)).Result;
        }
        public static List<AccountMapDTO> GetAccountMapByreference(int voucher)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Reference", voucher.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<AccountMapDTO>>("AccountMap", Dictionaryvalue)).Result;
        }
        public static List<PeriodDTO> GetPeriodByType(int Type)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("type", Type.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<PeriodDTO>>("Period", Dictionaryvalue)).Result;
        }

        public static List<VoucherExtensionDefinitionDTO> GetVoucherByExtensionDefinitionByVoucher(int voucher)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("VoucherDefinition", voucher.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherExtensionDefinitionDTO>>("VoucherExtensionDefinition", Dictionaryvalue)).Result;
        }

        public static List<TaxTransactionDTO> GetTaxTransactionByVoucher(int voucher)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Voucher", voucher.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<TaxTransactionDTO>>("TaxTransaction", Dictionaryvalue)).Result;
        }


        public static AccountMapDTO GetAccountMapByreferencefirstordefault(int Reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Reference", Reference.ToString());
                List<AccountMapDTO> retirnvalue = Task.Run(async () => await filterRequest.GetFilterData<List<AccountMapDTO>>("AccountMap", Dictionaryvalue)).Result;
                if (retirnvalue != null && retirnvalue.Count > 0)
                    return retirnvalue.FirstOrDefault();
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static UserRoleMapperDTO GetUserRoleMapperByUser(int user)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("User", user.ToString());
                List<UserRoleMapperDTO> retirnvalue = Task.Run(async () => await filterRequest.GetFilterData<List<UserRoleMapperDTO>>("UserRoleMapper", Dictionaryvalue)).Result;
                if (retirnvalue != null && retirnvalue.Count > 0)
                    return retirnvalue.FirstOrDefault();
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }


        public static GsltaxDTO GetGSLTaxByReference(int reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Reference", reference.ToString());
                List<GsltaxDTO> retirnvalue = Task.Run(async () => await filterRequest.GetFilterData<List<GsltaxDTO>>("Gsltax", Dictionaryvalue)).Result;
                if (retirnvalue != null && retirnvalue.Count > 0)
                    return retirnvalue.FirstOrDefault();
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static ObjectStateDTO GetObjectStateByReference(int reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Reference", reference.ToString());
                List<ObjectStateDTO> retirnvalue = Task.Run(async () => await filterRequest.GetFilterData<List<ObjectStateDTO>>("ObjectState", Dictionaryvalue)).Result;
                if (retirnvalue != null && retirnvalue.Count > 0)
                    return retirnvalue.FirstOrDefault();
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public static List<PackagesToPostDTO> GetPackagesToPostByRegistrationDetail(int RegistrationDetail)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("RegistrationDetail", RegistrationDetail.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<PackagesToPostDTO>>("PackagesToPost", Dictionaryvalue)).Result;
        }

        public static List<RoomTypeDTO> GetRoomTypeByConsigneeandIsActive(int Consignee, bool IsActive)
        {
            Dictionary<string, string> dickvalue = new Dictionary<string, string>();
            dickvalue.Add("consigneeunit", Consignee.ToString());
            dickvalue.Add("isactive", IsActive.ToString());

            return Task.Run(async () => await filterRequest.GetFilterData<List<RoomTypeDTO>>("RoomType", dickvalue)).Result;
        }

        public static List<RegistrationDetailDTO> GetRegistrationDetailByDateRoom(int RoomId, DateTime date)
        {
            Dictionary<string, string> dickvalue = new Dictionary<string, string>();
            dickvalue.Add("room", RoomId.ToString());
            dickvalue.Add("date", date.ToString());

            return Task.Run(async () => await filterRequest.GetFilterData<List<RegistrationDetailDTO>>("RegistrationDetail", dickvalue)).Result;
        }
        public static List<RoomTypeDTO> descriptionandconsigneeunitandroomClassandindex(string description, int consigneeunit, int roomClass, int index)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Description", description.ToString());
            Dictionaryvalue.Add("consigneeunit", consigneeunit.ToString());
            Dictionaryvalue.Add("roomClass", roomClass.ToString());
            Dictionaryvalue.Add("index", index.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RoomTypeDTO>>("RoomType", Dictionaryvalue)).Result;
        }

        public static List<RateCodeDetailDTO> GetRateCodeDetailByRateHeaderCode(int RateHeaderCode)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("ratecodeheader", RateHeaderCode.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeDetailDTO>>("RateCodeDetail", Dictionaryvalue)).Result;
        }
        public static List<RateCodeRateStrategyDTO> GetRateCodeRateStrategyByRateHeader(int RateHeaderCode)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCode", RateHeaderCode.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeRateStrategyDTO>>("RateCodeRateStrategy", Dictionaryvalue)).Result;
        }
        public static List<RateCodeRoomTypeDTO> GetRateCodeRoomTypeByrateCode(int rateCode)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("RateCodeHeader", rateCode.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeRoomTypeDTO>>("RateCodeRoomType", Dictionaryvalue)).Result;
        }
        public static List<AvailabilityCalendarDTO> GetAvailabilityCalendarBypointerandreference(int pointer, int reference)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("pointer", pointer.ToString());
            Dictionaryvalue.Add("reference", reference.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<AvailabilityCalendarDTO>>("AvailabilityCalendar", Dictionaryvalue)).Result;
        }
        public static List<RateCodeHeaderDTO> GetRateCodeHeaderByrateCatagory(int rateCatagory)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCatagory", rateCatagory.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeHeaderDTO>>("RateCodeHeader", Dictionaryvalue)).Result;
        }
        public static List<RegistrationDetailDTO> GetRegistrationDetailByrateCode(int rateCode)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCode", rateCode.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RegistrationDetailDTO>>("RegistrationDetail", Dictionaryvalue)).Result;
        }
        public static List<RateCodeHeaderDTO> GetRateCodeHeaderByconsigneeunit(int consigneeunit)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("consigneeunit", consigneeunit.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeHeaderDTO>>("RateCodeHeader", Dictionaryvalue)).Result;
        }
        public static List<HkvalueDTO> GetHKValueByreference(int reference)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("reference", reference.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<HkvalueDTO>>("HKValue", Dictionaryvalue)).Result;
        }
        public static List<NegotiationRateDTO> GetNegotiationRateByrateCode(int rateCode)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCode", rateCode.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<NegotiationRateDTO>>("NegotiationRate", Dictionaryvalue)).Result;
        }
        public static List<NegotiationRateDTO> GetNegotiationRateByConsignee(int rateCode)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("consignee", rateCode.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<NegotiationRateDTO>>("NegotiationRate", Dictionaryvalue)).Result;
        }
        public static List<RateCodePackageDTO> GetRateCodePackagesByrateCodeHeader(int rateCodeHeader)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCodeHeader", rateCodeHeader.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodePackageDTO>>("RateCodePackage", Dictionaryvalue)).Result;
        }
        public static List<PostingScheduleDTO> GetPostingScheduleBypackageHeader(int packageHeader)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("packageHeader", packageHeader.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<PostingScheduleDTO>>("PostingSchedule", Dictionaryvalue)).Result;
        }
        public static List<WeekDayDTO> GetWeekDaysByreference(int reference)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("reference", reference.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<WeekDayDTO>>("WeekDays", Dictionaryvalue)).Result;
        }
        public static List<RateCodeDetailGuestCountDTO> GetRateCodeDetailGuestCountByrateCodeDetail(int rateCodeDetail)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateCodeDetail", rateCodeDetail.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeDetailGuestCountDTO>>("RateCodeDetailGuestCount", Dictionaryvalue)).Result;
        }
        public static List<RoomTypeRateStrategyDTO> GetRoomTypeRateStrategyByrateStrategy(int rateStrategy)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateStrategy", rateStrategy.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RoomTypeRateStrategyDTO>>("RoomTypeRateStrategy", Dictionaryvalue)).Result;
        }
        public static List<RateCategoryRateStrategyDTO> GetRateCategoryRateStrategyByrateStrategy(int rateStrategy)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateStrategy", rateStrategy.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCategoryRateStrategyDTO>>("RateCategoryRateStrategy", Dictionaryvalue)).Result;
        }
        public static List<RateCodeRateStrategyDTO> GetRateCodeRateStrategyByrateStrategy(int rateStrategy)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("rateStrategy", rateStrategy.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeRateStrategyDTO>>("RateCodeRateStrategy", Dictionaryvalue)).Result;
        }
        public static List<RoomFeatureDTO> GetRoomFeaturesByreference(int reference)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("reference", reference.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RoomFeatureDTO>>("RoomFeature", Dictionaryvalue)).Result;
        }
        public static List<RoomDetailDTO> GetRoomDetailByroomType(int roomType)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("roomType", roomType.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RoomDetailDTO>>("RoomDetail", Dictionaryvalue)).Result;
        }
        public static List<RateCodeRoomTypeDTO> GetRateCodeRoomTypeByroomType(int roomType)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("roomType", roomType.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeRoomTypeDTO>>("RateCodeRoomType", Dictionaryvalue)).Result;
        }
        public static List<RateCodeDetailRoomTypeDTO> GetRateCodeDetailRoomTypeByroomType(int roomType)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("roomType", roomType.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RateCodeDetailRoomTypeDTO>>("RateCodeDetailRoomType", Dictionaryvalue)).Result;
        }
        public static List<RegistrationDetailDTO> GetRegistrationDetailByroom(int room)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("room", room.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RegistrationDetailDTO>>("RegistrationDetail", Dictionaryvalue)).Result;
        }
        public static List<RoomTypeDTO> GetRoomTypeBycomponentRoom(int componentRoom)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("componentRoom", componentRoom.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<RoomTypeDTO>>("RoomType", Dictionaryvalue)).Result;
        }
        public static RoomDetailDTO GetRoomDetailByspace(int space)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("space", space.ToString());
            var ret = Task.Run(async () => await filterRequest.GetFilterData<List<RoomDetailDTO>>("RoomDetail", Dictionaryvalue)).Result;

            if (ret != null)
                return ret.FirstOrDefault();
            else
                return null;
        }
        public static List<ActivityDefinitionDTO> GetActivityDefinitionBycomponetanddescription(int componet, int description)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Description", description.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDefinitionDTO>>("ActivityDefinition", Dictionaryvalue)).Result;
        }
        public static List<KeyDefinitionDTO> GetKeyDefinitionByspace(int space)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("space", space.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<KeyDefinitionDTO>>("KeyDefinition", Dictionaryvalue)).Result;
        }
        public static List<ConsigneeDTO> GetConsigneeBygslType(int gslType)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("gslType", gslType.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ConsigneeDTO>>("Consignee", Dictionaryvalue)).Result;
        }
        public static List<ConsigneeUnitDTO> GetConsigneeUnitByconsignee(int consignee)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("consignee", consignee.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", Dictionaryvalue)).Result;
        }
        public static List<ValueFactorDTO> GetValueFactorByreference(int consignee)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Reference", consignee.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ValueFactorDTO>>("ValueFactor", Dictionaryvalue)).Result;
        }
        public static List<ConsigneeUnitDTO> GetConsigneeUnitByconsigneeandtype(int consignee, int type)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("consignee", consignee.ToString());
            Dictionaryvalue.Add("type", type.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", Dictionaryvalue)).Result;
        }

        public static List<ConsigneeDTO> GetConsigneeBygslTypeandisActive(int gslType, bool isActive)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("gslType", gslType.ToString());
                Dictionaryvalue.Add("isActive", isActive.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ConsigneeDTO>>("Consignee", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetConsigneeBygslTypeandisActive" + Environment.NewLine + ex.Message);
                return new List<ConsigneeDTO>();
            }
        }
        public static RateAdjustmentDTO GetRateAdjustmentByvoucher(int voucher)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("voucher", voucher.ToString());
                List<RateAdjustmentDTO> ret = Task.Run(async () => await filterRequest.GetFilterData<List<RateAdjustmentDTO>>("RateAdjustment", Dictionaryvalue)).Result;


                if (ret != null)
                    return ret.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRateAdjustmentByvoucher" + Environment.NewLine + ex.Message);
                return new RateAdjustmentDTO();
            }
        }


        public static List<RoomTypeDTO> GetRoomTypeByispseudoRoomType(bool ispseudoRoomType)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("ispseudoRoomType", ispseudoRoomType.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<RoomTypeDTO>>("RoomType", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoomTypeByispseudoRoomType" + Environment.NewLine + ex.Message);
                return new List<RoomTypeDTO>();
            }
        }
        public static List<ActivityDTO> GetActivityByreferenceandactivityDefinition(int reference, int activityDefinition)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("reference", reference.ToString());
                Dictionaryvalue.Add("activityDefinition", activityDefinition.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDTO>>("Activity", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityByreferenceandactivityDefinition" + Environment.NewLine + ex.Message);
                return new List<ActivityDTO>();
            }
        }
        public static ArticleDTO GetArticleByname(string name)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("name", name.ToString());
                List<ArticleDTO> articleslist = Task.Run(async () => await filterRequest.GetFilterData<List<ArticleDTO>>("Article", Dictionaryvalue)).Result;
                if (articleslist != null && articleslist.Count > 0)
                    return articleslist.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetArticleByname" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static RegistrationPrivllegeDTO GetRegistrationPrivllegeByvoucher(int voucher)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("voucher", voucher.ToString());
                List<RegistrationPrivllegeDTO> articleslist = Task.Run(async () => await filterRequest.GetFilterData<List<RegistrationPrivllegeDTO>>("RegistrationPrivllege", Dictionaryvalue)).Result;
                if (articleslist != null && articleslist.Count > 0)
                    return articleslist.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationPrivllegeByvoucher" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<DailyResidentSummaryDTO> GetDailyResidentSummaryBydateandconsigneeUnit(DateTime date, int consigneeUnit)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("date", date.ToString());
                Dictionaryvalue.Add("consigneeUnit", consigneeUnit.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<DailyResidentSummaryDTO>>("DailyResidentSummary", Dictionaryvalue)).Result;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDailyResidentSummaryBydateandconsigneeUnit" + Environment.NewLine + ex.Message);
                return new List<DailyResidentSummaryDTO>();
            }
        }
        public static List<VoucherDTO> GetVoucherBydefinitionandissuedDate(int definition, DateTime issuedDate)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("definition", definition.ToString());
                Dictionaryvalue.Add("issuedDate", issuedDate.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherDTO>>("Voucher", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherBydefinitionandissuedDate" + Environment.NewLine + ex.Message);
                return new List<VoucherDTO>();
            }
        }
        public static List<VoucherDTO> GetVoucherBydefinitionandoriginConsigneeUnitandissuedDate(int definition, int originConsigneeUnit, DateTime issuedDate)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("definition", definition.ToString());
                Dictionaryvalue.Add("originConsigneeUnit", originConsigneeUnit.ToString());
                Dictionaryvalue.Add("issuedDate", issuedDate.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherDTO>>("Voucher", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherBydefinitionandoriginConsigneeUnitandissuedDate" + Environment.NewLine + ex.Message);
                return new List<VoucherDTO>();
            }
        }


        public static DeviceDTO GetDeviceByname(string name)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("machineName", name.ToString());
                List<DeviceDTO> returndevicelist = Task.Run(async () => await filterRequest.GetFilterData<List<DeviceDTO>>("Device", Dictionaryvalue)).Result;
                if (returndevicelist != null && returndevicelist.Count > 0)
                    return returndevicelist.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDeviceByname" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static List<DeviceDTO> GetDeviceByConsigneeunit(int consigneeunit)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("ConsigneeUnit", consigneeunit.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<DeviceDTO>>("Device", Dictionaryvalue)).Result;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDeviceByConsigneeunit" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static List<HkassignmentDTO> GetHKAssignmentByConsigneeunit(int consigneeunit)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("ConsigneeUnit", consigneeunit.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<HkassignmentDTO>>("Hkassignment", Dictionaryvalue)).Result;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetHKAssignmentByConsigneeunit" + Environment.NewLine + ex.Message);
                return null;
            }
        }


        public static List<RegistrationDetailDTO> GetRegistrationDetailByvoucher(int voucher)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("voucher", voucher.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<RegistrationDetailDTO>>("RegistrationDetail", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationDetailByvoucher" + Environment.NewLine + ex.Message);
                return new List<RegistrationDetailDTO>();
            }
        }
        public static List<RegistrationDetailDTO> GetRegistrationDetailByvoucherAndDate(int voucher, DateTime Date)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("voucher", voucher.ToString());
                Dictionaryvalue.Add("Date", Date.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<RegistrationDetailDTO>>("RegistrationDetail", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationDetailByvoucher" + Environment.NewLine + ex.Message);
                return new List<RegistrationDetailDTO>();
            }
        }
        public static List<RegistrationDetailDTO> GetRegistrationDetailByRoomAndDate(int room, DateTime Date)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("room", room.ToString());
                Dictionaryvalue.Add("Date", Date.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<RegistrationDetailDTO>>("RegistrationDetail", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationDetailByRoomAndDate" + Environment.NewLine + ex.Message);
                return new List<RegistrationDetailDTO>();
            }
        }



        public static List<ActivityDefinitionDTO> GetActivityDefinitionBydescriptionandreference(int description, int reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("description", description.ToString());
                Dictionaryvalue.Add("reference", reference.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDefinitionDTO>>("ActivityDefinition", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityDefinitionByreferenceanddescription" + Environment.NewLine + ex.Message);
                return new List<ActivityDefinitionDTO>();
            }
        }
        public static List<ActivityDefinitionDTO> GetActivityDefinitionByreference(int reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("reference", reference.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDefinitionDTO>>("ActivityDefinition", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityDefinitionByreferenceanddescription" + Environment.NewLine + ex.Message);
                return new List<ActivityDefinitionDTO>();
            }
        }
        public static List<ActivityDTO> GetActivityByreference(int reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("reference", reference.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDTO>>("Activity", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityByreference" + Environment.NewLine + ex.Message);
                return new List<ActivityDTO>();
            }
        }
        public static List<ActivityDefinitionDTO> GetActivityDefinitionByDescription(int Description)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("description", Description.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDefinitionDTO>>("ActivityDefinition", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityDefinitionByDescription" + Environment.NewLine + ex.Message);
                return new List<ActivityDefinitionDTO>();
            }
        }

        public static List<TravelDetailDTO> GetTravelDetailByvoucher(int reference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("voucher", reference.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<TravelDetailDTO>>("TravelDetail", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTravelDetailByvoucher" + Environment.NewLine + ex.Message);
                return new List<TravelDetailDTO>();
            }
        }

        public static List<VwConsigneeViewDTO> GetConsigneeViewByGslType(int GslType)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("gslType", GslType.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VwConsigneeViewDTO>>("VwConsigneeView", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetConsigneeViewByGslType" + Environment.NewLine + ex.Message);
                return new List<VwConsigneeViewDTO>();
            }
        }
        public static List<VwConsigneeViewDTO> SelectAllConsigneeView()
        {
            try
            {
                return Task.Run(async () => await consigneeRequest.SelectAllConsigneeView()).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: SelectAllConsigneeView" + Environment.NewLine + ex.Message);
                return new List<VwConsigneeViewDTO>();
            }
        }

        //public static VwConsigneeViewDTO GetConsigneeViewById(int Id)
        //{
        //    try
        //    {
        //        return Task.Run(async () => await consigneeRequest.GetConsigneeViewById(Id)).Result; 
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: GetConsigneeViewById" + Environment.NewLine + ex.Message);
        //        return null;
        //    }
        //}
        public static VwConsigneeViewDTO GetConsigneeViewById(int Id)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Id", Id.ToString());
                var returnval = Task.Run(async () => await filterRequest.GetFilterData<List<VwConsigneeViewDTO>>("VwConsigneeView", Dictionaryvalue)).Result;

                if (returnval != null && returnval.Count > 0)
                    return returnval.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetConsigneeViewById" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static List<IdentificationDTO> GetIdentificationByconsigneeandtype(int consignee, int type)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("consignee", consignee.ToString());
                Dictionaryvalue.Add("type", type.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<IdentificationDTO>>("Identification", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetIdentificationByconsigneeandtype" + Environment.NewLine + ex.Message);
                return new List<IdentificationDTO>();
            }
        }
        public static List<TransactionReferenceDTO> GetTransactionReferenceByreferenced(int referenced)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("referenced", referenced.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<TransactionReferenceDTO>>("TransactionReference", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTransactionReferenceByreferenced" + Environment.NewLine + ex.Message);
                return new List<TransactionReferenceDTO>();
            }
        }
        public static List<TransactionReferenceDTO> GetTransactionReferenceByreferring(int referring)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("referring", referring.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<TransactionReferenceDTO>>("TransactionReference", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTransactionReferenceByreferring" + Environment.NewLine + ex.Message);
                return new List<TransactionReferenceDTO>();
            }
        }
        public static DeviceDTO GetDeviceByhostandpreference(int host, int preference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("host", host.ToString());
                Dictionaryvalue.Add("preference", preference.ToString());
                List<DeviceDTO> returnval = Task.Run(async () => await filterRequest.GetFilterData<List<DeviceDTO>>("Device", Dictionaryvalue)).Result;
                if (returnval != null && returnval.Count > 0)
                    return returnval.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDeviceByhostandpreference" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static List<VoucherDTO> GetVoucherBydefinitionandconsignee1andremark(int definition, int consignee1, string remark)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("definition", definition.ToString());
                Dictionaryvalue.Add("consignee1", consignee1.ToString());
                Dictionaryvalue.Add("remark", remark.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherDTO>>("Voucher", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherBydefinitionandconsignee1andremark" + Environment.NewLine + ex.Message);
                return new List<VoucherDTO>();
            }
        }

        public static List<VoucherDTO> GetVoucherByExtension1(string Extension1)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Extension1", Extension1);
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherDTO>>("Voucher", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherByExtension1" + Environment.NewLine + ex.Message);
                return new List<VoucherDTO>();
            }
        }

        public static List<PreferenceAccessDTO> GetPreferenceAccessByvoucherDfnanddevice(int voucherDfn, int device)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("voucherDfn", voucherDfn.ToString());
                Dictionaryvalue.Add("device", device.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<PreferenceAccessDTO>>("PreferenceAccess", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPreferenceAccessByvoucherDfnanddevice" + Environment.NewLine + ex.Message);
                return new List<PreferenceAccessDTO>();
            }
        }



        public static List<ValueDTO> GetValueByArticle(int Article)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Article", Article.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ValueDTO>>("Value", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetValueByArticle" + Environment.NewLine + ex.Message);
                return new List<ValueDTO>();
            }
        }

        public static VwVoucherHeaderLightDTO GetVwVoucherHeaderLightById(int Id)
        {
            try
            {
                return Task.Run(async () => await transactionRequest.GetVwVoucherHeaderLightById(Id)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVwVoucherHeaderLightById" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<ActivityDTO> GetActivityByactivityDefinitionanduser(int activityDefinition, int user)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("activityDefinition", activityDefinition.ToString());
                Dictionaryvalue.Add("user", user.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDTO>>("Activity", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityByactivityDefinitionanduser" + Environment.NewLine + ex.Message);
                return new List<ActivityDTO>();
            }
        }
        public static List<ActivityDTO> GetActivityByactivityDefinitionandDate(int activityDefinition, DateTime date)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("activityDefinition", activityDefinition.ToString());
                Dictionaryvalue.Add("TimeStamp", date.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<ActivityDTO>>("Activity", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityByactivityDefinitionandDate" + Environment.NewLine + ex.Message);
                return new List<ActivityDTO>();
            }
        }
        public static List<VwVoucherHeaderLightDTO> GetVoucherDocument(int vouchercode, int consignee)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();

                if (vouchercode != null)
                    Dictionaryvalue.Add("Id", vouchercode.ToString());

                if (consignee != null)
                    Dictionaryvalue.Add("Consigee1", consignee.ToString());


                return Task.Run(async () => await filterRequest.GetFilterData<List<VwVoucherHeaderLightDTO>>("VwVoucherHeaderLight", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVocuherDocument" + Environment.NewLine + ex.Message);
                return new List<VwVoucherHeaderLightDTO>();
            }
        }

        public static List<VoucherConsigneeListDTO> GetVoucherConsigneeListByconsignee(int consignee)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("consignee", consignee.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherConsigneeListDTO>>("VoucherConsigneeList", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherConsigneeListByconsignee" + Environment.NewLine + ex.Message);
                return new List<VoucherConsigneeListDTO>();
            }
        }

        public static List<VoucherConsigneeListDTO> GetVoucherConsigneeListByconsigneeandVoucher(int consignee, int Voucher)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("consignee", consignee.ToString());
                Dictionaryvalue.Add("Voucher", Voucher.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherConsigneeListDTO>>("VoucherConsigneeList", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherConsigneeListByconsignee" + Environment.NewLine + ex.Message);
                return new List<VoucherConsigneeListDTO>();
            }
        }

        public static List<VoucherConsigneeListDTO> GetVoucherConsigneeListByVoucher(int Voucher)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Voucher", Voucher.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherConsigneeListDTO>>("VoucherConsigneeList", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherConsigneeListByconsignee" + Environment.NewLine + ex.Message);
                return new List<VoucherConsigneeListDTO>();
            }
        }

        public static List<RoleActivityDTO> GetRoleActivityByroleandactivityDefinition(int role, int activityDefinition)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("role", role.ToString());
                Dictionaryvalue.Add("activityDefinition", activityDefinition.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<RoleActivityDTO>>("RoleActivity", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoleActivityByroleandactivityDefinition" + Environment.NewLine + ex.Message);
                return new List<RoleActivityDTO>();
            }
        }

        public static List<KeyDefinitionDTO> GetKeyDefinitionBySpace(int Space)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Space", Space.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<KeyDefinitionDTO>>("KeyDefinition", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetKeyDefinitionBySpace" + Environment.NewLine + ex.Message);
                return new List<KeyDefinitionDTO>();
            }
        }


        public static List<KeyOptionDTO> GetKeyOptionByRoom(int RoomDetail)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("RoomDetail", RoomDetail.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<KeyOptionDTO>>("KeyOption", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetKeyOptionByRoom" + Environment.NewLine + ex.Message);
                return new List<KeyOptionDTO>();
            }
        }

        public static List<RoleActivityDTO> GetRoleActivityByactivityDefinition(int activityDefinition)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("activityDefinition", activityDefinition.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<RoleActivityDTO>>("RoleActivity", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoleActivityByactivityDefinition" + Environment.NewLine + ex.Message);
                return new List<RoleActivityDTO>();
            }
        }

        public static VoucherDTO GetVoucherByCode(string code)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Code", code.ToString());
                List<VoucherDTO> returnval = Task.Run(async () => await filterRequest.GetFilterData<List<VoucherDTO>>("Voucher", Dictionaryvalue)).Result;

                if (returnval != null && returnval.Count > 0)
                    return returnval.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherBylocalCode" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static List<VoucherDTO> GetVoucherByDefinition(int Definition)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Definition", Definition.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherDTO>>("Voucher", Dictionaryvalue)).Result;
 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherBylocalCode" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        public static CountryDTO GetCountryByIcaocountryCode(string code)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("IcaocountryCode", code.ToString());
                List<CountryDTO> returnval = Task.Run(async () => await filterRequest.GetFilterData<List<CountryDTO>>("Country", Dictionaryvalue)).Result;

                if (returnval != null && returnval.Count > 0)
                    return returnval.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetCountryByIcaocountryCode" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<GslacctRequirementDTO> GetGSLAcctRequirementBygslTypeList(int gslTypeList)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("gslTypeList", gslTypeList.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<GslacctRequirementDTO>>("GSLAcctRequirement", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetGSLAcctRequirementBygslTypeList" + Environment.NewLine + ex.Message);
                return new List<GslacctRequirementDTO>();
            }
        }

        public static List<LineItemDTO> GetLineItemByvoucher(int voucher)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("voucher", voucher.ToString());
                return Task.Run(async () => await filterRequest.GetFilterData<List<LineItemDTO>>("LineItem", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetLineItemByvoucher" + Environment.NewLine + ex.Message);
                return new List<LineItemDTO>();
            }
        }

        public static ControlAccountDTO GetControlAccountByid(int id)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("id", id.ToString());
                List<ControlAccountDTO> listvalue = Task.Run(async () => await filterRequest.GetFilterData<List<ControlAccountDTO>>("ControlAccount", Dictionaryvalue)).Result;

                if (listvalue != null)
                    return listvalue.FirstOrDefault();
                else
                    return null;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetControlAccountByid" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RegistrationDocumentDTO> GetRegistrationDTOData(DateTime? startdate, DateTime? EndDate, int? state, int consigneeunit)
        {
            try
            {

                return Task.Run(async () => await pmsLibraryRequest.GetRegistrationDTOData(startdate, EndDate, state, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationDTOData" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RegistrationListVMDTO> GetRegistrationViewModelData(DateTime? startdate, DateTime? EndDate, int? state, int consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetRegistrationViewModelData(startdate, EndDate, state, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationViewModelData" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<Tuple<string, int>> GetAvailableRoomCount(List<RoomTypeDTO> roomTypes, DateTime start, DateTime end)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAvailableRoomCount(roomTypes, start, end)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAvailableRoomCount" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<Tuple<string, int, int>> GetAvailableRoom(List<RoomTypeDTO> roomTypes, DateTime start, DateTime end)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAvailableRoom(roomTypes, start, end)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAvailableRoom" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static AvailableRateGeneratorDTO GetAvailableRates(RegistrationInfoDTO registrationInfo, int selectedHotelcode)
        {

            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAvailableRates(registrationInfo, selectedHotelcode)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAvailableRates" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<GeneratedRegistrationDTO> RegistrationDetailGenerator(int voucherId, DateTime arrivalDate, DateTime departureDate, RegistrationDetailDTO registrationDetail, List<DailyRateCodeDTO> dailyRateCodeDTOs)
        {

            try
            {
                return Task.Run(async () => await pmsLibraryRequest.RegistrationDetailGenerator(voucherId, arrivalDate, departureDate, registrationDetail, dailyRateCodeDTOs)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: RegistrationDetailGenerator" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<GeneratedRegistrationDTO> AmendRegistrationDate(int registrationState, RegistrationDetailDTO lastRegDetail, RegistrationInfoDTO registrationDTO, DateTime newArriveDate, DateTime newDepartureDate, int branch)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.AmendRegistrationDate(registrationState, lastRegDetail, registrationDTO, newArriveDate, newDepartureDate, branch)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: AmendRegistrationDate" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwVoucherDetailWithRoomDetailViewDTO> GetAvailabeRoomsByDateAndState(DateTime startdate, DateTime EndDate, List<int> state, int? roomtype)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAvailabeRoomsByDateAndState(startdate, EndDate, state, roomtype)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAvailabeRoomsByDateAndState" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwRoomPoschargeViewDTO> GetAllRoomPosCharges(DateTime startdate, DateTime EndDate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAllRoomPosCharges(startdate, EndDate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAllRoomPosCharges" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static string IdGenerater(string Type, int reference, int generationtype, int consigneeunit, bool isweb, int deviceid)
        {
            return Task.Run(async () => await commonRequest.IdGenerater(Type, reference, generationtype, consigneeunit, isweb, deviceid)).Result.ToString();
        }

        public static List<VwRouteDetailDTO> GetAllRouteView()
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAllRouteView()).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAllRouteView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwRoomFeatureViewDTO> GetRoomFeatureView()
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetRoomFeatureView()).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoomFeatureView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RegistrationStatusDTO> GetRegistrationStatusList(List<int> room, DateTime date)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetRegistrationStatusList(room, date)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationStatusList" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static RegistrationStatusDTO GetRegistrationStatus(int room, DateTime date)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetRegistrationStatus(room, date)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationStatus" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static bool RoomMove(int voucherId, DateTime arrivalDate, DateTime departureDate, RegistrationDetailDTO registrationDetail, List<DailyRateCodeDTO> dailyRateCodeList)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.RoomMove(voucherId, arrivalDate, departureDate, registrationDetail, dailyRateCodeList)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: RoomMove" + Environment.NewLine + ex.Message);
                return false;
            }
        }

        public static DailyRoomChargeDTO GetDailyRoomChargePostingByRegistration(int regCode, DateTime date, int definition, int laststate, TaxDTO applicableTax, int? lateCheckoutPunshment, int? EarlyCheckInArticle)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetDailyRoomChargePostingByRegistration(regCode, date, definition, laststate, applicableTax, lateCheckoutPunshment, EarlyCheckInArticle)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDailyRoomChargePostingByRegistration" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<DailyRoomChargeDTO> GetAllDailyRoomChargePosting(DateTime date, int definition, int laststate, TaxDTO? applicableTax)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAllDailyRoomChargePosting(date, definition, laststate, applicableTax)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAllDailyRoomChargePosting" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static GuestLedgerDTO GetGuestLedger(int regCode, DateTime arrival, DateTime departure, string roomNumber, int? window)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetGuestLedger(regCode, arrival, departure, roomNumber, window)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetGuestLedger" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RoomDetailDTO> GetUnassignedRoomsByState(DateTime arrivalDate, DateTime departureDate, int lastObjectState)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetUnassignedRoomsByState(arrivalDate, departureDate, lastObjectState)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetUnassignedRoomsByState" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static decimal ApplyExchangeRate(int registrationNo, int exchangeRule, string processOperation, int currency)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.ApplyExchangeRate(registrationNo, exchangeRule, processOperation, currency)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: ApplyExchangeRate" + Environment.NewLine + ex.Message);
                return 1;
            }
        }

        public static decimal GetLatestExchangeRate(int currency)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetLatestExchangeRate(currency)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetLatestExchangeRate" + Environment.NewLine + ex.Message);
                return 1;
            }
        }

        public static List<int> GetDailyRoomVoucherByReg(int registrationNo, DateTime date)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetDailyRoomVoucherByReg(registrationNo, date)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDailyRoomVoucherByReg" + Environment.NewLine + ex.Message);
                return new List<int>();
            }
        }

        public static List<VwRegistrationDocumentViewDTO> GetRegistrationDocumentViewByStartdateEnddateStateandConsigneeUnit(DateTime startdate, DateTime EndDate, int? state, int consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetRegistrationDocumentViewByStartdateEnddateStateandConsigneeUnit(startdate, EndDate, state, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationDocumentViewByStartdateEnddateStateandConsigneeUnit" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwRegistrationDocumentViewDTO> GetRegistrationDocumentViewByDate(DateTime Date)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetRegistrationDocumentViewByDate(Date)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationDocumentViewByDate" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static RegistrationDetailDTO GetLastRegistration(int RegistrationId)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetLastRegistration(RegistrationId)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetLastRegistration" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwTransactionReferenceFSnumberViewDTO> GetTransactionReferenceFSnumberViewById(int RegistrationId)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetTransactionReferenceFSnumberViewById(RegistrationId)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTransactionReferenceFSnumberViewById" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VoucherReconcilationDTO> GetVoucherReconcilationByType(string name, int reference, DateTime startDate, DateTime endDate)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetVoucherReconcilationByType(name, reference, startDate, endDate)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherReconcilationByType" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static decimal GetDiscount(int voucher, int Definition, int? window)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetDiscount(voucher, Definition, window)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDiscount" + Environment.NewLine + ex.Message);
                return 0;
            }
        }

        public static List<VwTransferBillViewDTO> GetTransferBill(int origin, DateTime? date, bool source)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetTransferBillBySourceOrDestination(origin, date, source)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTransferBill" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwLineItemDetailViewDTO> GetLineItemDetailByVoucher(int Registration)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetLineItemDetailByVoucher(Registration)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTransferBillBySource" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwPackageToPostViewDTO> GetPostingPackageToPostViewByRegistrationDetail(int Registration)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetPostingPackageToPostViewByRegistrationDetail(Registration)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPostingPackageToPostViewByRegistrationDetail" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwPackageToPostViewDTO> GetPostingPackageToPostViewByRegistrationCode(int Registration)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetPostingPackageToPostViewByRegistrationCode(Registration)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPostingPackageToPostViewByRegistrationCode" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwDailyChargePostingViewDTO> GetCheckOutDetailViewByVoucher(int Registration, int definition, int? Window)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetCheckOutDetailViewByVoucher(Registration, definition, Window)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetCheckOutDetailViewByVoucher" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwCheckoutDetailViewDTO> GetCheckOutDetailViewByPrintStatus(int Registration, int definition, int? Window, int value)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetCheckOutDetailViewByPrintStatus(Registration, definition, Window, value)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetCheckOutDetailViewByPrintStatus" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwCountryAndCityViewDTO> GetAllCountryAndCityView()
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAllCountryAndCityView()).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetAllCountryAndCityView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwScheduleDetailViewDTO> GetScheduleDetailByvoucherandType(int? voucher, int type)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetScheduleDetailByvoucherandType(voucher, type)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetScheduleDetailByvoucherandType" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwMessageViewDTO> GetMessageViewByRegistrationId(int Registration)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetMessageViewByRegistrationId(Registration)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetMessageViewByRegistrationId" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwTravelDetailDTO> GetTravelDetailByRegistrationId(int Registration)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetTravelDetailByRegistrationId(Registration)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTravelDetailByRegistrationId" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwServiceRequestDTO> GetServiceRequestByRegistrationId(int Registration)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetServiceRequestByRegistrationId(Registration)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetServiceRequestByRegistrationId" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static decimal GetUnitRoomRate(int rateCodeDetail, int roomTypeCode, int childCount, int adultCount)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetUnitRoomRate(rateCodeDetail, roomTypeCode, childCount, adultCount)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetUnitRoomRate" + Environment.NewLine + ex.Message);
                return 0;
            }
        }

        public static List<VwRoomManagmentViewDTO> GetAllRoomManagment(int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetAllRoomManagmentView(consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetUnitRoomRate" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        #region Report 

        public static List<RegistrationReportInfoDTO> GetRegistrationByFilter(string type, DateTime date, List<int> statelist, int? consgineeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRegistrationByFilter(type, date, statelist, consgineeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationByFilter" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RegistrationNonLinitemVoucherDTO> GetRegistrationNonLinitemVoucher(DateTime startdate, DateTime enddate, bool IsCheckout, int? consigneeunit, List<int>? Definition, int? Consignee, int? Company)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRegistrationNonLinitemVoucher(startdate, enddate, IsCheckout, consigneeunit, Definition, Consignee, Company)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationNonLinitemVoucher" + Environment.NewLine + ex.Message);
                return null;
            }
        }
      
        public static List<RegistrationWithPaymentMethodDTO> GetRegistrationWithPaymentMethodVoucher(DateTime startdate, DateTime enddate, bool IsCheckout, int? consigneeunit, List<int>? Definition, int? Consignee, int? Company)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRegistrationWithPaymentMethodVoucher(startdate, enddate, IsCheckout, consigneeunit, Definition, Consignee, Company)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationWithPaymentMethodVoucher" + Environment.NewLine + ex.Message);
                return null;
            }
        }
       
        public static List<RegistrationPaymentMethodSummaryDTO> GetRegistrationPaymentMethodSummary(DateTime startdate, DateTime enddate, bool IsCheckout, int? consigneeunit, List<int>? Definition, int? Consignee, int? Company)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRegistrationPaymentMethodSummary(startdate, enddate, IsCheckout, consigneeunit, Definition, Consignee, Company)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationPaymentMethodSummary" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RegistrationVoucherDTO> GetRegistrationVoucher(DateTime startdate, DateTime enddate, bool IsCheckout, int? consigneeunit, List<int>? Definition, int? Consignee, int? Company)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRegistrationVoucher(startdate, enddate, IsCheckout, consigneeunit, Definition, Consignee, Company)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationVoucher" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<CityLedgerDTO> GetRegistrationCityLedger(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRegistrationCityLedger(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRegistrationCityLedger" + Environment.NewLine + ex.Message);
                return null;
            }
        }
      
        public static List<int> GetCustomerHighBalance(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetCustomerHighBalance(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetCustomerHighBalance" + Environment.NewLine + ex.Message);
                return null;
            }
        }
     
        public static ResponseModel<List<DailyResidentSummaryReport>> GetDailyResidentSummary(DateTime date, int? consigneeunit, int deviceid)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetDailyResidentSummary(date, consigneeunit, deviceid)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDailyResidentSummary" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<DailyBusinessReportDTO> GetDailyBusinessReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetDailyBusinessReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDailyBusinessReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<ForeignExchangeReport> GetForeignExchangeDetailReport(DateTime date, int definition, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetForeignExchangeDetailReport(date, definition, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetForeignExchangeDetailReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<ForignExchangeDTO> GetForeignExchangeSummaryReport(DateTime date, int definition, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetForeignExchangeSummaryReport(date, definition, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetForeignExchangeSummaryReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<PoliceReportDTO> GetPoliceReports(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetPoliceReports(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPoliceReports" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<BillTransferReport> GetBillTransferDetail(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetBillTransferDetail(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetBillTransferDetail" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<ManagerialFlashReport> GetManagerialFlashReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetManagerialFlashReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetManagerialFlashReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RateAdjustmentReport> GetRateAdjustmentReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRateAdjustmentReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRateAdjustmentReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<PackageReportDTO> GetPackageReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetPackageReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPackageReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<FiscalReconciliationReportDTO> GetPMSFiscalReconciliationReport(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetPMSFiscalReconciliationReport(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPMSFiscalReconciliationReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RoomPOSChargesReportDTO> GetRoomPOSCharges(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRoomPOSCharges(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoomPOSCharges" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<ResidentSummaryOfSummaryReportDTO> GetDailyResidentSummaryofSummary(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetDailyResidentSummaryofSummary(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoomPOSCharges" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<DepositLedgerReportDTO> GetDepositLedger(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetDepositLedger(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDepositLedger" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RoomIncomeReport> GetRoomIncomeReport(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRoomIncomeReport(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDepositLedger" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RateCheckReport> GetRateCheckReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRateCheckReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRateCheckReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<TrialBalanceReport> GetTrialBalanceReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetTrialBalanceReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTrialBalanceReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<DetailDailySalesTransactionsReportDTO> GetDetailDailySalesTransaction(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetDetailDailySalesTransaction(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDetailDailySalesTransaction" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<TravelDetailReportDTO> GetTravelDetailReport(int Type, DateTime dateTime, int? Guest, int? Consgneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetTravelDetailReport(Type, dateTime, Guest, Consgneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTravelDetailReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static CasherSummaryReportDTO GetCasherSummary(DateTime date, int? user, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetCasherSummary(date, user, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetCasherSummary" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwVoucherHeaderWithRegistrationViewDTO> GetDailySalesSummary(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetDailySalesSummary(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDailySalesSummary" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<CashDropDocumentReportDTO> GetCashDropreport(DateTime startdate, DateTime enddate, int? consigneeunit, int? User)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetCashDropreport(startdate, enddate, consigneeunit, User)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetCashDropreport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<NationalityReportDTO> GetNationalityReport(DateTime startdate, DateTime enddate, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetNationalityReport(startdate, enddate, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetNationalityReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<RoomMovedReportDTO> GetRoomMovedReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRoomMovedReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoomMovedReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static PMSDashBoardReport GetPMSDashBoardReport(DateTime Date, int consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetPMSDashBoardReport(Date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPMSDashBoardReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwRoomActivityViewDTO> GetRoomActivityView(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetRoomActivityView(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoomActivityView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<HKActivityReportDTO> GetHouseKeepingActivityReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetHouseKeepingActivityReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRoomActivityView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<HKTaskAssignmentReportDTO> GetTaskAssignmentReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetTaskAssignmentReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTaskAssignmentReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<DiscripancyReportDTO> GetDiscripancyReport(DateTime date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsReportRequest.GetDiscripancyReport(date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetDiscripancyReport" + Environment.NewLine + ex.Message);
                return null;
            }
        }
        #endregion

        public static List<VoucherDetailReportViewDTO> GetVoucherDetailReportForSummary(DateTime date, int Consigneeunit)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("issuedDate", date.ToString());
                Dictionaryvalue.Add("isIssued", "true");
                Dictionaryvalue.Add("isVoid", "false");
                Dictionaryvalue.Add("voucherDefinition", "106");
                return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherDetailReportViewDTO>>("VoucherDetailReportView", Dictionaryvalue)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetVoucherDetailReportForSummary" + Environment.NewLine + ex.Message);
                return new List<VoucherDetailReportViewDTO>();
            }
        }

        public static List<HeldTransaction> GetRMSHeldTransaction(int definition, int state)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetRMSHeldTransaction(definition, state)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetRMSHeldTransaction" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static TotalRebateDTO GetTotalRebate(DateTime Date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetTotalRebate(Date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTotalRebate" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<GetAllOrderStationMapView> GetUniqueOrderStationByPoSMachine()
        {
            try
            {
                List<GetAllOrderStationMapView> result = Task.Run(async () => await pmsLibraryRequest.GetAllOrderStationMapView()).Result;

                if (result != null)
                    result = result.GroupBy(o => o.name).Select(o => o.First()).ToList();


                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTotalRebate" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<OrderStationConsumption> GetConsumptionCalculation(int shift, DateTime currentTime, int organazationUnitDefn, int stationDevice, int rounDigit, bool RemoveColsed)
        {

            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetConsumptionCalculation(shift, currentTime, organazationUnitDefn, stationDevice, rounDigit, RemoveColsed)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetConsumptionCalculation" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwActivityDetailViewDTO> GetActivityDetailView(int? Reference, DateTime? Date, int? ActivityDescLuk)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetActivityDetailView(Reference, Date, ActivityDescLuk)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetActivityDetailView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwPackageViewDTO> GetPackageView(DateTime Date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetPackageView(Date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetPackageView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<VwTravelDetailDTO> GetTravelDetailView(int Type, DateTime Date, int? consigneeunit)
        {
            try
            {
                return Task.Run(async () => await pmsLibraryRequest.GetTravelDetailView(Type, Date, consigneeunit)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: GetTravelDetailView" + Environment.NewLine + ex.Message);
                return null;
            }
        }

        public static List<FieldFormatDTO> Get_Field_Format_By_Reference(int reference)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("reference", reference.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<FieldFormatDTO>>("FieldFormat", Dictionaryvalue)).Result;
        }

        public static DeviceDTO GetDeviceByPreference(int preference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Preference", preference.ToString());
                List<DeviceDTO> returndevicelist = Task.Run(async () => await filterRequest.GetFilterData<List<DeviceDTO>>("Device", Dictionaryvalue)).Result;
                if (returndevicelist != null && returndevicelist.Count > 0)
                    return returndevicelist.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static VoucherDetailDTO get_voucher_detail(int Id)
        {
            return Task.Run(async () => await transactionRequest.get_voucher_detail(Id)).Result;
        }

        public static List<MiniConsigneeViewDTO> Get_Required_Consignees(int voucherDefinition)
        {
            return Task.Run(async () => await consigneeRequest.Get_Required_Consignees(voucherDefinition)).Result;
        }

        public static RegistrationPrintOutDTO GetRegistrationConformation(int Id)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetRegistrationConformation(Id)).Result;
        }

        public static List<VwVoucherHeaderDTO> Get_Document_Browser_Data(string filter)
        {
            var result = Task.Run(async () => await transactionRequest.Get_Document_Browser_Data(filter)).Result;
            return result.Data;

        }

        public static List<VwConsigneeViewDTO> Get_Consignee_Browser_Data(string filter)
        {
            return Task.Run(async () => await consigneeRequest.Get_Consignee_Browser_Data(filter)).Result;
        }

        public static List<VoucherDetailReportViewDTO> Get_voucherDetailReport(int Id)
        {
            return Task.Run(async () => await transactionRequest.Get_voucherDetailReport(Id)).Result;
        }

        public static List<VwRegistrationWithRoomStateViewDTO> GetRegistrationWithRoomStatus(DateTime Date, int State, int consigneeunit)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetRegistrationWithRoomStatus(Date, State, consigneeunit)).Result;
        }

        public static List<HkLoadDistributionDTO> AssignTask(DateTime assignmentDate, List<ConsigneeDTO> listOfEmployees, List<int> hkStatus, int foStatus, int taskCode, int Consigneeunit)
        {
            return Task.Run(async () => await pmsLibraryRequest.TaskAssign(assignmentDate, listOfEmployees, hkStatus, foStatus, taskCode, Consigneeunit)).Result;
        }

        public static PeriodDTO GetPeriodByDateAndType(DateTime Date, int type)
        {
            var response = Task.Run(async () => await commonRequest.GetPeriodByDateAndType(Date, type)).Result;
            if (response != null && response.Count > 0)
                return response.FirstOrDefault();
            else
                return null;
        }

        public static List<ViewFunctWithAccessMDTO> GetFuncwithAccessMatView(int currentRole, int Function)
        {
            return Task.Run(async () => await commonRequest.GetFuncwithAccessMatView(currentRole, Function)).Result;

        }

        public static UserRoleMapperDTO GetUserRoleByUserId(int userid)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("User", userid.ToString());
            var returnval = Task.Run(async () => await filterRequest.GetFilterData<List<UserRoleMapperDTO>>("UserRoleMapper", Dictionaryvalue)).Result;
            if (returnval != null && returnval.Count > 0)
                return returnval.FirstOrDefault();
            else
                return null;

        }

        public static List<AttachmentDTO> GetAttachmentByReference(int Reference)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Reference", Reference.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<AttachmentDTO>>("Attachment", Dictionaryvalue)).Result;
        }
        public static List<VoucherLookupListDTO> GetVoucherLookupListByVoucher(int Voucher)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Voucher", Voucher.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<VoucherLookupListDTO>>("VoucherLookupList", Dictionaryvalue)).Result;
        }
        
        public static ClosingValidationDTO SelectClosingValidationByComponentandBranch(int PMS_Pointer, int selectedHotelcode)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("systemConstant", PMS_Pointer.ToString());
            Dictionaryvalue.Add("status", selectedHotelcode.ToString());
            var returnval = Task.Run(async () => await filterRequest.GetFilterData<List<ClosingValidationDTO>>("ClosingValidation", Dictionaryvalue)).Result;
            if (returnval != null && returnval.Count > 0)
                return returnval.LastOrDefault();
            else
                return null;
        }

        public static ActivityDefinitionDTO GetActivityDefinitionByDescription(object printed)
        {
            throw new NotImplementedException();
        }

        public static ResponseModel<List<LicenseDTO>> Read_All_Licenses(string TIN)
        {
            return Task.Run(async () => await securityRequest.Read_All_Licenses(TIN)).Result;
        }

        public static ResponseModel<LicenseDTO> Validate_POS_License(int subSystem, string TIN, string MRC)
        {
            try
            {
                return Task.Run(async () => await securityRequest.Validate_POS_License(subSystem, TIN, MRC)).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Validate POS License!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        //  Get_Value_Factor_Definition_By_Reference_And_Type
        public static List<ValueFactorDefinitionDTO> Get_Value_Factor_Definition_By_Reference_And_Type(int reference, int type, int? consigneeUnit = null)
        {
            return Task.Run(async () => await commonRequest.Get_Value_Factor_Definition_By_Reference_And_Type(reference, type, consigneeUnit)).Result;
        }
        public static ConsigneeUnitDTO Get_ConsigneeUnit_By_Code(string code)
        {
            return Task.Run(async () => await commonRequest.Get_ConsigneeUnit_By_Code(code)).Result;
        }
        public static ResponseModel<List<LicenseDTO>> ReadLicense(string tin)
        {
            return Task.Run(async () => await commonRequest.ReadLicense(tin)).Result;
        } 
        public static ConsigneeDTO Get_Consignee_By_Code(string code)
        {
            return Task.Run(async () => await commonRequest.Get_Consignee_By_Code(code)).Result;
        }
        public static List< PreferenceDTO> Get_Preference_By_Reference(int Reference)
        {
            return Task.Run(async () => await commonRequest.Get_Preference_By_Reference(Reference)).Result;
        }
        public static VwRequiredGslDTO Get_VwRequiredGslDTO_By_VoucherDefn_Type(int voucherdef, int type, int gsltype)
        {
            return Task.Run(async () => await commonRequest.Get_VwRequiredGslDTO_By_VoucherDefn_Type(voucherdef, type, gsltype)).Result;
        }
        public static List<VwRequiredGslDTO> Get_VwRequiredGslDTO_By_VoucherDefn_Type(int voucherdef, int type)
        {
            return Task.Run(async () => await commonRequest.Get_VwRequiredGslDTO_By_VoucherDefn_Type(voucherdef, type)).Result;
        }
        public static List<VwOrderStationMapDTO> Get_VwOrderStationMap(int PosDevice)
        {
            return Task.Run(async () => await commonRequest.Get_VwOrderStationMap(PosDevice)).Result;
        }
        public static ConsigneeBuffer Save_Consignee_Buffer(ConsigneeBuffer consignee)
        {
            return Task.Run(async () => await consigneeRequest.CreateConsigneeBuffer(consignee)).Result;
        }
        public static List<EventDisplayView> GetEventDisplayView(int id)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetEventDisplayView(id)).Result;
        }

        public static List<EventDisplayView> GetEventDisplayView(DateTime startdate, DateTime EndTime, int? consigneeunit)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetEventDisplayView(startdate, EndTime, consigneeunit)).Result;
        }
        public static EventFolioDTO GetEventFolioData(int id)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetEventFolioData(id)).Result;
        }
        public static List<EventRequirementView> GetEventRequirementView(int id)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetEventRequirementView(id)).Result;
        }

        public static List<EventHeaderView> GetEventHeaderView(DateTime startdate, DateTime EndTime, int? consigneeunit)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetEventHeaderView(startdate, EndTime, consigneeunit)).Result;
        }
        public static EventHeaderView GetEventHeaderViewById( int id)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetEventHeaderViewById(id)).Result;
        }
        
        public static List<PriceMethod> Get_Article_Values(int articleId, int voucherDefn, int? consignee, int consigneeUnit, decimal qty, DateTime enquiryDate, int deviceId, bool isPos = true)
        {
            return Task.Run(async () => await commonRequest.Get_Article_Values(articleId, voucherDefn,  consignee,  consigneeUnit,  qty,  enquiryDate,  deviceId, isPos)).Result;
        }
    }
}
