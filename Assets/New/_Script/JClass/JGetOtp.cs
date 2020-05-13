[System.Serializable]
public class JGetOtp
{
    public JOtp otp;
}

[System.Serializable]
public struct JOtp
{
    public string otp_key;
    public int num_of_try;
    public string expired_at;
}