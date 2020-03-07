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
	[Message(TestMessageOpcode.C2S_PlayerSyncEntityMessage)]
	public partial class C2S_PlayerSyncEntityMessage : IMessage {}

	[Message(TestMessageOpcode.C2B_EnterClubBattleReqMessage)]
	public partial class C2B_EnterClubBattleReqMessage : IRequest {}

	[Message(TestMessageOpcode.B2C_EnterClubBattleAckMessage)]
	public partial class B2C_EnterClubBattleAckMessage : IResponse {}

	[Message(TestMessageOpcode.B2C_InitPlayerEntityBattleMessage)]
	public partial class B2C_InitPlayerEntityBattleMessage : IMessage {}

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

}
namespace Crazy.Common
{
	public static partial class TestMessageOpcode
	{
		 public const ushort C2S_TestMessage = 1054;
		 public const ushort S2C_AllocPlayerIdMessage = 1055;
		 public const ushort Vector3 = 1056;
		 public const ushort C2S_PlayerSyncEntityMessage = 1057;
		 public const ushort C2B_EnterClubBattleReqMessage = 1058;
		 public const ushort B2C_EnterClubBattleAckMessage = 1059;
		 public const ushort B2C_InitPlayerEntityBattleMessage = 1060;
		 public const ushort C2L_CreateRoomReqMessage = 1061;
		 public const ushort L2C_CreateRoomAckMessage = 1062;
		 public const ushort C2L_JoinRoomReqMessage = 1063;
		 public const ushort L2C_JoinRoomAckMessage = 1064;
		 public const ushort C2L_GetRoomInfoReqMessage = 1065;
		 public const ushort L2C_GetRoomInfoAckMessage = 1066;
		 public const ushort C2L_GetRoomListInfoReqMessage = 1067;
		 public const ushort L2C_GetRoomListInfoAckMessage = 1068;
		 public const ushort RoomInfoMessage = 1069;
	}
}
