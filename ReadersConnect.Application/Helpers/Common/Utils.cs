namespace ReadersConnect.Application.Helpers.Common
{
    public static class Utils
    {
        //STATUS CODES
        public const string StatusCode_Success = "10000";
        public const string StatusCode_UnExpectedError = "10001";
        public const string StatusCode_InvalidRequest = "10002";
        public const string StatusCode_UnAuthorised = "10003";
        public const string StatusCode_DuplicateRequest = "10005";
        public const string StatusCode_AmountLimitExceeded = "10006";

        //CONTROLLER ENDPOINT RESPONSE CODES
        public const string Response_Success = "00";
        public const string Response_Failure = "99";

        //STATUS MESSAGES
        public const string StatusMessage_Success = "Request Successful";
        public const string StatusMessage_UnKnownError = "Unknown Error Occured while performing this Action";
        public const string StatusMessage_TokenValueNull = "Authorization Token Value is Null";
        public const string StatusMessage_BadRequest = "Required Request Parameter is Invalid / Missing";
        public const string StatusMessage_Unauthorized = "Authorization Token is UnAuthorized";
        public const string StatusMessage_PartialContent = "Invalid Response From Service";
        public const string StatusMessage_ForbiddenError= "User Not Authorized";
        public const string StatusMessage_Failure = "Request Failed";
        public const string StatusMessage_DatabaseConnectionTimeOut = "Database Connection TimeOut";
        public const string StatusMessage_StoredProcedureError = "Stored Procedure Execution Failed";
        public const string StatusMessage_ExceptionError = "An Exception Occured";
        public const string StatusMessage_DatabaseConnectionError = "Database Connection Error";
        public const string StatusMessage_EncryptionError = "Error Encountered During Encryption / Decryption";
        public const string StatusMessage_RestApiCallNotSuccessful = "Rest API call Not Successful";

        //CONTROLLER ENDPOINT RESPONSE DESCRIPTION
        public const string ResponseDescription_Success = "Request Successful";
        public const string ResponseDescription_DatabaseConnectionTimeout = "Database Connection Timeout Occured";
        public const string ResponseDescription_UnKnownError = "An Error Occured While Performing Action. Try Again Later";
        public const string ResponseDescription_DatabaseConnectionError = "System Malfunction";
        public const string ResponseDescription_Exception = "Exception Occured While Performing Action";

        //APPLICATION CODE HTTP STSTUS CODES
        // public const int HttpStatusCode_OK = StatusCodes.;

        //USER ROLES
        //public const string Role_Admin = "Admin";
        //public const string Role_Developer = "Developer";
        //public const string Role_Manager = "Property Manager";
        //public const string Role_Manager_Asst = "Property Manager Assistant";
        //public const string Role_Renter = "Renter";
    }
}