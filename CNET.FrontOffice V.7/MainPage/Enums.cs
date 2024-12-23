namespace CNET.ERP.Client.UI_Logic.PMS.Enum
{
    public enum SaveResult
    {
        SAVE_SUCESSESFULLY,
        SAVE_THENRESET,
        SAVE_THENSHOWNOTHING,
        SAVE_THENSHOWNOTHINGREESET
    }

    public enum DeleteResult
    {
        DELETE_SUCESSESFULLY,
        DELETE_THENRESET,
        DELETE_THENSHOWNOTHING,
        DELETE_THENSHOWNOTHINGRESET
    }

    public enum MessageType
    {
        ALLERT,
        MESSAGEBOX
    }




    public enum ExceptionHandling
    {
        RETURN,
        SHOWMESSAGE,
        SHOWMESSAGEANDLOG,
        SHOWMESSAGEANDLOGTHENTHROW,
        THROW
    }

    //public class SaveClickedResult
    //{
    //    public SaveResult SaveResult;
    //    public MessageType MessageType;
    //}


    //public class DeleteClickedResult
    //{
    //    public DeleteResult DeleteResult;
    //    public MessageType MessageType;
    //}
     
}
