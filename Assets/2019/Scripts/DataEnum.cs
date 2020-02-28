public enum PrefKey
{
    Email,
    Password,
    PlayerID,
    Token,
    Language,       //1 = bahasa, 0 = english
    Tutorial,
    Gender,         //0 = not set, 1 = male, 2 = female
    VolSFX,
    VolBGM,
    AdsLeft,
    AdsLastDate,
    BankruptLeft,
    LoginType,
    DontShowToday,
    Timestamp,
    NewAvatar,
    NewVehicle,
    NewIAP,
    RestoreSKU,
    RestoreInvoice,
    RestoreKey
}

public enum LoginType
{
    FB,
    Google,
    Other,
    None
}

public enum ActivityType
{
    UserPlayCapsun = 8,
    SpecialHandCapsun = 9,
    CardDragonKing = 10,
    CardDragon = 11,
    UserSendGift = 32,
    UserWinCapsun = 33,
    UserBuyIAP = 36,
    UserPlayOnRoom5 = 57,       //Sulawesi Capsa
    UserPlayOnRoom6 = 58,       //Surabaya Capsa
    UserPlayOnRoom7 = 59,       //Padang Capsa
    UserPlayOnRoom8 = 60,        //Papua Capsa
    UserLevelUp = 63,
    UserBankrupt = 68,
    UserNumOfFriend = 69,
    UserOpenChest = 48,
    UserNumOfExplore = 70,
    UserNumOfAvatar = 71,
    UserNumOfVehicle = 72,
    UserSpendGem = 74
}