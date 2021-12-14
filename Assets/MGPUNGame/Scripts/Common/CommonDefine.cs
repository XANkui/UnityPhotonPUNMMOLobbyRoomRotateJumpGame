using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGPUNGame2 { 

	public class CommonDefine
	{
		
		private static string mNickName = "游客";
		public static string NickName
		{
			get
			{
				int value = Random.Range(0, 9999);
				return mNickName + value.ToString();
			}
		}

		private static string mRoomName = "Room";
		public static string RoomName
		{
			get
			{
				int value = Random.Range(0, 9999);
				return mRoomName + value.ToString();
			}
		}

		public const string PUN_GAME_LEVEL = "MG_RotateJumpGame";
		public const string PUN_MAINMENU_LEVEL = "MG_MainMenu";

		public const int MAX_COUNT_PLAYER_PER_ROOM = 3;
		public const string PLAYER_SURE = "PlayerSure";
		public const string PLAYER_SURE_ROLE = "PlayerSureRole";

		public const string PLAYER_PREFAB_BASEPATH = "Prefabs/";
		public const string PLAYER_LOADED_LEVEL = "GameScene";

		public const string ROOM_PROPERTIES_TO_START_GAME = "StartGame";
		public const string ROOM_ROTATE_JUMP_START_COUNT_TIMER = "RotateJumpGameCountTime";
		public const float ROOM_ROTATE_JUMP_GAME_TIME = 10.0f;
		


	}
}
