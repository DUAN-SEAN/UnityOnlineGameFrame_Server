using Crazy.Common;
namespace Crazy.Common
{
// ��ս��ϵͳ��ص���Ϣд������
	[Message(BattleMessageOpcode.C2S_ReadyBattleBarrierReq)]
	public partial class C2S_ReadyBattleBarrierReq : IBattleMessage {}

// ��ս��ϵͳ��ص���Ϣд������
	[Message(BattleMessageOpcode.S2CM_ReadyBattleBarrierAck)]
	public partial class S2CM_ReadyBattleBarrierAck : IBattleMessage {}

// ս��ָ��
	[Message(BattleMessageOpcode.C2S_BattleCommandMessage)]
	public partial class C2S_BattleCommandMessage : IBattleMessage {}

//����������Ϣ
	[Message(BattleMessageOpcode.S2C_BodyInitBattleMessage)]
	public partial class S2C_BodyInitBattleMessage : IBattleMessage {}

//�������������߼�
	[Message(BattleMessageOpcode.C2S_CommandBattleMessage)]
	public partial class C2S_CommandBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.S2C_EventBattleMessage)]
	public partial class S2C_EventBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.S2C_SyncHpShieldStateBattleMessage)]
	public partial class S2C_SyncHpShieldStateBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.S2C_SyncPhysicsStateBattleMessage)]
	public partial class S2C_SyncPhysicsStateBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.S2C_SyncLevelTaskBattleMessage)]
	public partial class S2C_SyncLevelTaskBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.TaskState)]
	public partial class TaskState {}

	[Message(BattleMessageOpcode.S2C_SyncSkillStateBattleMessage)]
	public partial class S2C_SyncSkillStateBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.SkillState)]
	public partial class SkillState {}

	[Message(BattleMessageOpcode.S2C_SyncLevelStateBattleMessage)]
	public partial class S2C_SyncLevelStateBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.C2S_ExitBattleMessage)]
	public partial class C2S_ExitBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.S2C_ExitBattleMessage)]
	public partial class S2C_ExitBattleMessage : IBattleMessage {}

	[Message(BattleMessageOpcode.S2CM_FinishBattleMessage)]
	public partial class S2CM_FinishBattleMessage : IBattleMessage {}

}
namespace Crazy.Common
{
	public static partial class BattleMessageOpcode
	{
		 public const ushort C2S_ReadyBattleBarrierReq = 1001;
		 public const ushort S2CM_ReadyBattleBarrierAck = 1002;
		 public const ushort C2S_BattleCommandMessage = 1003;
		 public const ushort S2C_BodyInitBattleMessage = 1004;
		 public const ushort C2S_CommandBattleMessage = 1005;
		 public const ushort S2C_EventBattleMessage = 1006;
		 public const ushort S2C_SyncHpShieldStateBattleMessage = 1007;
		 public const ushort S2C_SyncPhysicsStateBattleMessage = 1008;
		 public const ushort S2C_SyncLevelTaskBattleMessage = 1009;
		 public const ushort TaskState = 1010;
		 public const ushort S2C_SyncSkillStateBattleMessage = 1011;
		 public const ushort SkillState = 1012;
		 public const ushort S2C_SyncLevelStateBattleMessage = 1013;
		 public const ushort C2S_ExitBattleMessage = 1014;
		 public const ushort S2C_ExitBattleMessage = 1015;
		 public const ushort S2CM_FinishBattleMessage = 1016;
	}
}
