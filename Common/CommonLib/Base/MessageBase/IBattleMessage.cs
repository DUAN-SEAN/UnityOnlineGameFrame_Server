namespace Crazy.Common
{
    public interface IBattleMessage : IMessage
    {
        ulong BattleId { get; set; }
    }
}
