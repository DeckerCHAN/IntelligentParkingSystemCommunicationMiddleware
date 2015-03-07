namespace IPSCM.Entities.Results
{
    public enum ResultCode
    {
        Success = 200,
        SuccessButNoBinding = 210,
        SuccessButInsufficientFunds = 220,
        WrongArguments = 400,
        WrongToken = 410,
        WrongSign = 411,
        ServerFailure = 500
    }
}
