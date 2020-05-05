using Crazy.Common;
namespace Crazy.Common
{
////��֤��½��Ϣ
	[Message(TestMessageOpcode.C2S_TestMessage)]
	public partial class C2S_TestMessage : IMessage {}

	[Message(TestMessageOpcode.S2C_AllocPlayerIdMessage)]
	public partial class S2C_AllocPlayerIdMessage : IMessage {}

	[Message(TestMessageOpcode.Vector3)]
	public partial class Vector3 {}

////���ͬ������λ�õ���Ϣ
	[Message(TestMessageOpcode.SyncPlayerEntityBattleMessage)]
	public partial class SyncPlayerEntityBattleMessage : ISyncBattleMessage {}

	[Message(TestMessageOpcode.SyncChomperEntityPhysicStateBattleMessage)]
	public partial class SyncChomperEntityPhysicStateBattleMessage : ISyncBattleMessage {}

	[Message(TestMessageOpcode.SyncChomperEntityAnimationStateBattleMessage)]
	public partial class SyncChomperEntityAnimationStateBattleMessage : ISyncBattleMessage {}

	[Message(TestMessageOpcode.SyncEnemyEntityAnimationStateBattleMessage)]
	public partial class SyncEnemyEntityAnimationStateBattleMessage : ISyncBattleMessage {}

	[Message(TestMessageOpcode.SyncStartLevelBattleMessage)]
	public partial class SyncStartLevelBattleMessage : ISyncBattleMessage {}

	[Message(TestMessageOpcode.C2B_EnterClubBattleReqMessage)]
	public partial class C2B_EnterClubBattleReqMessage : IRequest {}

	[Message(TestMessageOpcode.B2C_EnterClubBattleAckMessage)]
	public partial class B2C_EnterClubBattleAckMessage : IResponse {}

	[Message(TestMessageOpcode.B2C_InitPlayerEntityBattleMessage)]
	public partial class B2C_InitPlayerEntityBattleMessage : IMessage {}

	[Message(TestMessageOpcode.B2C_MissingPlayerEntityBattleMessage)]
	public partial class B2C_MissingPlayerEntityBattleMessage : IMessage {}

	[Message(TestMessageOpcode.C2L_CreateRoomReqMessage)]
	public partial class C2L_CreateRoomReqMessage : IRequest {}

	[Message(TestMessageOpcode.L2C_CreateRoomAckMessage)]
	public partial class L2C_CreateRoomAckMessage : IResponse {}

	[Message(TestMessageOpcode.C2L_JoinRoomReqMessage)]
	public partial class C2L_JoinRoomReqMessage : IRequest {}

	[Message(TestMessageOpcode.L2C_JoinRoomAckMessage)]
	public partial class L2C_JoinRoomAckMessage : IResponse {}

	[Message(TestMessageOpcode.C2L_GetRoomInfoReqMessage)]
	public partial class C2L_GetRoomInfoReqMessage : IRequest {}

	[Message(TestMessageOpcode.L2C_GetRoomInfoAckMessage)]
	public partial class L2C_GetRoomInfoAckMessage : IResponse {}

	[Message(TestMessageOpcode.C2L_GetRoomListInfoReqMessage)]
	public partial class C2L_GetRoomListInfoReqMessage : IRequest {}

	[Message(TestMessageOpcode.L2C_GetRoomListInfoAckMessage)]
	public partial class L2C_GetRoomListInfoAckMessage : IResponse {}

	[Message(TestMessageOpcode.RoomInfoMessage)]
	public partial class RoomInfoMessage {}

	[Message(TestMessageOpcode.C2G_AllocBattleServerMessageReq)]
	public partial class C2G_AllocBattleServerMessageReq : IRequest {}

	[Message(TestMessageOpcode.C2G_AllocBattleServerMessageAck)]
	public partial class C2G_AllocBattleServerMessageAck : IResponse {}

}
namespace Crazy.Common
{
	public static partial class TestMessageOpcode
	{
		 public const ushort C2S_TestMessage = 1060;
		 public const ushort S2C_AllocPlayerIdMessage = 1061;
		 public const ushort Vector3 = 1062;
		 public const ushort SyncPlayerEntityBattleMessage = 1063;
		 public const ushort SyncChomperEntityPhysicStateBattleMessage = 1064;
		 public const ushort SyncChomperEntityAnimationStateBattleMessage = 1065;
		 public const ushort SyncEnemyEntityAnimationStateBattleMessage = 1066;
		 public const ushort SyncStartLevelBattleMessage = 1067;
		 public const ushort C2B_EnterClubBattleReqMessage = 1068;
		 public const ushort B2C_EnterClubBattleAckMessage = 1069;
		 public const ushort B2C_InitPlayerEntityBattleMessage = 1070;
		 public const ushort B2C_MissingPlayerEntityBattleMessage = 1071;
		 public const ushort C2L_CreateRoomReqMessage = 1072;
		 public const ushort L2C_CreateRoomAckMessage = 1073;
		 public const ushort C2L_JoinRoomReqMessage = 1074;
		 public const ushort L2C_JoinRoomAckMessage = 1075;
		 public const ushort C2L_GetRoomInfoReqMessage = 1076;
		 public const ushort L2C_GetRoomInfoAckMessage = 1077;
		 public const ushort C2L_GetRoomListInfoReqMessage = 1078;
		 public const ushort L2C_GetRoomListInfoAckMessage = 1079;
		 public const ushort RoomInfoMessage = 1080;
		 public const ushort C2G_AllocBattleServerMessageReq = 1081;
		 public const ushort C2G_AllocBattleServerMessageAck = 1082;
	}
}
