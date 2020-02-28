using UnityEngine;
using System.Collections;

public class SyncVariableType
{
    public static string PlayerProfile = "player_profile";
    public static string String = "string";
    public static string BoolArray = "bool_array";
    public static string IntArray = "int_array";
}

public enum ControllerType
{
    Player, Bot,
}

public enum MenuOrBoxType
{
    MainMenu, SettingBox, DailyQuestBox, LeaderboardBox, Mailbox, ExploreChestBox, GetSocialBox, MyProfileBox, GameSelectionMenu, 
    GameRelatedMenu, OtherProfileBox, RegisterMenu, LoginMenu, LoginOthersMenu, AvatarMenu, AvatarDetailBox, IngamePauseBox, 
    PrivateChatUI, MessageBox, CreatePrivate, ItemReceivedBox, InviteFriends, SearchPlayers, RoomListMenu, Garage, FreeCreditUI, 
    LuckySpinUI, InviteCodeUI, SlotMachineUI, SlotMachineInfo, PaymentUI, TransferCoinUI, TransferLogUI, VerifyUI, ChestBoxUI, 
    ChestPreviewUI, AchievementUI, BindAccountUI, PromoCodeUI, ContactSupportUI, HelpGameUI, VipUI, VipRewardsUI, NewsEventUI, SpecialEventUI, None
}

public enum EnvironmentType
{
    None, Jakarta, Bali, Yogyakarta, Medan, Sulawesi, Surabaya, Padang, Papua
}

public enum GameType
{
    CapsaSusun,
    TexasPoker,
    None
}

public enum CardPointType
{
    Club, Diamond, Heart, Spade,
}

public enum Suit { Clubs, Diamonds, Hearts, Spades };
public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, Max };
public enum HandValue { HighCard = 100, Pair = 200, TwoPairs = 300, ThreeOfAKind = 400, Straight = 500, Flush = 600, FullHouse = 700, FourOfAKind = 800, StraightFlush = 900 };

public enum PointType
{
    None, HighCard08, HighCard09, HighCard10, HighCardJack, HighCardQueen, HighCardKing, HighCardAce,
    OnePair, OnePair08, OnePair09, OnePair10, OnePairJack, OnePairQueen, OnePairKing, OnePairAce,
    TwoPairs,TwoPairs08, TwoPairs09, TwoPairs10, TwoPairsJack, TwoPairsQueen, TwoPairsKing, TwoPairsAce,
    ThreeKinds, ThreeKind08, ThreeKind09, ThreeKind10, ThreeKindJack, ThreeKindQueen, ThreeKindKing, ThreeKindAce, 
    StraightSmall, StraightBig, StraightRoyal,
    FullHouse08, FullHouse09, FullHouse10, FullHouseJack, FullHouseQueen, FullHouseKing, FullHouseAce,
    FlushClub, FlushDiamond, FlushHeart, FlushSpade,
    FourKinds, FourKind08, FourKind09, FourKind10, FourKindJack, FourKindQueen, FourKindKing, FourKindAce,
    StraightFlushSmall, StraightFlushBig, RoyalFlush,
    Max,
}

//Buy Coins and Gems
public enum ShopItemType
{
    Gems,
    Coins,
    Badges,
    Houses,
    Cars,
    Stickers,
    None,
}


public enum IconType
{
    Car_001_SuzukiSwift, Car_002_HondaFit, Car_003_HondaCivic, Car_004_MazdaR8, Car_005_Mark2, Car_006_Lexus, Car_007_MitsubishiEvo,
    Car_008_NissanFairlady, Car_009_NissanGTR, Car_010_Chevrolet, Car_011_Ferrari, Car_Profile, icon_add, icon_add02, icon_arrow_down,
    icon_arrow01, icon_back, icon_chat, icon_clock, icon_close, icon_coin, icon_coin01, icon_coin02, icon_coin03, icon_coin04,
    icon_coins, icon_delete, icon_delete01, icon_email, icon_emoji, icon_enter, icon_experience01, icon_experience02, icon_experience03,
    icon_experience04, icon_facebook, icon_gem, icon_gem01, icon_gem02, icon_gem03, icon_gem04, icon_globe, icon_help,
    icon_leaderboard, icon_like, icon_lock, icon_lock_opened, icon_logout, icon_mell, Icon_Music, icon_pause, icon_playrivals,
    icon_refresh, icon_search, icon_settings, Icon_SFX, icon_shift, icon_shop, icon_special, icon_ticket01, icon_ticket02,
    icon_ticket03, icon_ticket04, icon_trophy_gold, icon_users, icon_video, icon_vip, icon_voicetalk, itunes_10, itunes_15, itunes_25,
    mpt_1000, mpt_3000, mpt_5000, ooredoo_1000, ooredoo_3000, ooredoo_5000, playstore_10, playstore_15, playstore_25, telenor_1000, telenor_3000,
    telenor_5000,
    Max,
}

public enum SFXType
{
    SFX_CardDistribute, SFX_CardDistributeOnce, SFX_Click, SFX_Lost, SFX_PopupClose, SFX_PopupOpen, SFX_Won, SFX_WinCoin, SFX_Promote, SFX_Timer, SFX_CardMatch, SFX_QuestComplete, SFX_OpenChest, SFX_Notification,
    SFX_ButtonDisable, SFX_RoundStart, SFX_CardPlace, SFX_Empty, SFX_CardA, SFX_CardIn, SFX_CardOut, SFX_ChestOpen, SFX_Demote, SFX_ExplosionEnd, SFX_ExplosionStart, SFX_Finish, SFX_LoseCount,
    SFX_MenuOpen, SFX_TabClick, SFX_TimeOver, SFX_WinA, SFX_WinB, SFX_WinC, SFX_WinHigh, SFX_WinLow, SFX_WinMed, SFX_CoinSpread, SFX_Wheel, SFX_StartVehicle, SFX_StartVehicle2, SFX_StartVehicle3,
    SFX_SpecialDragon, SFX_SpecialCard,
    SFX_StartSlot, SFX_SlotSegment, SFX_SlotWin1, SFX_CointCount, SFX_SlotWin2, SFX_SlotWin3, SFX_SlotEnd, SFX_StartgameSlot, SFX_PokerSwitch, SFX_PokerFold, SFX_PokerCall, SFX_PokerRaise,
    SFX_PokerTimer, SFX_PokerCheck, SFX_SlotDragonA, SFX_SlotDragonB, SFX_SlotTop, Max
}

public enum ScreenAspectRatio
{
    WideScreen_16x10,
    WideScreen_16x9,
    iPhone_3x2,
    iPad_4x3,
    WP8_5x3,
}

public enum MessageBoxType
{
    OK,
    ACCEPT,
    OK_CANCEL,
    ACCEPT_CANCEL,
    NO_BUTTONS,

}
