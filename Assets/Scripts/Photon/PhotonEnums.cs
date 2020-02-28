using UnityEngine;
using System.Collections;

namespace PhotonEnums
{
    public class GameType
    {
        //Custom room properties
        public static string TexasPoker = "texas_poker";
        
    }

    public class Room
    {
        //Custom room properties
        public static string BotCount = "bot_count"; //For Room Selection Item

        public static string GameType = "game_type";
        public static string Environment = "environment";

        public static string Slots = "slots";

        public static string MinimumBet = "minimum_bet";

        public static string Password = "password";

        public static string MasterClientID = "master_client_id";
        public static string BetType = "bet_type";
        public static string PokerCardTable = "PokerCardTable";
        public static string LastIndexDealer = "last_dealer";
        ////Room List
        //public static string PlayerName = "players_name";
        //public static string GenderPlayers = "gender_player";
        //public static string PhotoPlayers = "photo_player";        
    }

    public class Player
    {
        //Player custom properties
        public static string ContentURL = "content_url";
        public static string Name = "name";
        
        public static string Active = "active";
        public static string NextRoundIn = "next_round_in"; //Whether the player participate in the next round in Show Game
        public static string ReadyInitialized = "ready_initialized";

        public static string IsBot = "is_bot";

        public static string SlotIndex = "slot_index";
        public static string Money = "money";

        public static string PictureURL = "picture_url";
        public static string Gender = "gender_avatar";
        public static string PlayerID = "player_id";
        public static string Cards = "cards";

        public static string TotalPlayed = "total_played";
        public static string WinRate = "win_rate";
        public static string LevelChar = "level_char";

        #region Score Texas Poker
        public static string RankPoker = "rank_poker";
        public static string HandRank = "hand_rank";
        public static string ValueHand = "value_hand";
        public static string SecondValueHand = "second_value_hand";

        public static string Kicker = "kicker";
        public static string SecondKicker = "second_kicker";    

        public static string ChipsBet = "chips_bet";
        public static string TotalBet = "total_bet";
        #endregion

        #region Capsa Susun
        public static string SusunComplete = "susun_complete";

        public static string SpecialRank = "special_rank";
        public static string TopSetRank = "top_set_rank";
        public static string MiddleSetRank = "middle_set_rank";
        public static string BottomSetRank = "bottom_set_rank";
        public static string TopSetHighCard = "top_set_high_card";
		public static string TopSetHighCardType = "top_set_high_card_type";
        public static string MiddleSetHighCard = "middle_set_high_card";
		public static string MiddleSetHighCardType = "middle_set_high_card_type";
		public static string MiddleSetSecondHighCard = "middle_set_2nd_high_card";
		public static string MiddleSetSecondHighCardType = "middle_set_2nd_high_card_type";
        public static string BottomSetHighCard = "bottom_set_high_card";
		public static string BottomSetHighCardType = "bottom_set_high_card_type";
		public static string BottomSetSecondHighCard = "bottom_set_2nd_high_card";
		public static string BottomSetSecondHighCardType = "bottom_set_2nd_high_card_type";
        public static string TopSetSecondHighCard = "top_set_2nd_high_card";
        public static string TopSetThirdHighCard = "top_set_3rd_high_card";
        public static string MiddleSetThirdHighCard = "middle_set_3rd_high_card";
        public static string MiddleSetFourthHighCard = "middle_set_4th_high_card";
        public static string MiddleSetFifthHighCard = "middle_set_5th_high_card";
        public static string BottomSetThirdHighCard = "bottom_set_3rd_high_card";
        public static string BottomSetFourthHighCard = "bottom_set_4th_high_card";
        public static string BottomSetFifthHighCard = "bottom_set_5th_high_card";
        public static string TopSetTotal = "top_set_total";
        public static string MiddleSetTotal = "middle_set_total";
        public static string BottomSetTotal = "bottom_set_total";

        public static string SpecialRankResult = "special_rank_result";
        public static string SpecialRankScore = "special_rank_scorea";
        public static string ScoopResult = "scoop_result";

        public static string TopScore = "top_score";
        public static string MiddleScore = "middle_score";
        public static string BottomScore = "bottom_score";
        public static string TopResult = "top_result";
        public static string MiddleResult = "middle_result";
        public static string BottomResult = "bottom_result";
        public static string ScoopCount = "scoop_count";
        #endregion
    }

    public class RPC
    {
        public static string QuitGameRPC = "QuitGameRPC";
        public static string QuitInTheMiddleRPC = "QuitInTheMiddleRPC";
        public static string RPC_IMBankrupt = "RPC_IMBankrupt";

        //Social
        public static string SetMyStartGameRPC = "SetMyStartGameRPC";
        public static string SetMyStartPokerRPC = "SetMyStartPokerRPC";

        public static string PrepareRoundRPC = "PrepareRoundRPC";
        public static string ShowConnectedBoxRPC = "ShowConnectedBoxRPC";

        public static string GameStartedRPC = "GameStartedRPC";
        public static string RestartGameRoundRPC = "RestartGameRoundRPC";

        public static string CardReceivedRPC = "CardReceivedRPC";

        //New
        public static string RPC_AssignBot = "RPC_AssignBot";
        public static string RPC_RequestSlot = "RPC_RequestSlot";
        public static string RPC_ReturnSlot = "RPC_ReturnSlot";
        public static string RPC_RequestInitializeSync = "RPC_RequestInitializeSync";
        public static string RPC_TimerSpectatorSync = "RPC_TimerSpectatorSync";

        public static string RPC_ShowResult = "RPC_ShowResult";
        public static string RPC_SyncResultBot = "RPC_SyncResultBot";

        //Poker
        public static string RPC_StartMatchTogether = "RPC_StartMatchTogether";
        public static string RPC_SyncNextTurn = "RPC_SyncNextTurn";
        public static string RPC_ForceSyncBotMoney = "RPC_ForceSyncBotMoney";

        public static string RPC_SyncCalculateEndRound = "RPC_SyncCalculateEndRound";
        public static string RPC_SyncCatchupOnTable = "RPC_SyncCatchupOnTable";
        public static string RPC_TakeYourPotTogether = "RPC_TakeYourPotTogether";
        public static string RPC_ForceQuitMatch = "RPC_ForceQuitMatch";
    }
}